using HereticalSolutions.Persistence;

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

namespace HereticalSolutions.HereticalEngine.Assimp
{
	public abstract class AssetImporterFromFile<TAsset, TDTO>
		: AssetImporter
	{
		protected readonly ISerializer serializer;

		protected readonly ISerializationArgument serializationArgument;

		protected readonly ILoadVisitorGeneric<TAsset, TDTO> visitor;

		public AssetImporterFromFile(
			IRuntimeResourceManager resourceManager,
			ISerializer serializer,
			ISerializationArgument serializationArgument,
			ILoadVisitorGeneric<TAsset, TDTO> visitor)
			: base(resourceManager)
		{
			this.serializer = serializer;

			this.serializationArgument = serializationArgument;

			this.visitor = visitor;
		}

		public override void Import()
		{
			serializer.Deserialize<TDTO>(
				serializationArgument,
				out var dto);

			visitor.Load(
				dto,
				out var asset);

			var resourceID = ResourceID;

			IResourceData resourceData = GetResourceData(resourceID);

			AddResource(
				asset,
				resourceData);
		}

		protected abstract string ResourceID { get; }

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

		protected virtual void AddResource(
			TAsset asset,
			IResourceData resourceData,
			IProgress<float> progress = null)
		{
			AddResourceAsDefault(
				asset,
				resourceData,
				progress);
		}

		protected void AddResourceAsDefault(
			TAsset asset,
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
						ResourceType = typeof(TAsset)
					},
					RuntimeResourceManagerFactory.BuildRuntimeResourceStorageHandle(
						asset));

			resourceData.AddVariant(
				variantData,
				progress);
		}
	}
}