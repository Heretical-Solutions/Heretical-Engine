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
		private const string TEXTURE_RAM_VARIANT_ID = "RAM texture";

		private const int TEXTURE_RAM_PRIORITY = 0;

		private readonly string resourceID;

		//Due to the fact that Silk.NET uses Image.Load<T>(path) instead of loading from byte array or some kind of DTO we are limited to using
		//one type of serialization. That's why I've put FileSystemSettings here directly instead of using ISerializationArgument
		private readonly FileSystemSettings fsSettings;

		public TextureRAMAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			FileSystemSettings fsSettings)
			: base(resourceManager)
		{
			this.resourceID = resourceID;

			this.fsSettings = fsSettings;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				GetOrCreateResourceData(
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
					fsSettings),
				true,
				progress);

			progress?.Report(1f);

			return result;
		}
	}
}