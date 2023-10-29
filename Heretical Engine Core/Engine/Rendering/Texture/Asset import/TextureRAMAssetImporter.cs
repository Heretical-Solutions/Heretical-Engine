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
		private const string TEXTURE_RAM_RESOURCE_ID = "RAM texture";

		private readonly string resourceID;

		private readonly string variantID;

		//Due to the fact that Silk.NET uses Image.Load<T>(path) instead of loading from byte array or some kind of DTO we are limited to using
		//one type of serialization. That's why I've put FileSystemSettings here directly instead of using ISerializationArgument
		private readonly FileSystemSettings fsSettings;

		public TextureRAMAssetImporter(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			string variantID,
			FileSystemSettings fsSettings)
			: base(resourceManager)
		{
			this.resourceID = resourceID;

			this.variantID = variantID;

			this.fsSettings = fsSettings;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				CreateNestedResourceData(
					resourceID,
					TEXTURE_RAM_RESOURCE_ID),
				new ResourceVariantDescriptor()
				{
					VariantID = variantID,
					VariantIDHash = variantID.AddressToHash(),
					Priority = 0,
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