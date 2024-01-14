/*
#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MaterialOpenGLAssetImporter : AAssetImporter
	{
		private string resourcePath;


		private string materialPrototypeDescriptorResourcePath;

		private string materialPrototypeDescriptorResourceVariantID;

		public MaterialOpenGLAssetImporter(
			ApplicationContext context)
			: base(
				context)
		{
		}

		public void Initialize(
			string resourcePath,
			string materialPrototypeDescriptorResourcePath,
			string materialPrototypeDescriptorResourceVariantID)
		{
			this.resourcePath = resourcePath;


			this.materialPrototypeDescriptorResourcePath = materialPrototypeDescriptorResourcePath;

			this.materialPrototypeDescriptorResourceVariantID = materialPrototypeDescriptorResourceVariantID;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<MaterialOpenGLAssetImporter>(
				$"IMPORTING {resourcePath} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourcePath)
					.ThrowExceptions<IResourceData, MaterialOpenGLAssetImporter>(context.Logger),
				new ResourceVariantDescriptor()
				{
					VariantID = AssetImportConstants.ASSET_3D_MODEL_OPENGL_VARIANT_ID,
					VariantIDHash = AssetImportConstants.ASSET_3D_MODEL_OPENGL_VARIANT_ID.AddressToHash(),
					Priority = AssetImportConstants.NORMAL_PRIORIITY,
					Source = EResourceSources.RUNTIME_GENERATED,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(MaterialOpenGL),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				MaterialFactory.BuildConcurrentMaterialOpenGLStorageHandle(
					materialPrototypeDescriptorResourcePath,
					materialPrototypeDescriptorResourceVariantID,
					context),
#else
				MaterialFactory.BuildMaterialOpenGLStorageHandle(
					materialPrototypeDescriptorResourcePath,
					materialPrototypeDescriptorResourceVariantID,
					context),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, MaterialOpenGLAssetImporter>(context.Logger);

			progress?.Report(1f);

			context.Logger?.Log<MaterialOpenGLAssetImporter>(
				$"IMPORTING {resourcePath} FINISHED");

			return result;
		}

		public override void Cleanup()
		{
			resourcePath = null;


			materialPrototypeDescriptorResourcePath = null;

			materialPrototypeDescriptorResourceVariantID = null;
		}
	}
}
*/