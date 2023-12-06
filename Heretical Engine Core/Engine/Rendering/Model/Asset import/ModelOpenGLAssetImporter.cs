#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

#define PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ModelOpenGLAssetImporter : AssetImporter
	{

		private readonly string resourcePath;

		private readonly string modelRAMPath;

		private readonly string modelRAMVariantID;
		

		public ModelOpenGLAssetImporter(
			string resourcePath,
			string modelRAMPath,
			string modelRAMVariantID,
			ApplicationContext context)
			: base(
				context)
		{
			this.resourcePath = resourcePath;

			this.modelRAMPath = modelRAMPath;

			this.modelRAMVariantID = modelRAMVariantID;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<ModelOpenGLAssetImporter>(
				$"IMPORTING {resourcePath} INITIATED");

			progress?.Report(0f);

			IReadOnlyResourceStorageHandle modelRAMStorageHandle = null;

			IProgress<float> localProgress = progress.CreateLocalProgress(
				0f,
				0.25f);

			modelRAMStorageHandle = await LoadDependency(
				modelRAMPath,
				modelRAMVariantID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, ModelOpenGLAssetImporter>(context.Logger);

			var modelDTO = modelRAMStorageHandle.GetResource<ModelAssetDescriptor>();

			progress?.Report(0.25f);

			List<AssetImporter> assetImporters = new List<AssetImporter>();

			for (int i = 0; i < modelDTO.TextureResourcePaths.Length; i++)
			{
				assetImporters.Add(
					BuildTextureAssetImporter(
						modelDTO.TextureResourcePaths[i]));
			}

			for (int i = 0; i < modelDTO.GeometryResourcePaths.Length; i++)
			{
				assetImporters.Add(
					BuildGeometryAssetImporter(
						modelDTO.GeometryResourcePaths[i],
						//"Diffuse shader", //"Default shader", //TODO: CHANGE. THIS IS ONLY TEMPORARY
						ShaderOpenGLAssetImporter.SHADER_OPENGL_VARIANT_ID));
			}

			for (int i = 0; i < modelDTO.MaterialResourcePaths.Length; i++)
			{
				assetImporters.Add(
					BuildMaterialAssetImporter(
						modelDTO.MaterialResourcePaths[i]));
			}

			for (int i = 0; i < modelDTO.MeshResourcePaths.Length; i++)
			{
				assetImporters.Add(
					BuildMeshAssetImporter(
						modelDTO.MeshResourcePaths[i]));
			}

			progress?.Report(0.5f);

#if PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES
			int totalAssetImporters = assetImporters.Count;

			List<Task> assetImportersTasks = new List<Task>();

			List<float> assetImportProgresses = new List<float>();

			foreach (var assetImporter in assetImporters)
			{
				IProgress<float> localImportProgress = progress.CreateLocalProgress(
					0.5f,
					0.75f,
					assetImportProgresses,
					totalAssetImporters);

				assetImportersTasks.Add(
					assetImporter
						.Import(
							localImportProgress));
						//.ThrowExceptions<IResourceVariantData, ModelOpenGLAssetImporter>(context.Logger));
			}

			await Task
				.WhenAll(assetImportersTasks)
				.ThrowExceptions<ModelOpenGLAssetImporter>(context.Logger);
#else
			int totalAssetImporters = assetImporters.Count;

			int current = 0;

			foreach (var assetImporter in assetImporters)
			{
				IProgress<float> localImportProgress = progress.CreateLocalProgress(
					0.5f,
					0.75f,
					current,
					totalAssetImporters);

				await assetImporter
					.Import(
						localImportProgress)
					.ThrowExceptions<IResourceVariantData, ModelOpenGLAssetImporter>(context.Logger);

				current++;
			}
#endif

			progress?.Report(0.75f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourcePath)
					.ThrowExceptions<IResourceData, ModelOpenGLAssetImporter>(context.Logger),
				new ResourceVariantDescriptor()
				{
					VariantID = MODEL_OPENGL_VARIANT_ID,
					VariantIDHash = MODEL_OPENGL_VARIANT_ID.AddressToHash(),
					Priority = MODEL_OPENGL_PRIORITY,
					Source = EResourceSources.RUNTIME_GENERATED,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(ModelOpenGL),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ModelFactory.BuildConcurrentModelOpenGLStorageHandle(
					modelRAMPath,
					modelRAMVariantID,
					context),
#else
				ModelFactory.BuildModelOpenGLStorageHandle(
					modelRAMPath,
					modelRAMVariantID,
					context),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, ModelOpenGLAssetImporter>(context.Logger);

			progress?.Report(1f);

			context.Logger?.Log<ModelOpenGLAssetImporter>(
				$"IMPORTING {resourcePath} FINISHED");

			return result;
		}

		private MeshOpenGLAssetImporter BuildMeshAssetImporter(
			string resourcePath)
		{
			var meshImporter = new MeshOpenGLAssetImporter(
				resourcePath,
				resourcePath,
				MeshAssetDescriptorAssetImporter.MESH_RAM_VARIANT_ID,
				context);

			return meshImporter;
		}

		private GeometryOpenGLAssetImporter BuildGeometryAssetImporter(
			string resourcePath,
			string shaderOpenGLPath,
			string shaderOpenGLVariantID)
		{
			var geometryImporter = new GeometryOpenGLAssetImporter(
				resourcePath,
				shaderOpenGLPath,
				shaderOpenGLVariantID,
				resourcePath,
				GeometryRAMAssetImporter.GEOMETRY_RAM_VARIANT_ID,
				context);

			return geometryImporter;
		}

		private MaterialOpenGLAssetImporter BuildMaterialAssetImporter(
			string resourcePath)
		{
			var materialImporter = new MaterialOpenGLAssetImporter(
				resourcePath,
				resourcePath,
				MaterialPrototypeDescriptorAssetImporter.MATERIAL_RAM_VARIANT_ID,
				context);

			return materialImporter;
		}

		private TextureOpenGLAssetImporter BuildTextureAssetImporter(
			string resourcePath)
		{
			var textureImporter = new TextureOpenGLAssetImporter(
				resourcePath,
				resourcePath,
				TextureRAMAssetImporter.TEXTURE_RAM_VARIANT_ID,
				resourcePath,
				TextureAssetDescriptorAssetImporter.TEXTURE_DESCRIPTOR_VARIANT_ID,
				context);

			return textureImporter;
		}
	}
}