using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.HereticalEngine.Assimp;

using HereticalSolutions.Persistence.IO;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureAssimp : AssetImporter
	{
		private string resourceID;

		//Due to the fact that Silk.NET uses Image.Load<T>(path) instead of loading from byte array or some kind of DTO we are limited to using
		//one type of serialization. That's why I've put FileSystemSettings here directly instead of using ISerializationArgument
		private FileSystemSettings fsSettings;

		//And for the same reason gl is not fed into a visitor but is used directly here
		private GL gl;

		public TextureAssimp(
			IRuntimeResourceManager resourceManager,
			string resourceID,
			FileSystemSettings fsSettings,
			GL gl)
			: base(
				resourceManager)
		{
			this.resourceID = resourceID;

			this.fsSettings = fsSettings;

			this.gl = gl;
		}

		public override void Import()
		{
			var image = Image.Load<Rgba32>(fsSettings.FullPath);

			var asset = new Texture(
				gl,
				image);

			//In the tutorial image was wrapped in using statement. Calling Dispose here because it's fed directly into Texture constructor and therefore
			//it's not disposed there as it would if path was fed instead
			image.Dispose();

			IResourceData resourceData = GetResourceData(resourceID);

			AddResource(
				asset,
				resourceData);
		}

		//TODO: this is a third class (after AssetImporterFromFile and ShaderAssimp) that has this method and the methods below. Extract?
		protected IResourceData GetResourceData(
			string resourceID)
		{
			IResourceData resourceData = null;

			if (resourceManager.HasResource(resourceID))
			{
				resourceData = (IResourceData)resourceManager.GetResource(resourceID);
			}
			else
			{
				resourceData = RuntimeResourceManagerFactory.BuildResourceData(
					new ResourceDescriptor()
					{
						ID = resourceID,
						IDHash = resourceID.AddressToHash()
					});

				resourceManager.AddResource((IReadOnlyResourceData)resourceData);
			}

			return resourceData;
		}

		private void AddResource(
			Texture asset,
			IResourceData resourceData,
			IProgress<float> progress = null)
		{
			AddResourceAsDefault(
				asset,
				resourceData,
				progress);
		}

		private void AddResourceAsDefault(
			Texture asset,
			IResourceData resourceData,
			IProgress<float> progress = null)
		{
			var variantData = RuntimeResourceManagerFactory.BuildResourceVariantData(
					new ResourceVariantDescriptor()
					{
						VariantID = string.Empty,
						VariantIDHash = string.Empty.AddressToHash(),
						Priority = 0,
						Source = EResourceSources.LOCAL_STORAGE,
						ResourceType = typeof(Texture)
					},
					RuntimeResourceManagerFactory.BuildRuntimeResourceStorageHandle(
						asset));

			resourceData.AddVariant(
				variantData,
				progress);
		}
	}
}