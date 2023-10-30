using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.Persistence.IO;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using Silk.NET.OpenGL;

using Silk.NET.Assimp;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureRAMAssetImporter : AssetImporter
	{
		public const string TEXTURE_RAM_VARIANT_ID = "RAM texture";

		public const int TEXTURE_RAM_PRIORITY = 0;

		private readonly string resourceID;

		private readonly FilePathSettings filePathSettings;

		public TextureRAMAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			FilePathSettings filePathSettings)
			: base(resourceManager)
		{
			this.resourceID = resourceID;

			this.filePathSettings = filePathSettings;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID),
				new ResourceVariantDescriptor()
				{
					VariantID = TEXTURE_RAM_VARIANT_ID,
					VariantIDHash = TEXTURE_RAM_VARIANT_ID.AddressToHash(),
					Priority = TEXTURE_RAM_PRIORITY,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(Image<Rgba32>),
				},
				TextureFactory.BuildTextureRAMStorageHandle(
					filePathSettings),
				true,
				progress);

			progress?.Report(1f);

			return result;
		}
	}
}