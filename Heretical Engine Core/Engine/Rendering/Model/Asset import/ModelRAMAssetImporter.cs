// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

//DO NOT DO THIS FOR NOW. SOME ASSET IMPORTERS ARE BLINDLY LOOKING FOR
//RESOURCES BY GIVEN ID EXPECTING THEM TO BE PRESENT IN THE RESOURCE MANAGER. PARALLELIZING THE LOAD PROCESS MAY CAUSE RACE
//CONDITIONS
//TODO: UPDATE CERTAIN RESOURCE IMPORTERS AND STORAGE HANDLES TO WAIT UNTIL RESOURCE BECOMES AVAILABLE
//#define PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES

using System;

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.Persistence.IO;

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.Math;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Scenes;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Logging;

using Silk.NET.Assimp;

using System.Numerics;

using AssimpMesh = Silk.NET.Assimp.Mesh;

using AssimpAPI = Silk.NET.Assimp.Assimp;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ModelRAMAssetImporter
		: AssetImporter
	{
		public const string MODEL_RAM_VARIANT_ID = "Model DTO";

		public const int MODEL_RAM_PRIORITY = 0;

		private const string MODEL_MESHES_NESTED_RESOURCE_ID = "Meshes";

		private const string MODEL_GEOMETRIES_NESTED_RESOURCE_ID = "Geometries";

		private const string MODEL_MATERIALS_NESTED_RESOURCE_ID = "Materials";

		private const string MODEL_TEXTURES_NESTED_RESOURCE_ID = "Textures";

		private readonly string resourceID;

		private readonly FilePathSettings filePathSettings;

		private readonly ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer;

		private readonly AssimpAPI assimp;

		public ModelRAMAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			FilePathSettings filePathSettings,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
			: base(
				resourceManager,
				logger)
		{
			this.resourceID = resourceID;

			this.filePathSettings = filePathSettings;

			this.mainThreadCommandBuffer = mainThreadCommandBuffer;

			assimp = AssimpAPI.GetApi();
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			var result = await ImportModel(
				filePathSettings,
				progress)
				.ThrowExceptions<IResourceVariantData, ModelRAMAssetImporter>(logger);

			progress?.Report(1f);

			return result;
		}

		private async Task<IResourceVariantData> ImportModel(
			FilePathSettings filePathSettings,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			List<AssetImporter> assetImporters = new List<AssetImporter>();

			var modelDTO = ImportModelAtPath(
				filePathSettings,
				assetImporters);

			progress?.Report(0.333f);

#if PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES
			int totalAssetImporters = assetImporters.Count;

			List<Task> assetImportersTasks = new List<Task>();

			List<float> assetImportProgresses = new List<float>();

			foreach (var assetImporter in assetImporters)
			{
				IProgress<float> localImportProgress = progress.CreateLocalProgress(
					0.333f,
					0.666f,
					assetImportProgresses,
					totalAssetImporters);

				assetImportersTasks.Add(
					Task.Run(
						() => assetImporter
							.Import(
								localImportProgress)
							.ThrowExceptions<IResourceVariantData, ModelRAMAssetImporter>(logger)));
			}

			await Task
				.WhenAll(assetImportersTasks)
				.ThrowExceptions<ModelRAMAssetImporter>(logger);
#else
			int totalAssetImporters = assetImporters.Count;

			int current = 0;

			foreach (var assetImporter in assetImporters)
			{
				IProgress<float> localImportProgress = progress.CreateLocalProgress(
					0.333f,
					0.666f,
					current,
					totalAssetImporters);

				await assetImporter
					.Import(
						localImportProgress)
					.ThrowExceptions<IResourceVariantData, ModelRAMAssetImporter>(logger);

				current++;
			}
#endif

			progress?.Report(0.666f);

			IProgress<float> localProgress = progress.CreateLocalProgress(
				0.666f,
				1f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, ModelRAMAssetImporter>(logger),
				new ResourceVariantDescriptor()
				{
					VariantID = MODEL_RAM_VARIANT_ID,
					VariantIDHash = MODEL_RAM_VARIANT_ID.AddressToHash(),
					Priority = MODEL_RAM_PRIORITY,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(ModelDTO),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ResourceManagementFactory.BuildConcurrentPreallocatedResourceStorageHandle(
					modelDTO),
#else
				ResourceManagementFactory.BuildPreallocatedResourceStorageHandle(
					modelDTO),
#endif
				true,
				localProgress)
				.ThrowExceptions<IResourceVariantData, ModelRAMAssetImporter>(logger);

			progress?.Report(1f);

			return result;
		}

		private unsafe ModelDTO ImportModelAtPath(
			FilePathSettings filePathSettings,
			List<AssetImporter> assetImporters)
		{
			ModelDTO result = default;

			var scene = assimp.ImportFile(
				filePathSettings.FullPath,
				(uint)PostProcessSteps.Triangulate);

			if (scene == null
				|| scene->MFlags == AssimpAPI.SceneFlagsIncomplete
				|| scene->MRootNode == null)
			{
				var error = assimp.GetErrorStringS();

				throw new Exception(error);
			}

			//DO NOT USE MName in scene. It's not documented in Assimp and I have no idea why it's even present
			//It does load some kind of bytes but both AsString and ToString throw exceptions so just don't use it
			//result.Name = scene->MName.AsString;

			result.Name = Path.GetFileNameWithoutExtension(filePathSettings.FullPath);

			result.RootNode = default;

			List<string> meshResourcesInAsset = new List<string>();

			Dictionary<string, string> textureResourcesInAsset = new Dictionary<string, string>();

			Dictionary<uint, string> materialResourcesInAsset = new Dictionary<uint, string>();

			ImportMaterials(
				resourceID,
				scene,
				ref result,
				textureResourcesInAsset,
				materialResourcesInAsset,
				assetImporters);

			ImportMeshes(
				resourceID,
				scene,
				ref result,
				meshResourcesInAsset,
				materialResourcesInAsset,
				assetImporters);

			ImportNodeRecursive(
				scene->MRootNode,
				scene,
				filePathSettings,
				ref result.RootNode,
				meshResourcesInAsset,
				materialResourcesInAsset,
				assetImporters);

			return result;
		}

		private unsafe void ImportMaterials(
			string resourceID,
			Scene* scene,
			ref ModelDTO modelDTO,
			Dictionary<string, string> textureResourcesInAsset,
			Dictionary<uint, string> materialResourcesInAsset,
			List<AssetImporter> assetImporters)
		{
			modelDTO.MaterialResourceIDs = new string[scene->MNumMaterials];

			for (int i = 0; i < scene->MNumMaterials; i++)
			{
				Material* material = scene->MMaterials[i];

				var materialImporter = BuildMaterialAssetImporter(
					resourceID,
					material,
					(uint)i,
					textureResourcesInAsset,
					materialResourcesInAsset,
					assetImporters,
					out string materialResourceID);

				assetImporters.Add(materialImporter);

				modelDTO.MaterialResourceIDs[i] = materialResourceID;
			}

			modelDTO.TextureResourceIDs = new string[textureResourcesInAsset.Count];

			for (int i = 0; i < modelDTO.TextureResourceIDs.Length; i++)
			{
				modelDTO.TextureResourceIDs[i] = $"{resourceID}/{MODEL_TEXTURES_NESTED_RESOURCE_ID}/{textureResourcesInAsset.Values.ElementAt(i)}";
			}
		}

		#region Material and texture asset importers

		private unsafe MaterialRAMAssetImporter BuildMaterialAssetImporter(
			string resourceID,
			Material* material,
			uint materialIndex,
			Dictionary<string, string> textureResourcesInAsset,
			Dictionary<uint, string> materialResourcesInAsset,
			List<AssetImporter> assetImporters,
			out string materialResourceID)
		{
			MaterialDTO materialDTO = default;

			GenerateMaterialResourceName(
				material,
				materialResourcesInAsset,
				out string materialName,
				out string materialResourceName);

			materialResourcesInAsset.Add(
				materialIndex,
				materialResourceName);

			List<string> textureResourcesInMaterial = new List<string>();

			// we assume a convention for sampler names in the shaders. Each diffuse texture should be named
			// as 'texture_diffuseN' where N is a sequential number ranging from 1 to MAX_SAMPLER_NUMBER. 
			// Same applies to other texture as the following list summarizes:
			// diffuse: texture_diffuseN
			// specular: texture_specularN
			// normal: texture_normalN

			//0. base color
			LoadMaterialTextures(
				material,
				TextureType.BaseColor,
				textureResourcesInAsset,
				textureResourcesInMaterial,
				assetImporters);

			// 1. diffuse maps
			LoadMaterialTextures(
				material,
				TextureType.Diffuse,
				textureResourcesInAsset,
				textureResourcesInMaterial,
				assetImporters);

			// 2. specular maps
			LoadMaterialTextures(
				material,
				TextureType.Specular,
				textureResourcesInAsset,
				textureResourcesInMaterial,
				assetImporters);

			// 3. normal maps
			LoadMaterialTextures(
				material,
				TextureType.Height,
				textureResourcesInAsset,
				textureResourcesInMaterial,
				assetImporters);

			// 4. height maps
			LoadMaterialTextures(
				material,
				TextureType.Ambient,
				textureResourcesInAsset,
				textureResourcesInMaterial,
				assetImporters);

			materialDTO.Name = materialName;

			materialDTO.ShaderResourceID = "Default shader"; //TODO: Implement shader selection

			materialDTO.TextureResourceIDs = new string[textureResourcesInMaterial.Count];

			for (int i = 0; i < materialDTO.TextureResourceIDs.Length; i++)
			{
				materialDTO.TextureResourceIDs[i] = $"{resourceID}/{MODEL_TEXTURES_NESTED_RESOURCE_ID}/{textureResourcesInMaterial[i]}";
			}

			materialResourceID = $"{resourceID}/{MODEL_MATERIALS_NESTED_RESOURCE_ID}/{materialResourceName}";

			return new MaterialRAMAssetImporter(
				resourceManager,
				materialResourceID,
				materialDTO,
				logger);
		}

		private unsafe void GenerateMaterialResourceName(
			Material* material,
			Dictionary<uint, string> materialResourcesInAsset,
			out string materialName,
			out string materialResourceName)
		{
			AssimpString materialNameInAsset;

			assimp.GetMaterialString(
				material,
				Assimp.MaterialNameBase,
				0,
				0,
				&materialNameInAsset);

			materialName = materialNameInAsset.AsString;

			materialResourceName = materialName;

			int redundancyCounter = 0;

			while (materialResourcesInAsset.Values.Contains(materialResourceName))
			{
				redundancyCounter++;

				materialResourceName = $"{materialName} ({redundancyCounter})";
			}
		}

		private unsafe void LoadMaterialTextures(
			Material* mat,
			TextureType type,
			Dictionary<string, string> textureResourcesInAsset,
			List<string> textureResourcesInMaterial,
			List<AssetImporter> assetImporters)
		{
			var textureCount = assimp.GetMaterialTextureCount(
				mat,
				type);

			for (uint i = 0; i < textureCount; i++)
			{
				AssimpString path;

				assimp.GetMaterialTexture(
					mat,
					type,
					i,
					&path,
					null,
					null,
					null,
					null,
					null,
					null);

				if (textureResourcesInAsset.ContainsKey(path))
				{
					var textureResourceName = textureResourcesInAsset[path];

					if (textureResourcesInMaterial.Contains(textureResourceName))
					{
						continue;
					}

					textureResourcesInMaterial.Add(textureResourceName);
				}
				else
				{
					var textureImporter = BuildTextureAssetImporter(
						resourceID,
						path.AsString,
						filePathSettings,
						textureResourcesInAsset,
						out string textureResourceName,
						out string textureResourceID);

					assetImporters.Add(textureImporter);

					textureResourcesInMaterial.Add(textureResourceName);
				}
			}
		}

		private unsafe TextureRAMAssetImporter BuildTextureAssetImporter(
			string resourceID,
			string textureRelativePathFromSource,
			FilePathSettings filePathSettings,
			Dictionary<string, string> textureResourcesInAsset,
			out string textureResourceName,
			out string textureResourceID)
		{
			var textureFilePathSettings = FindTexture(
				filePathSettings,
				textureRelativePathFromSource,
				out string textureFileNameWithoutExtension);

			GenerateTextureResourceName(
				textureFileNameWithoutExtension,
				textureResourcesInAsset,
				out textureResourceName);

			textureResourcesInAsset.Add(
				textureFilePathSettings.FullPath,
				textureResourceName);

			textureResourceID = $"{resourceID}/{MODEL_TEXTURES_NESTED_RESOURCE_ID}/{textureResourceName}";

			var textureImporter = new TextureRAMAssetImporter(
				resourceManager,
				textureResourceID,
				textureFilePathSettings,
				mainThreadCommandBuffer,
				logger);

			return textureImporter;
		}

		private FilePathSettings FindTexture(
			FilePathSettings filePathSettings,
			string textureRelativePathFromSource,
			out string textureFileNameWithoutExtension)
		{
			var result = new FilePathSettings();

			result.ApplicationDataFolder = filePathSettings.ApplicationDataFolder;


			var assetDirectoryName = Path.GetDirectoryName(filePathSettings.FullPath);


			var expectedTextureFullPath = Path
				.Combine(
					assetDirectoryName,
					textureRelativePathFromSource)
				.SanitizePath();

			var textureFileName = Path.GetFileName(expectedTextureFullPath);

			textureFileNameWithoutExtension = Path.GetFileNameWithoutExtension(expectedTextureFullPath);


			var expectedTextureFullPathIfPlacedByGuidelines = Path
				.Combine(
					assetDirectoryName,
					"../Textures",
					textureFileName)
				.SanitizePath();

			if (Path.Exists(expectedTextureFullPathIfPlacedByGuidelines))
			{
				result.FullPath = expectedTextureFullPathIfPlacedByGuidelines;
			}
			else
			{
				result.FullPath = expectedTextureFullPath;
			};

			return result;
		}

		private void GenerateTextureResourceName(
			string textureFileNameWithoutExtension,
			Dictionary<string, string> textureResourcesInAsset,
			out string textureResourceName)
		{
			string textureName = textureFileNameWithoutExtension;

			textureResourceName = textureName;

			int redundancyCounter = 0;

			while (textureResourcesInAsset.Values.Contains(textureResourceName))
			{
				redundancyCounter++;

				textureResourceName = $"{textureName} ({redundancyCounter})";
			}
		}

		#endregion

		private unsafe void ImportMeshes(
			string resourceID,
			Scene* scene,
			ref ModelDTO modelDTO,
			List<string> meshResourcesInAsset,
			Dictionary<uint, string> materialResourcesInAsset,
			List<AssetImporter> assetImporters)
		{
			modelDTO.MeshResourceIDs = new string[scene->MNumMeshes];

			modelDTO.GeometryResourceIDs = new string[scene->MNumMeshes];

			for (int i = 0; i < scene->MNumMeshes; i++)
			{
				Mesh* mesh = scene->MMeshes[i];

				var meshImporter = BuildMeshAssetImporter(
					resourceID,
					mesh,
					meshResourcesInAsset,
					materialResourcesInAsset,
					assetImporters,
					out string meshResourceID,
					out string geometryResourceID);

				assetImporters.Add(meshImporter);

				modelDTO.MeshResourceIDs[i] = meshResourceID;

				modelDTO.GeometryResourceIDs[i] = geometryResourceID;
			}
		}

		#region Mesh and geometry asset importers

		private unsafe MeshRAMAssetImporter BuildMeshAssetImporter(
			string resourceID,
			AssimpMesh* mesh,
			List<string> meshResourcesInAsset,
			Dictionary<uint, string> materialResourcesInAsset,
			List<AssetImporter> assetImporters,
			out string meshResourceID,
			out string geometryResourceID)
		{
			GenerateMeshResourceName(
				mesh,
				meshResourcesInAsset,
				out string meshResourceName);

			meshResourcesInAsset.Add(meshResourceName);

			var geometryImporter = BuildGeometryAssetImporter(
				resourceID,
				meshResourceName,
				mesh,
				out geometryResourceID);

			assetImporters.Add(geometryImporter);

			meshResourceID = $"{resourceID}/{MODEL_MESHES_NESTED_RESOURCE_ID}/{meshResourceName}";

			var materialResourceID = $"{resourceID}/{MODEL_MESHES_NESTED_RESOURCE_ID}/{materialResourcesInAsset[mesh->MMaterialIndex]}";

			return new MeshRAMAssetImporter(
				resourceManager,
				meshResourceID,
				new MeshDTO
				{
					GeometryResourceID = geometryResourceID,

					MaterialResourceID = materialResourceID
				},
				logger);
		}

		private unsafe void GenerateMeshResourceName(
			Mesh* mesh,
			List<string> meshResourcesInAsset,
			out string meshResourceName)
		{
			string meshName = mesh->MName.AsString;

			meshResourceName = meshName;

			int redundancyCounter = 0;

			while (meshResourcesInAsset.Contains(meshResourceName))
			{
				redundancyCounter++;

				meshResourceName = $"{meshName} ({redundancyCounter})";
			}
		}

		private unsafe GeometryRAMAssetImporter BuildGeometryAssetImporter(
			string resourceID,
			string meshResourceName,
			AssimpMesh* mesh,
			out string geometryResourceID)
		{
			List<Vertex> vertices = new List<Vertex>();

			List<uint> indices = new List<uint>();

			ParseVertices(
				mesh,
				vertices);

			ParseFaces(
				mesh,
				indices);

			geometryResourceID = $"{resourceID}/{MODEL_GEOMETRIES_NESTED_RESOURCE_ID}/{meshResourceName}";

			return new GeometryRAMAssetImporter(
				resourceManager,
				geometryResourceID,
				new Geometry
				{
					Vertices = BuildVertices(vertices),
					Indices = BuildIndices(indices)
				},
				logger);
		}

		private unsafe void ParseVertices(
			Mesh* mesh,
			List<Vertex> vertices)
		{
			// walk through each of the mesh's vertices
			for (uint i = 0; i < mesh->MNumVertices; i++)
			{
				Vertex vertex = new Vertex();

				vertex.BoneIds = new int[Vertex.MAX_BONE_INFLUENCE];

				vertex.Weights = new float[Vertex.MAX_BONE_INFLUENCE];

				vertex.Position = mesh->MVertices[i].ToSilkNetVector3D();

				// normals
				if (mesh->MNormals != null)
					vertex.Normal = mesh->MNormals[i].ToSilkNetVector3D();
				// tangent
				if (mesh->MTangents != null)
					vertex.Tangent = mesh->MTangents[i].ToSilkNetVector3D();
				// bitangent
				if (mesh->MBitangents != null)
					vertex.Bitangent = mesh->MBitangents[i].ToSilkNetVector3D();

				// texture coordinates
				if (mesh->MTextureCoords[0] != null) // does the mesh contain texture coordinates?
				{
					// a vertex can contain up to 8 different texture coordinates. We thus make the assumption that we won't 
					// use models where a vertex can have multiple texture coordinates so we always take the first set (0).
					Vector3 texcoord3 = mesh->MTextureCoords[0][i];

					vertex.TexCoords = new Vector2(texcoord3.X, texcoord3.Y).ToSilkNetVector2D();
				}

				vertices.Add(vertex);
			}
		}

		private unsafe void ParseFaces(
			Mesh* mesh,
			List<uint> indices)
		{
			// now wak through each of the mesh's faces (a face is a mesh its triangle) and retrieve the corresponding vertex indices.
			for (uint i = 0; i < mesh->MNumFaces; i++)
			{
				Face face = mesh->MFaces[i];

				// retrieve all indices of the face and store them in the indices vector
				for (uint j = 0; j < face.MNumIndices; j++)
					indices.Add(face.MIndices[j]);
			}
		}

		private float[] BuildVertices(List<Vertex> vertexCollection)
		{
			var vertices = new List<float>();

			foreach (var vertex in vertexCollection)
			{
				vertices.Add(vertex.Position.X);

				vertices.Add(vertex.Position.Y);

				vertices.Add(vertex.Position.Z);

				vertices.Add(vertex.TexCoords.X);

				vertices.Add(vertex.TexCoords.Y);
			}

			return vertices.ToArray();
		}

		private uint[] BuildIndices(List<uint> indices)
		{
			return indices.ToArray();
		}

		#endregion

		private unsafe void ImportNodeRecursive(
			Node* node,
			Scene* scene,
			FilePathSettings filePathSettings,
			ref ModelNodeDTO currentNodeDTO,
			List<string> meshResourcesInAsset,
			Dictionary<uint, string> materials,
			List<AssetImporter> assetImporters)
		{
			currentNodeDTO.Name = node->MName.AsString;

			currentNodeDTO.Children = new ModelNodeDTO[node->MNumChildren];


			Transform transform = default;

			transform.TRSMatrix = node->MTransformation.ToSilkNetMatrix4X4();

			transform.DecomposeTRSMatrix();

			TransformDTO transformDTO = new TransformDTO
			{
				Position = transform.Position.ToNumericsVector3(),

				Rotation = transform.Rotation.ToNumericsQuaternion(),

				Scale = transform.Scale.ToNumericsVector3()
			};

			currentNodeDTO.Transform = transformDTO;


			currentNodeDTO.MeshResourceIDs = new string[node->MNumMeshes];

			for (var i = 0; i < node->MNumMeshes; i++)
			{
				var meshResourceID = $"{resourceID}/{MODEL_MESHES_NESTED_RESOURCE_ID}/{meshResourcesInAsset[(int)(node->MMeshes[i])]}";

				currentNodeDTO.MeshResourceIDs[i] = meshResourceID;
			}

			for (var i = 0; i < node->MNumChildren; i++)
			{
				currentNodeDTO.Children[i] = default;

				ImportNodeRecursive(
					node->MChildren[i],
					scene,
					filePathSettings,
					ref currentNodeDTO.Children[i],
					meshResourcesInAsset,
					materials,
					assetImporters);
			}
		}
	}
}