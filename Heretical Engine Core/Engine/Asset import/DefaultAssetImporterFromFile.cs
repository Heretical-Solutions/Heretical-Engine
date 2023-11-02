#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.Logging;

using HereticalSolutions.Persistence;

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

namespace HereticalSolutions.HereticalEngine.AssetImport
{
	public abstract class DefaultAssetImporterFromFile<TAsset, TDTO>
		: AssetImporter
	{
		protected readonly string fullResourceID;

		protected readonly ISerializer serializer;

		protected readonly ISerializationArgument serializationArgument;

		protected readonly ILoadVisitorGeneric<TAsset, TDTO> visitor;

		public DefaultAssetImporterFromFile(
			IRuntimeResourceManager resourceManager,
			string fullResourceID,
			ISerializer serializer,
			ISerializationArgument serializationArgument,
			ILoadVisitorGeneric<TAsset, TDTO> visitor,
			IFormatLogger logger)
			: base(
				resourceManager,
				logger)
		{
			this.fullResourceID = fullResourceID;

			this.serializer = serializer;

			this.serializationArgument = serializationArgument;

			this.visitor = visitor;
		}

		public override async Task<IResourceVariantData> Import(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			serializer.Deserialize<TDTO>(
				serializationArgument,
				out var dto);

			visitor.Load(
				dto,
				out var asset);

			var result = await AddAssetAsResourceToManager(
				asset,
				true,
				progress)
				.ThrowExceptions<IResourceVariantData>(
					GetType(),
					logger);

			progress?.Report(1f);

			return result;
		}

		protected virtual async Task<IResourceVariantData> AddAssetAsResourceToManager(
			TAsset asset,
			bool allocate = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			var result = await AddAssetAsResourceVariant(
				await GetOrCreateResourceData(fullResourceID)
					.ThrowExceptions<IResourceData>(
						GetType(),
						logger),
				new ResourceVariantDescriptor()
				{
					VariantID = string.Empty,
					VariantIDHash = string.Empty.AddressToHash(),
					Priority = 0,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(TAsset)
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ResourceManagementFactory
					.BuildConcurrentPreallocatedResourceStorageHandle(asset),
#else
				ResourceManagementFactory
					.BuildPreallocatedResourceStorageHandle(asset),
#endif
				allocate,
				progress)
				.ThrowExceptions<IResourceVariantData>(
					GetType(),
					logger);

			progress?.Report(1f);

			return result;
		}
	}
}