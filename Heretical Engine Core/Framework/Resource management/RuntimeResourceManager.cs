using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents a runtime resource manager.
    /// </summary>
    public class RuntimeResourceManager
        : IRuntimeResourceManager,
          IContainsDependencyResources
    {
        private readonly IRepository<int, string> rootResourceIDHashToID;

        private readonly IRepository<int, IReadOnlyResourceData> rootResourcesRepository;

        private readonly ILogger logger;

        public RuntimeResourceManager(
            IRepository<int, string> rootResourceIDHashToID,
            IRepository<int, IReadOnlyResourceData> rootResourcesRepository,
            ILogger logger = null)
        {
            this.rootResourceIDHashToID = rootResourceIDHashToID;

            this.rootResourcesRepository = rootResourcesRepository;

            this.logger = logger;
        }

        #region IRuntimeResourceManager

        #region IReadOnlyRuntimeResourceManager

        #region Has

        public bool HasRootResource(int rootResourceIDHash)
        {
            return rootResourcesRepository.Has(rootResourceIDHash);
        }

        public bool HasRootResource(string rootResourceID)
        {
            return HasRootResource(rootResourceID.AddressToHash());
        }

        public bool HasResource(int[] resourcePathPartHashes)
        {
            if (!rootResourcesRepository.TryGet(
                resourcePathPartHashes[0],
                out var currentData))
                return false;

            return GetNestedResourceRecursive(
                ref currentData,
                resourcePathPartHashes);
        }

        public bool HasResource(string[] resourcePathParts)
        {
            if (!rootResourcesRepository.TryGet(
                resourcePathParts[0].AddressToHash(),
                out var currentData))
                return false;

            return GetNestedResourceRecursive(
                ref currentData,
                resourcePathParts);
        }

        #endregion

        #region Get

        public IReadOnlyResourceData GetRootResource(int rootResourceIDHash)
        {
            if (!rootResourcesRepository.TryGet(
                rootResourceIDHash,
                out var resource))
                return null;

            return resource;
        }

        public IReadOnlyResourceData GetRootResource(string rootResourceID)
        {
            return GetRootResource(rootResourceID.AddressToHash());
        }

        public IReadOnlyResourceData GetResource(int[] resourcePathPartHashes)
        {
            if (!rootResourcesRepository.TryGet(
                resourcePathPartHashes[0],
                out var currentResource))
                return null;

            if (!GetNestedResourceRecursive(
                ref currentResource,
                resourcePathPartHashes))
                return null;

            return currentResource;
        }

        public IReadOnlyResourceData GetResource(string[] resourcePathParts)
        {
            if (!rootResourcesRepository.TryGet(
                resourcePathParts[0].AddressToHash(),
                out var currentResource))
                return null;

            if (!GetNestedResourceRecursive(
                ref currentResource,
                resourcePathParts))
                return null;

            return currentResource;
        }

        #endregion

        #region Try get

        public bool TryGetRootResource(
            int rootResourceIDHash,
            out IReadOnlyResourceData resource)
        {
            return rootResourcesRepository.TryGet(
                rootResourceIDHash,
                out resource);
        }

        public bool TryGetRootResource(
            string rootResourceID,
            out IReadOnlyResourceData resource)
        {
            return TryGetRootResource(
                rootResourceID.AddressToHash(),
                out resource);
        }

        public bool TryGetResource(
            int[] resourcePathPartHashes,
            out IReadOnlyResourceData resource)
        {
            if (!rootResourcesRepository.TryGet(
                resourcePathPartHashes[0],
                out resource))
                return false;

            return GetNestedResourceRecursive(
                ref resource,
                resourcePathPartHashes);
        }

        public bool TryGetResource(
            string[] resourcePathParts,
            out IReadOnlyResourceData resource)
        {
            if (!rootResourcesRepository.TryGet(
                resourcePathParts[0].AddressToHash(),
                out resource))
                return false;

            return GetNestedResourceRecursive(
                ref resource,
                resourcePathParts);
        }

        #endregion

        #region Get default

        public IResourceVariantData GetDefaultRootResource(int rootResourceIDHash)
        {
            if (!rootResourcesRepository.TryGet(
                rootResourceIDHash,
                out var resource))
                return null;

            return resource.DefaultVariant;
        }

        public IResourceVariantData GetDefaultRootResource(string rootResourceID)
        {
            return GetDefaultRootResource(rootResourceID.AddressToHash());
        }

        public IResourceVariantData GetDefaultResource(int[] resourcePathPartHashes)
        {
            if (!rootResourcesRepository.TryGet(
                resourcePathPartHashes[0],
                out var currentResource))
                return null;

            if (!GetNestedResourceRecursive(
                ref currentResource,
                resourcePathPartHashes))
                return null;

            return currentResource.DefaultVariant;
        }

        public IResourceVariantData GetDefaultResource(string[] resourcePathParts)
        {
            if (!rootResourcesRepository.TryGet(
                resourcePathParts[0].AddressToHash(),
                out var currentResource))
                return null;

            if (!GetNestedResourceRecursive(
                ref currentResource,
                resourcePathParts))
                return null;

            return currentResource.DefaultVariant;
        }

        #endregion

        #region Try get default

        public bool TryGetDefaultRootResource(
            int rootResourceIDHash,
            out IResourceVariantData resource)
        {
            if (!rootResourcesRepository.TryGet(
                rootResourceIDHash,
                out var rootResource))
            {
                resource = null;

                return false;
            }

            resource = rootResource.DefaultVariant;

            return true;
        }

        public bool TryGetDefaultRootResource(
            string rootResourceID,
            out IResourceVariantData resource)
        {
            return TryGetDefaultRootResource(
                rootResourceID.AddressToHash(),
                out resource);
        }

        public bool TryGetDefaultResource(
            int[] resourcePathPartHashes,
            out IResourceVariantData resource)
        {
            if (!rootResourcesRepository.TryGet(
                resourcePathPartHashes[0],
                out var currentResource))
            {
                resource = null;

                return false;
            }

            if (!GetNestedResourceRecursive(
                ref currentResource,
                resourcePathPartHashes))
            {
                resource = null;

                return false;
            }

            resource = currentResource.DefaultVariant;

            return true;
        }

        public bool TryGetDefaultResource(
            string[] resourcePathParts,
            out IResourceVariantData resource)
        {
            if (!rootResourcesRepository.TryGet(
                resourcePathParts[0].AddressToHash(),
                out var currentResource))
            {
                resource = null;

                return false;
            }

            if (!GetNestedResourceRecursive(
                ref currentResource,
                resourcePathParts))
            {
                resource = null;

                return false;
            }

            resource = currentResource.DefaultVariant;

            return true;
        }

        #endregion

        #region All's

        public IEnumerable<int> RootResourceIDHashes { get => rootResourcesRepository.Keys; }

        public IEnumerable<string> RootResourceIDs { get => rootResourceIDHashToID.Values; }

        public IEnumerable<IReadOnlyResourceData> AllRootResources { get => rootResourcesRepository.Values; }

        #endregion

        #endregion

        public async Task AddRootResource(
            IReadOnlyResourceData rootResource,
            IProgress<float> progress = null)
        {
            progress?.Report(0f);

            if (!rootResourcesRepository.TryAdd(
                rootResource.Descriptor.IDHash,
                rootResource))
            {
                progress?.Report(1f);

                return;
            }

            ((IResourceData)rootResource).ParentResource = null;

            rootResourceIDHashToID.AddOrUpdate(
                rootResource.Descriptor.IDHash,
                rootResource.Descriptor.ID);

            progress?.Report(1f);
        }

        public async Task RemoveRootResource(
            int rootResourceIDHash = -1,
            bool free = true,
            IProgress<float> progress = null)
        {
            progress?.Report(0f);

            if (!rootResourcesRepository.TryGet(
                rootResourceIDHash,
                out var resource))
            {
                progress?.Report(1f);

                return;
            }

            rootResourcesRepository.TryRemove(rootResourceIDHash);

            rootResourceIDHashToID.TryRemove(rootResourceIDHash);

            if (free)
                await ((IResourceData)resource)
                    .Clear(
                        free,
                        progress)
                    .ThrowExceptions<RuntimeResourceManager>(logger);

            progress?.Report(1f);
        }

        public async Task RemoveRootResource(
            string rootResourceID,
            bool free = true,
            IProgress<float> progress = null)
        {
            await RemoveRootResource(
                rootResourceID.AddressToHash(),
                free,
                progress)
                .ThrowExceptions<RuntimeResourceManager>(logger);
        }

        public async Task ClearAllRootResources(
            bool free = true,
            IProgress<float> progress = null)
        {
            progress?.Report(0f);

            int totalRootResourcesCount = rootResourcesRepository.Count;

            int counter = 0;

            foreach (var key in rootResourcesRepository.Keys)
            {
                if (rootResourcesRepository.TryGet(
                    key,
                    out var rootResource))
                {
                    IProgress<float> localProgress = progress.CreateLocalProgress(
                        0f,
                        1f,
                        counter,
                        totalRootResourcesCount);

                    await ((IResourceData)rootResource)
                        .Clear(
                            free,
                            localProgress)
                        .ThrowExceptions<RuntimeResourceManager>(logger);
                }

                counter++;

                progress?.Report((float)counter / (float)totalRootResourcesCount);

            }

            rootResourceIDHashToID.Clear();

            rootResourcesRepository.Clear();

            progress?.Report(1f);
        }

        #endregion

        #region IContainsDependencyResources

        public async Task<IReadOnlyResourceStorageHandle> LoadDependency(
            string path,
            string variantID = null,
            IProgress<float> progress = null)
        {
            IReadOnlyResourceData dependencyResource = await GetDependencyResource(path)
                .ThrowExceptions<IReadOnlyResourceData, RuntimeResourceManager>(
                    logger);

            IResourceVariantData dependencyVariantData = await ((IContainsDependencyResourceVariants)dependencyResource)
                .GetDependencyResourceVariant(variantID)
                .ThrowExceptions<IResourceVariantData, RuntimeResourceManager>(
                    logger);

            progress?.Report(0.5f);

            var dependencyStorageHandle = dependencyVariantData.StorageHandle;

            if (!dependencyStorageHandle.Allocated)
            {
                IProgress<float> localProgress = progress.CreateLocalProgress(
                    0.5f,
                    1f);

                await dependencyStorageHandle
                    .Allocate(
                        localProgress)
                    .ThrowExceptions<RuntimeResourceManager>(
                        logger);
            }

            progress?.Report(1f);

            return dependencyStorageHandle;
        }

        public async Task<IReadOnlyResourceData> GetDependencyResource(
            string path)
        {
            IReadOnlyResourceData dependencyResource = GetResource(
                path.SplitAddressBySeparator());

            if (dependencyResource == null)
                throw new Exception(
                    logger.TryFormat<RuntimeResourceManager>(
                        $"RESOURCE {path} DOES NOT EXIST"));

            return dependencyResource;
        }

        #endregion

        private bool GetNestedResourceRecursive(
            ref IReadOnlyResourceData currentData,
            int[] resourcePathPartHashes)
        {
            for (int i = 1; i < resourcePathPartHashes.Length; i++)
            {
                if (!currentData.TryGetNestedResource(
                    resourcePathPartHashes[i],
                    out var newCurrentData))
                    return false;

                currentData = newCurrentData;
            }

            return true;
        }

        private bool GetNestedResourceRecursive(
            ref IReadOnlyResourceData currentData,
            string[] resourcePathParts)
        {
            for (int i = 1; i < resourcePathParts.Length; i++)
            {
                if (!currentData.TryGetNestedResource(
                    resourcePathParts[i],
                    out var newCurrentData))
                    return false;

                currentData = newCurrentData;
            }

            return true;
        }
    }
}