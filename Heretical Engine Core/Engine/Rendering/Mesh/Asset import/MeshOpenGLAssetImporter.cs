#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MeshOpenGLAssetImporter : AAssetImporter
	{
		private string resourcePath;


		private string meshAssetDescriptorResourcePath;

		private string meshAssetDescriptorResourceVariantID;

		public MeshOpenGLAssetImporter(
			ApplicationContext context)
			: base(
				context)
		{
		}

		public void Initialize(
			string resourcePath,
			string meshAssetDescriptorResourcePath,
			string meshAssetDescriptorResourceVariantID)
		{
			this.resourcePath = resourcePath;


			this.meshAssetDescriptorResourcePath = meshAssetDescriptorResourcePath;

			this.meshAssetDescriptorResourceVariantID = meshAssetDescriptorResourceVariantID;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<MeshOpenGLAssetImporter>(
				$"IMPORTING {resourcePath} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourcePath)
					.ThrowExceptions<IResourceData, MeshOpenGLAssetImporter>(context.Logger),
				new ResourceVariantDescriptor()
				{
					VariantID = AssetImportConstants.ASSET_3D_MODEL_OPENGL_VARIANT_ID,
					VariantIDHash = AssetImportConstants.ASSET_3D_MODEL_OPENGL_VARIANT_ID.AddressToHash(),
					Priority = AssetImportConstants.NORMAL_PRIORIITY,
					Source = EResourceSources.RUNTIME_GENERATED,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(MeshOpenGL),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				MeshFactory.BuildConcurrentMeshOpenGLStorageHandle(
					meshAssetDescriptorResourcePath,
					meshAssetDescriptorResourceVariantID,
					context),
#else
				MeshFactory.BuildMeshOpenGLStorageHandle(
					meshAssetDescriptorResourcePath,
					meshAssetDescriptorResourceVariantID,
					context),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, MeshOpenGLAssetImporter>(context.Logger);

			progress?.Report(1f);

			context.Logger?.Log<MeshOpenGLAssetImporter>(
				$"IMPORTING {resourcePath} FINISHED");

			return result;
		}
	}
}