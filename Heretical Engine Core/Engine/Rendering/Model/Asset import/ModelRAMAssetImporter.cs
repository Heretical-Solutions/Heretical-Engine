// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Silk.NET.Maths;

using Silk.NET.Assimp;

using System.Numerics;

using AssimpMesh = Silk.NET.Assimp.Mesh;

using AssimpAPI = Silk.NET.Assimp.Assimp;

using HereticalSolutions.Persistence.IO;

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.Math;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Scenes;
using HereticalSolutions.HereticalEngine.Rendering.Factories;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ModelRAMAssetImporter
		: AssetImporter
	{
		private const string MODEL_RAM_VARIANT_ID = "Model DTO";

		private const int MODEL_RAM_PRIORITY = 0;

		private const string MODEL_MESHES_NESTED_RESOURCE_ID = "Meshes";

		private const string MODEL_TEXTURES_NESTED_RESOURCE_ID = "Textures";

		private const string MODEL_MATERIALS_NESTED_RESOURCE_ID = "Materials";

		private readonly string resourceID;

		private readonly FileSystemSettings fsSettings;

		private readonly AssimpAPI assimp;

		public ModelRAMAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			FileSystemSettings fsSettings)
			: base(resourceManager)
		{
			this.resourceID = resourceID;

			this.fsSettings = fsSettings;

			assimp = AssimpAPI.GetApi();
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			var result = await ImportModelAtPath(
				fsSettings,
				progress);

			progress?.Report(1f);

			return result;
		}

		private async Task<IResourceVariantData> ImportModelAtPath(
			FileSystemSettings fsSettings,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			IProgress<float> localProgress = null;

			if (progress != null)
			{
				var localProgressInstance = new Progress<float>();

				localProgressInstance.ProgressChanged += (sender, value) =>
				{
					progress.Report(value * 0.5f);
				};

				localProgress = localProgressInstance;
			}

			var modelDTO = await ImportModel(
				fsSettings,
				localProgress);

			progress?.Report(0.5f);

			localProgress = null;

			if (progress != null)
			{
				var localProgressInstance = new Progress<float>();

				localProgressInstance.ProgressChanged += (sender, value) =>
				{
					progress.Report(value * 0.5f + 0.5f);
				};

				localProgress = localProgressInstance;
			}

			var result = await AddAssetAsResourceVariant(
				GetOrCreateResourceData(
					resourceID),
				new ResourceVariantDescriptor()
				{
					VariantID = MODEL_RAM_VARIANT_ID,
					VariantIDHash = MODEL_RAM_VARIANT_ID.AddressToHash(),
					Priority = MODEL_RAM_PRIORITY,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(ModelDTO),
				},
				ResourceManagementFactory.BuildPreallocatedResourceStorageHandle(
					modelDTO),
				true,
				localProgress);

			progress?.Report(1f);

			return result;
		}

		private unsafe async Task<ModelDTO> ImportModel(
			FileSystemSettings fsSettings,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			ModelDTO result = default;

			var scene = assimp.ImportFile(
				fsSettings.FullPath,
				(uint)PostProcessSteps.Triangulate);

			if (scene == null
				|| scene->MFlags == AssimpAPI.SceneFlagsIncomplete
				|| scene->MRootNode == null)
			{
				var error = assimp.GetErrorStringS();

				progress?.Report(1f);

				throw new Exception(error);
			}

			result.Name = scene->MName.AsString;

			result.RootNode = default;

			List<string> meshes = new List<string>();

			Dictionary<string, string> textures = new Dictionary<string, string>();

			Dictionary<uint, string> materials = new Dictionary<uint, string>();

			List<AssetImporter> assetImporters = new List<AssetImporter>();

			ImportMaterials(
				scene,
				ref result,
				textures,
				materials,
				assetImporters);

			ImportNode(
				scene->MRootNode,
				scene,
				fsSettings,
				ref result.RootNode,
				meshes,
				materials,
				assetImporters);

			result.Meshes = new string[meshes.Count];

			for (int i = 0; i < result.Meshes.Length; i++)
			{
				result.Meshes[i] = $"{resourceID}/{MODEL_MESHES_NESTED_RESOURCE_ID}/{meshes[i]}";
			}

			progress?.Report(1f);

			return result;
		}

		private unsafe void ImportMaterials(
			Scene* scene,
			ref ModelDTO modelDTO,
			Dictionary<string, string> textures,
			Dictionary<uint, string> materials,
			List<AssetImporter> assetImporters)
		{
			modelDTO.Materials = new string[scene->MNumMaterials];

			for (int i = 0; i < scene->MNumMaterials; i++)
			{
				Material* material = scene->MMaterials[i];

				var materialImporter = BuildMaterialAssetImporter(
					resourceID,
					material,
					(uint)i,
					textures,
					materials,
					assetImporters,
					out string materialResourceName,
					out string materialResourceID);

				//materialImporter.Import();
				assetImporters.Add(materialImporter);

				modelDTO.Materials[i] = materialResourceID;
			}

			modelDTO.Textures = new string[textures.Count];

			for (int i = 0; i < modelDTO.Textures.Length; i++)
			{
				modelDTO.Textures[i] = $"{resourceID}/{MODEL_TEXTURES_NESTED_RESOURCE_ID}/{textures.Values.ElementAt(i)}";
			}
		}

		#region Material and texture asset importers

		private unsafe MaterialRAMAssetImporter BuildMaterialAssetImporter(
			string resourceID,
			Material* material,
			uint materialIndex,
			Dictionary<string, string> textures,
			Dictionary<uint, string> materials,
			List<AssetImporter> assetImporters,
			out string materialResourceName,
			out string materialResourceID)
		{
			MaterialDTO materialDTO = default;

			AssimpString name;
			
			assimp.GetMaterialString(
				material,
				Assimp.MaterialNameBase,
				0,
				0,
				&name);

			List<string> materialTextures = new List<string>();

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
				textures,
				materialTextures,
				assetImporters);

			// 1. diffuse maps
			LoadMaterialTextures(
				material,
				TextureType.Diffuse,
				textures,
				materialTextures,
				assetImporters);

			// 2. specular maps
			LoadMaterialTextures(
				material,
				TextureType.Specular,
				textures,
				materialTextures,
				assetImporters);

			// 3. normal maps
			LoadMaterialTextures(
				material,
				TextureType.Height,
				textures,
				materialTextures,
				assetImporters);

			// 4. height maps
			LoadMaterialTextures(
				material,
				TextureType.Ambient,
				textures,
				materialTextures,
				assetImporters);


			string materialName = name.AsString;

			materialResourceName = materialName;

			int redundancyCounter = 0;

			while (materials.Values.Contains(materialResourceName))
			{
				redundancyCounter++;

				materialResourceName = $"{materialName} ({redundancyCounter})";
			}

			materials.Add(
				materialIndex,
				materialResourceName);

			materialResourceID = $"{resourceID}/{MODEL_MATERIALS_NESTED_RESOURCE_ID}/{materialResourceName}";


			materialDTO.Name = materialName;

			materialDTO.Shader = "Default shader"; //TODO: Implement shader selection

			materialDTO.Textures = new string[materialTextures.Count];

			for (int i = 0; i < materialDTO.Textures.Length; i++)
			{
				materialDTO.Textures[i] = $"{resourceID}/{MODEL_TEXTURES_NESTED_RESOURCE_ID}/{materialTextures[i]}";
			}

			return new MaterialRAMAssetImporter(
				resourceManager,
				materialResourceID,
				materialDTO);
		}

		private unsafe void LoadMaterialTextures(
			Material* mat,
			TextureType type,
			Dictionary<string, string> textures,
			List<string> materialTextures,
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

				Console.WriteLine(path.AsString);

				if (textures.ContainsKey(path))
				{
					var textureResourceName = textures[path];

					if (materialTextures.Contains(textureResourceName))
					{
						continue;
					}

					materialTextures.Add(textureResourceName);
				}
				else
				{
					var textureImporter = BuildTextureAssetImporter(
						resourceID,
						path.AsString,
						fsSettings,
						textures,
						out string textureResourceName,
						out string textureResourceID);

					//textureImporter.Import();
					assetImporters.Add(textureImporter);

					materialTextures.Add(textureResourceName);
				}
			}
		}

		private unsafe TextureRAMAssetImporter BuildTextureAssetImporter(
			string resourceID,
			string textureRelativePathFromSource,
			FileSystemSettings fsSettings,
			Dictionary<string, string> materialTextures,
			out string textureResourceName,
			out string textureResourceID)
		{
			var textureFsSettings = new FileSystemSettings();

			textureFsSettings.ApplicationDataFolder = fsSettings.ApplicationDataFolder;


			var assetDirectoryName = Path.GetDirectoryName(fsSettings.FullPath);


			var expectedTextureFullPath = Path.Combine(assetDirectoryName, textureRelativePathFromSource).Replace("\\", "/");

			var textureFileName = Path.GetFileName(expectedTextureFullPath);

			var textureFileNameWithoutExtension = Path.GetFileNameWithoutExtension(expectedTextureFullPath);


			var expectedTextureFullPathIfPlacedByGuidelines = Path.Combine(assetDirectoryName, "../Textures", textureFileName).Replace("\\", "/");

			Console.WriteLine($"Expected texture full path if placed by guidelines: {expectedTextureFullPathIfPlacedByGuidelines}");

			if (Path.Exists(expectedTextureFullPathIfPlacedByGuidelines))
			{
				var textureRelativePath = expectedTextureFullPathIfPlacedByGuidelines.Substring(fsSettings.ApplicationDataFolder.Length);

				textureFsSettings.RelativePath = textureRelativePath;
			}
			else
			{
				Console.WriteLine($"Expected texture full path if placed manually: {expectedTextureFullPath}");

				var textureRelativePath = expectedTextureFullPath.Substring(fsSettings.ApplicationDataFolder.Length);

				textureFsSettings.RelativePath = textureRelativePath;
			};

			Console.WriteLine($"Texture full path: {textureFsSettings.FullPath}");

			string textureName = textureFileNameWithoutExtension;

			textureResourceName = textureName;

			int redundancyCounter = 0;

			while (materialTextures.Values.Contains(textureResourceName))
			{
				redundancyCounter++;

				textureResourceName = $"{textureName} ({redundancyCounter})";
			}

			materialTextures.Add(
				textureFsSettings.FullPath,
				textureResourceName);

			textureResourceID = $"{resourceID}/{MODEL_TEXTURES_NESTED_RESOURCE_ID}/{textureResourceName}";

			var textureImporter = new TextureRAMAssetImporter(
				resourceManager,
				textureResourceID,
				textureFsSettings);

			return textureImporter;
		}

		#endregion

		private unsafe void ImportNode(
			Node* node,
			Scene* scene,
			FileSystemSettings fsSettings,
			ref ModelNodeDTO currentNodeDTO,
			List<string> meshes,
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


			currentNodeDTO.MeshesWithMaterials = new MeshWithMaterialDTO[node->MNumMeshes];

			for (var i = 0; i < node->MNumMeshes; i++)
			{
				currentNodeDTO.MeshesWithMaterials[i] = default;

				var mesh = scene->MMeshes[node->MMeshes[i]];

				ImportMesh(
					mesh,
					fsSettings,
					ref currentNodeDTO.MeshesWithMaterials[i],
					meshes,
					materials,
					assetImporters);
			}

			for (var i = 0; i < node->MNumChildren; i++)
			{
				currentNodeDTO.Children[i] = default;

				ImportNode(
					node->MChildren[i],
					scene,
					fsSettings,
					ref currentNodeDTO.Children[i],
					meshes,
					materials,
					assetImporters);
			}
		}

		private unsafe void ImportMesh(
			AssimpMesh* mesh,
			FileSystemSettings fsSettings,
			ref MeshWithMaterialDTO meshWithMaterialDTO,
			List<string> meshes,
			Dictionary<uint, string> materials,
			List<AssetImporter> assetImporters)
		{
			Console.WriteLine($"Importing mesh: {mesh->MName.AsString}");

			var meshImporter = BuildMeshAssetImporter(
				resourceID,
				mesh,
				meshes,
				out string meshResourceName,
				out string meshResourceID);

			//meshImporter.Import();
			assetImporters.Add(meshImporter);

			meshWithMaterialDTO.Mesh = meshResourceID;


			uint materialIndex = mesh->MMaterialIndex;

			meshWithMaterialDTO.Material = materials[materialIndex];
		}

		#region Mesh asset importer

		private unsafe MeshRAMAssetImporter BuildMeshAssetImporter(
			string resourceID,
			AssimpMesh* mesh,
			List<string> meshes,
			out string meshResourceName,
			out string meshResourceID)
		{
			List<Vertex> vertices = new List<Vertex>();

			List<uint> indices = new List<uint>();

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

			// now wak through each of the mesh's faces (a face is a mesh its triangle) and retrieve the corresponding vertex indices.
			for (uint i = 0; i < mesh->MNumFaces; i++)
			{
				Face face = mesh->MFaces[i];

				// retrieve all indices of the face and store them in the indices vector
				for (uint j = 0; j < face.MNumIndices; j++)
					indices.Add(face.MIndices[j]);
			}

			string meshName = mesh->MName.AsString;

			meshResourceName = meshName;

			int redundancyCounter = 0;

			while (meshes.Contains(meshResourceName))
			{
				redundancyCounter++;

				meshResourceName = $"{meshName} ({redundancyCounter})";
			}

			meshes.Add(meshResourceName);

			meshResourceID = $"{resourceID}/{MODEL_MESHES_NESTED_RESOURCE_ID}/{meshResourceName}";

			return new MeshRAMAssetImporter(
				resourceManager,
				meshResourceID,
				new Mesh
				{
					Vertices = BuildVertices(vertices),
					Indices = BuildIndices(indices)
				});
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
	}
}