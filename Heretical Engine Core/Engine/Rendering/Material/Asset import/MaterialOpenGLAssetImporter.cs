#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MaterialOpenGLAssetImporter : AssetImporter
	{
		public const string MATERIAL_OPENGL_VARIANT_ID = "OpenGL material";

		public const int MATERIAL_OPENGL_PRIORITY = 1;

		private readonly string resourceID;

		private readonly string materialRAMPath;

		private readonly string materialRAMVariantID;

		public MaterialOpenGLAssetImporter(
			string resourceID,
			string materialRAMPath,
			string materialRAMVariantID,
			ApplicationContext context)
			: base(
				context)
		{
			this.resourceID = resourceID;

			this.materialRAMPath = materialRAMPath;

			this.materialRAMVariantID = materialRAMVariantID;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<MaterialOpenGLAssetImporter>(
				$"IMPORTING {resourceID} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, MaterialOpenGLAssetImporter>(context.Logger),
				new ResourceVariantDescriptor()
				{
					VariantID = MATERIAL_OPENGL_VARIANT_ID,
					VariantIDHash = MATERIAL_OPENGL_VARIANT_ID.AddressToHash(),
					Priority = MATERIAL_OPENGL_PRIORITY,
					Source = EResourceSources.RUNTIME_GENERATED,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(MaterialOpenGL),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				MaterialFactory.BuildConcurrentMaterialOpenGLStorageHandle(
					materialRAMPath,
					materialRAMVariantID,
					context),
#else
				MaterialFactory.BuildMaterialOpenGLStorageHandle(
					materialRAMPath,
					materialRAMVariantID,
					context),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, MaterialOpenGLAssetImporter>(context.Logger);

			progress?.Report(1f);

			context.Logger?.Log<MaterialOpenGLAssetImporter>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}
	}
}