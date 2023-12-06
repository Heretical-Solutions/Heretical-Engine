#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MaterialAssetDescriptorAssetImporter : AssetImporter
	{
		private readonly string resourcePath;

		private readonly MaterialAssetDescriptor descriptor;

		public MaterialAssetDescriptorAssetImporter(
			string resourcePath,
			MaterialAssetDescriptor descriptor,
			ApplicationContext context)
			: base(
				context)
		{
			this.resourcePath = resourcePath;

			this.descriptor = descriptor;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<MaterialAssetDescriptorAssetImporter>(
				$"IMPORTING {resourcePath} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourcePath)
					.ThrowExceptions<IResourceData, MaterialAssetDescriptorAssetImporter>(context.Logger),
				new ResourceVariantDescriptor()
				{
					VariantID = AssetImportConstants.ASSET_3D_MODEL_ASSET_DESCRIPTOR_VARIANT_ID,
					VariantIDHash = AssetImportConstants.ASSET_3D_MODEL_ASSET_DESCRIPTOR_VARIANT_ID.AddressToHash(),
					Priority = AssetImportConstants.DEFAULT_PRIORIITY,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(MaterialAssetDescriptor),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ResourceManagementFactory.BuildConcurrentPreallocatedResourceStorageHandle<MaterialAssetDescriptor>(
					descriptor,
					context),
#else
				ResourceManagementFactory.BuildPreallocatedResourceStorageHandle<MaterialAssetDescriptor>(
					descriptor,
					context),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, MaterialAssetDescriptorAssetImporter>(context.Logger);

			progress?.Report(1f);

			context.Logger?.Log<MaterialAssetDescriptorAssetImporter>(
				$"IMPORTING {resourcePath} FINISHED");

			return result;
		}
	}
}