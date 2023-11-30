#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Application;

using Silk.NET.Assimp;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureDescriptorAssetImporter : AssetImporter
	{
		public const string TEXTURE_DESCRIPTOR_VARIANT_ID = "Descriptor DTO";

		public const int TEXTURE_DESCRIPTOR_PRIORITY = 0;

		private readonly string resourceID;

		private readonly string textureName;

		private readonly TextureType textureType;

		public TextureDescriptorAssetImporter(
			string resourceID,
			string textureName,
			TextureType textureType,
			ApplicationContext context)
			: base(
				context)
		{
			this.resourceID = resourceID;

			this.textureName = textureName;

			this.textureType = textureType;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			context.Logger?.Log<TextureDescriptorAssetImporter>(
				$"IMPORTING {resourceID} INITIATED");

			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(
					resourceID)
					.ThrowExceptions<IResourceData, TextureDescriptorAssetImporter>(context.Logger),
				new ResourceVariantDescriptor()
				{
					VariantID = TEXTURE_DESCRIPTOR_VARIANT_ID,
					VariantIDHash = TEXTURE_DESCRIPTOR_VARIANT_ID.AddressToHash(),
					Priority = TEXTURE_DESCRIPTOR_PRIORITY,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(TextureDescriptorDTO),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ResourceManagementFactory.BuildConcurrentPreallocatedResourceStorageHandle<TextureDescriptorDTO>(
					TextureFactory.BuildTextureDescriptorDTO(
						textureName,
						textureType),
					context),
#else
				ResourceManagementFactory.BuildPreallocatedResourceStorageHandle<TextureDescriptorDTO>(
					TextureFactory.BuildTextureDescriptorDTO(
						textureName,
						textureType),
					context),
#endif
				true,
				progress)
				.ThrowExceptions<IResourceVariantData, TextureDescriptorAssetImporter>(context.Logger);

			progress?.Report(1f);

			context.Logger?.Log<TextureDescriptorAssetImporter>(
				$"IMPORTING {resourceID} FINISHED");

			return result;
		}
	}
}