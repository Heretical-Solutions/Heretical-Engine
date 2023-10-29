// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Silk.NET.Maths;

using Silk.NET.Assimp;

using Silk.NET.OpenGL;

using System.Numerics;

using AssimpMesh = Silk.NET.Assimp.Mesh;

using AssimpAPI = Silk.NET.Assimp.Assimp;

using HereticalSolutions.Persistence.IO;

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.Math;

using HereticalSolutions.HereticalEngine.AssetImport;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ModelAssetImporter
		: AssetImporter,
		IDisposable
	{
		public ModelAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			FileSystemSettings fsSettings,
			GL gl,
			bool gamma = false)
			: base(
				resourceManager)
		{
			this.resourceID = resourceID;

			this.fsSettings = fsSettings;

			this.gl = gl;

			assimp = AssimpAPI.GetApi();
		}

		private string resourceID;

		//Due to the fact that Silk.NET uses Image.Load<T>(path) instead of loading from byte array or some kind of DTO we are limited to using
		//one type of serialization. That's why I've put FileSystemSettings here directly instead of using ISerializationArgument
		private FileSystemSettings fsSettings;

		//And for the same reason gl is not fed into a visitor but is used directly here
		private readonly GL gl;

		private AssimpAPI assimp;

		private Dictionary<string, TextureOpenGL> texturesLoaded = new Dictionary<string, TextureOpenGL>();

		private List<Mesh> meshes = new List<Mesh>();

		public unsafe override object Import()
		{
			var scene = assimp.ImportFile(
				fsSettings.FullPath,
				(uint)PostProcessSteps.Triangulate);

			if (scene == null
				|| scene->MFlags == AssimpAPI.SceneFlagsIncomplete
				|| scene->MRootNode == null)
			{
				var error = assimp.GetErrorStringS();

				throw new Exception(error);
			}

			IResourceData resourceData = GetResourceData(resourceID);

			Console.WriteLine($"Scene materials count: {scene->MNumMaterials}");

			ProcessNode(
				scene->MRootNode,
				scene,
				resourceData);

			return null;
		}

		//TODO: this is a third class (after AssetImporterFromFile and ShaderAssimp) that has this method and the methods below. Extract?
		private IResourceData GetResourceData(
			string resourceID)
		{
			IResourceData resourceData = null;

			if (resourceManager.HasRootResource(resourceID))
			{
				resourceData = (IResourceData)resourceManager.GetRootResource(resourceID);
			}
			else
			{
				resourceData = RuntimeResourceManagerFactory.BuildResourceData(
					new ResourceDescriptor()
					{
						ID = resourceID,
						IDHash = resourceID.AddressToHash()
					});

				resourceManager.AddRootResource((IReadOnlyResourceData)resourceData);
			}

			return resourceData;
		}

		private void AddResourceAsVariant<TValue>(
			string variantID,
			TValue asset,
			IResourceData resourceData,
			IProgress<float> progress = null)
		{
			var variantData = RuntimeResourceManagerFactory.BuildResourceVariantData(
				new ResourceVariantDescriptor()
				{
					VariantID = variantID,
					VariantIDHash = variantID.AddressToHash(),
					Priority = 0,
					Source = EResourceSources.LOCAL_STORAGE,
					ResourceType = typeof(TValue)
				},
				RuntimeResourceManagerFactory.BuildPreallocatedRuntimeResourceStorageHandle(
					asset));

			resourceData.AddVariant(
				variantData,
				progress);
		}

		private unsafe void ProcessNode(
			Node* node,
			Scene* scene,
			IResourceData resourceData)
		{
			for (var i = 0; i < node->MNumMeshes; i++)
			{
				var mesh = scene->MMeshes[node->MMeshes[i]];

				meshes.Add(
					ProcessMesh(
						mesh,
						scene,
						resourceData));
			}

			for (var i = 0; i < node->MNumChildren; i++)
			{
				ProcessNode(
					node->MChildren[i],
					scene,
					resourceData);
			}
		}

		private unsafe Mesh ProcessMesh(
			AssimpMesh* mesh,
			Scene* scene,
			IResourceData resourceData)
		{
			Console.WriteLine($"Importing mesh: {mesh->MName.AsString}");

			// data to fill
			List<Vertex> vertices = new List<Vertex>();

			List<uint> indices = new List<uint>();

			List<TextureOpenGL> textures = new List<TextureOpenGL>();

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

			Console.WriteLine($"Vertex count: {vertices.Count}");

			Console.WriteLine($"Index count: {indices.Count}");

			Console.WriteLine($"Material index: {mesh->MMaterialIndex}");

			// process materials
			Material* material = scene->MMaterials[mesh->MMaterialIndex];
			// we assume a convention for sampler names in the shaders. Each diffuse texture should be named
			// as 'texture_diffuseN' where N is a sequential number ranging from 1 to MAX_SAMPLER_NUMBER. 
			// Same applies to other texture as the following list summarizes:
			// diffuse: texture_diffuseN
			// specular: texture_specularN
			// normal: texture_normalN

			Console.WriteLine($"Material's property count: {material->MNumProperties}");

			Console.WriteLine($"Material's allocated count: {material->MNumAllocated}");

			//0. base color
			Console.WriteLine($"Base color");

			var baseColorMaps = LoadMaterialTextures(material, TextureType.BaseColor, "UNUSED");

			if (baseColorMaps.Any())
				textures.AddRange(baseColorMaps);

			// 1. diffuse maps
			Console.WriteLine($"Diffuse maps");

			var diffuseMaps = LoadMaterialTextures(material, TextureType.Diffuse, "texture_diffuse");
			
			if (diffuseMaps.Any())
				textures.AddRange(diffuseMaps);

			// 2. specular maps
			Console.WriteLine($"Specular maps");
			var specularMaps = LoadMaterialTextures(material, TextureType.Specular, "texture_specular");

			if (specularMaps.Any())
				textures.AddRange(specularMaps);

			// 3. normal maps
			Console.WriteLine($"Normal maps");
			var normalMaps = LoadMaterialTextures(material, TextureType.Height, "texture_normal");

			if (normalMaps.Any())
				textures.AddRange(normalMaps);

			// 4. height maps
			Console.WriteLine($"Height maps");
			var heightMaps = LoadMaterialTextures(material, TextureType.Ambient, "texture_height");

			if (heightMaps.Any())
				textures.AddRange(heightMaps);

			// return a mesh object created from the extracted mesh data
			var result = new Mesh(
				gl,
				BuildVertices(vertices),
				BuildIndices(indices),
				textures);

			AddResourceAsVariant<Mesh>(
				$"Mesh/{mesh->MName.AsString}",
				result,
				resourceData);

			return result;
		}

		private unsafe List<TextureOpenGL> LoadMaterialTextures(
			Material* mat,
			TextureType type,
			string typeName)
		{
			var textureCount = assimp.GetMaterialTextureCount(
				mat,
				type);

			Console.WriteLine($"Texture count: {textureCount}");

			List<TextureOpenGL> textures = new List<TextureOpenGL>();

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

				if (texturesLoaded.ContainsKey(path))
				{
					textures.Add(texturesLoaded[path]);
				}
				else
				{
					var textureFsSettings = new FileSystemSettings();

					textureFsSettings.ApplicationDataFolder = fsSettings.ApplicationDataFolder;


					var assetDirectoryName = Path.GetDirectoryName(fsSettings.FullPath);


					var expectedTextureFullPathIfPlacedManually = Path.Combine(assetDirectoryName, path.AsString).Replace("\\", "/");

					var textureFileName = Path.GetFileName(expectedTextureFullPathIfPlacedManually);

					var textureFileNameWithoutExtension = Path.GetFileNameWithoutExtension(expectedTextureFullPathIfPlacedManually);


					var expectedTextureFullPathIfPlacedByEngine = Path.Combine(assetDirectoryName, "../Textures", textureFileName).Replace("\\", "/");

					Console.WriteLine($"Expected texture full path if placed by engine: {expectedTextureFullPathIfPlacedByEngine}");

					if (Path.Exists(expectedTextureFullPathIfPlacedByEngine))
					{
						var textureRelativePath = expectedTextureFullPathIfPlacedByEngine.Substring(fsSettings.ApplicationDataFolder.Length);

						textureFsSettings.RelativePath = textureRelativePath;
					}
					else
					{
						Console.WriteLine($"Expected texture full path if placed manually: {expectedTextureFullPathIfPlacedManually}");

						var textureRelativePath = expectedTextureFullPathIfPlacedManually.Substring(fsSettings.ApplicationDataFolder.Length);

						textureFsSettings.RelativePath = textureRelativePath;
					};

					Console.WriteLine($"Texture full path: {textureFsSettings.FullPath}");

					var variantID = $"Texture/{textureFileNameWithoutExtension}";

					Console.WriteLine($"Variant ID: {variantID}");

					var textureAssimp = new TextureRAMAssetImporter(
						resourceManager,
						resourceID,
						variantID,
						textureFsSettings,
						gl,
						type);

					var texture = (TextureOpenGL)textureAssimp.Import();

					textures.Add(texture);

					texturesLoaded.Add(path, texture);
				}
			}

			return textures;
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

		public void Dispose()
		{
			foreach (var mesh in meshes)
			{
				mesh.Dispose();
			}

			texturesLoaded.Clear();

			texturesLoaded = null;
		}
	}
}