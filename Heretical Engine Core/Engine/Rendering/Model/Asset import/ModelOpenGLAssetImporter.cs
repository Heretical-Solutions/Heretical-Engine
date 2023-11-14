#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

//DO NOT DO THIS FOR NOW. SOME ASSET IMPORTERS ARE BLINDLY LOOKING FOR
//RESOURCES BY GIVEN ID EXPECTING THEM TO BE PRESENT IN THE RESOURCE MANAGER. PARALLELIZING THE LOAD PROCESS MAY CAUSE RACE
//CONDITIONS
//TODO: UPDATE CERTAIN RESOURCE IMPORTERS AND STORAGE HANDLES TO WAIT UNTIL RESOURCE BECOMES AVAILABLE
//#define PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ModelOpenGLAssetImporter : AssetImporter
	{
		public const string MODEL_OPENGL_VARIANT_ID = "OpenGL model";

		public const int MODEL_OPENGL_PRIORITY = 1;


		private readonly string resourceID;

		private readonly IReadOnlyResourceStorageHandle modelRAMStorageHandle;

		private readonly GL cachedGL;

		private readonly ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer;

		public ModelOpenGLAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			IReadOnlyResourceStorageHandle modelRAMStorageHandle,
			GL gl,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
			: base(
				resourceManager,
				logger)
		{
			this.resourceID = resourceID;

			this.modelRAMStorageHandle = modelRAMStorageHandle;

			cachedGL = gl;

			this.mainThreadCommandBuffer = mainThreadCommandBuffer;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			logger?.Log<ModelOpenGLAssetImporter>(
				$"IMPORTING {resourceID} INITIATED");

			progress?.Report(0f);

			if (!modelRAMStorageHandle.Allocated)
			{
				IProgress<float> localProgress = progress.CreateLocalProgress(
					0f,
					0.25f);

				await modelRAMStorageHandle
					.Allocate(
						localProgress)
					.ThrowExceptions<ModelOpenGLStorageHandle>(logger);
			}

			var modelDTO = modelRAMStorageHandle.GetResource<ModelDTO>();

			progress?.Report(0.25f);

			List<AssetImporter> assetImporters = new List<AssetImporter>();

			for (int i = 0; i < modelDTO.MeshResourceIDs.Length; i++)
			{
				assetImporters.Add(
					BuildMeshAssetImporter(
						resourceManager,
						modelDTO.MeshResourceIDs[i]));
			}

			for (int i = 0; i < modelDTO.GeometryResourceIDs.Length; i++)
			{
				assetImporters.Add(
					BuildGeometryAssetImporter(
						resourceManager,
						modelDTO.GeometryResourceIDs[i],
						cachedGL));
			}

			for (int i = 0; i < modelDTO.MaterialResourceIDs.Length; i++)
			{
				assetImporters.Add(
					BuildMaterialAssetImporter(
						resourceManager,
						modelDTO.MaterialResourceIDs[i]));
			}

			for (int i = 0; i < modelDTO.TextureResourceIDs.Length; i++)
			{
				assetImporters.Add(
					BuildTextureAssetImporter(
						resourceManager,
						modelDTO.TextureResourceIDs[i],
						cachedGL));
			}

			assetImporters.Reverse();

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
					0.5f,
					0.75f,
					current,
					totalAssetImporters);

				await assetImporter
					.Import(
						localImportProgress)
					.ThrowExceptions<IResourceVariantData, ModelRAMAssetImporter>(logger);

				current++;
			}
#endif

			progress?.Report(0.75f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, ModelOpenGLAssetImporter>(logger),
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
					resourceManager,
					modelRAMStorageHandle,
					logger),
#else
				ModelFactory.BuildModelOpenGLStorageHandle(
					resourceManager,
					textureRAMStorageHandle,
					logger),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, ModelOpenGLAssetImporter>(logger);

			progress?.Report(1f);

			logger?.Log<ModelOpenGLAssetImporter>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}

		private MeshOpenGLAssetImporter BuildMeshAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID)
		{
			var resourceStorageHandle = resourceManager
				.GetResource(
					resourceID.SplitAddressBySeparator())
				.GetVariant(
					MeshRAMAssetImporter.MESH_RAM_VARIANT_ID)
				.StorageHandle;

			var meshImporter = new MeshOpenGLAssetImporter(
				resourceManager,
				resourceID,
				resourceStorageHandle,
				logger);

			return meshImporter;
		}

		private GeometryOpenGLAssetImporter BuildGeometryAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			GL gl)
		{
			var resourceStorageHandle = resourceManager
				.GetResource(
					resourceID.SplitAddressBySeparator())
				.GetVariant(
					GeometryRAMAssetImporter.GEOMETRY_RAM_VARIANT_ID)
				.StorageHandle;

			var geometryImporter = new GeometryOpenGLAssetImporter(
				resourceManager,
				resourceID,
				resourceStorageHandle,
				gl,
				mainThreadCommandBuffer,
				logger);

			return geometryImporter;
		}

		private MaterialOpenGLAssetImporter BuildMaterialAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID)
		{
			var resourceStorageHandle = resourceManager
				.GetResource(
					resourceID.SplitAddressBySeparator())
				.GetVariant(
					MaterialRAMAssetImporter.MATERIAL_RAM_VARIANT_ID)
				.StorageHandle;

			var materialImporter = new MaterialOpenGLAssetImporter(
				resourceManager,
				resourceID,
				resourceStorageHandle,
				logger);

			return materialImporter;
		}

		private TextureOpenGLAssetImporter BuildTextureAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			GL gl)
		{
			var resourceStorageHandle = resourceManager
				.GetResource(
					resourceID.SplitAddressBySeparator())
				.GetVariant(
					TextureRAMAssetImporter.TEXTURE_RAM_VARIANT_ID)
				.StorageHandle;

			var textureImporter = new TextureOpenGLAssetImporter(
				resourceManager,
				resourceID,
				resourceStorageHandle,
				gl,
				mainThreadCommandBuffer,
				logger);

			return textureImporter;
		}
	}
}