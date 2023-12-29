using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.HereticalEngine.Rendering;

using HereticalSolutions.ResourceManagement;

namespace HereticalSolutions.HereticalEngine.AssetImport
{
	#if FIXME

	public class CreateMaterialPrototypeDescriptorPostProcessor
		: AAssetImportPostProcessor
	{
		private readonly string shaderResourcePath;

		private readonly ApplicationContext context;

		public CreateMaterialPrototypeDescriptorPostProcessor(
			string shaderResourcePath,
			ApplicationContext context)
		{
			this.shaderResourcePath = shaderResourcePath;

			this.context = context;
		}

		public override async Task OnImport(
			IResourceVariantData variantData,
			IProgress<float> progress = null)
		{
			var materialAssetDescriptor = variantData.StorageHandle.GetResource<MaterialAssetDescriptor>();

			string materialAssetDescriptorPath = variantData.Resource.Descriptor.FullPath;

			string shaderName = shaderResourcePath.SplitAddressBySeparator().Last();

			string prototypeName = $"{materialAssetDescriptor.Name}_{shaderName}_Prototype";

			string materialPrototypeDescriptorPath = materialAssetDescriptorPath.ReplaceLast(prototypeName);

			var result = new MaterialPrototypeDescriptor
			{
				Name = prototypeName,

				ShaderResourcePath = shaderResourcePath,

				TextureResourcePaths = materialAssetDescriptor.TextureResourcePaths
			};

			var importMaterialPrototypeDescriptorTask = assetImportManager.Import<MaterialPrototypeDescriptorAssetImporter>(
				(importer) =>
				{
					importer.Initialize(
						materialPrototypeDescriptorPath,
						result);
				});

			await importMaterialPrototypeDescriptorTask;
		}
	}

	#endif
}