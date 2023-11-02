using HereticalSolutions.Repositories;

namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents a runtime resource manager.
    /// </summary>
    public class RuntimeResourceManager
        : IRuntimeResourceManager
    {
        private readonly IRepository<int, string> rootResourceIDHashToID;

        private readonly IRepository<int, IReadOnlyResourceData> rootResourcesRepository;

        public RuntimeResourceManager(
            IRepository<int, string> rootResourceIDHashToID,
            IRepository<int, IReadOnlyResourceData> rootResourcesRepository)
        {
            this.rootResourceIDHashToID = rootResourceIDHashToID;

            this.rootResourcesRepository = rootResourcesRepository;
        }

        #region IRuntimeResourceManager

        #region IReadOnlyRuntimeResourceManager

        #region Has

        public bool HasRootResource(int resourceIDHash)
        {
            return rootResourcesRepository.Has(resourceIDHash);
        }

        public bool HasRootResource(string resourceID)
        {
            return HasRootResource(resourceID.AddressToHash());
        }

        public bool HasResource(int[] resourceIDHashes)
        {
            if (!rootResourcesRepository.TryGet(
                resourceIDHashes[0],
                out var currentData))
                return false;

            return GetNestedResourceRecursive(
                ref currentData,
                resourceIDHashes);
        }

        public bool HasResource(string[] resourceIDs)
        {
            if (!rootResourcesRepository.TryGet(
                resourceIDs[0].AddressToHash(),
                out var currentData))
                return false;

            return GetNestedResourceRecursive(
                ref currentData,
                resourceIDs);
        }

        #endregion

        #region Get

        public IReadOnlyResourceData GetRootResource(int resourceIDHash)
        {
            if (!rootResourcesRepository.TryGet(
                resourceIDHash,
                out var resource))
                return null;

            return resource;
        }

        public IReadOnlyResourceData GetRootResource(string resourceID)
        {
            return GetRootResource(resourceID.AddressToHash());
        }

        public IReadOnlyResourceData GetResource(int[] resourceIDHashes)
        {
            if (!rootResourcesRepository.TryGet(
                resourceIDHashes[0],
                out var currentResource))
                return null;

            if (!GetNestedResourceRecursive(
                ref currentResource,
                resourceIDHashes))
                return null;

            return currentResource;
        }

        public IReadOnlyResourceData GetResource(string[] resourceIDs)
        {
            if (!rootResourcesRepository.TryGet(
                resourceIDs[0].AddressToHash(),
                out var currentResource))
                return null;

            if (!GetNestedResourceRecursive(
                ref currentResource,
                resourceIDs))
                return null;

            return currentResource;
        }

        #endregion

        #region Try get

        public bool TryGetRootResource(
            int resourceIDHash,
            out IReadOnlyResourceData resource)
        {
            return rootResourcesRepository.TryGet(
                resourceIDHash,
                out resource);
        }

        public bool TryGetRootResource(
            string resourceID,
            out IReadOnlyResourceData resource)
        {
            return TryGetRootResource(
                resourceID.AddressToHash(),
                out resource);
        }

        public bool TryGetResource(
            int[] resourceIDHashes,
            out IReadOnlyResourceData resource)
        {
            if (!rootResourcesRepository.TryGet(
                resourceIDHashes[0],
                out resource))
                return false;

            return GetNestedResourceRecursive(
                ref resource,
                resourceIDHashes);
        }

        public bool TryGetResource(
            string[] resourceIDs,
            out IReadOnlyResourceData resource)
        {
            if (!rootResourcesRepository.TryGet(
                resourceIDs[0].AddressToHash(),
                out resource))
                return false;

            return GetNestedResourceRecursive(
                ref resource,
                resourceIDs);
        }

        #endregion

        #region Get default

        public IResourceVariantData GetDefaultRootResource(int resourceIDHash)
        {
            if (!rootResourcesRepository.TryGet(
                resourceIDHash,
                out var resource))
                return null;

            return resource.DefaultVariant;
        }

        public IResourceVariantData GetDefaultRootResource(string resourceID)
        {
            return GetDefaultRootResource(resourceID.AddressToHash());
        }

        public IResourceVariantData GetDefaultResource(int[] resourceIDHashes)
        {
            if (!rootResourcesRepository.TryGet(
                resourceIDHashes[0],
                out var currentResource))
                return null;

            if (!GetNestedResourceRecursive(
                ref currentResource,
                resourceIDHashes))
                return null;

            return currentResource.DefaultVariant;
        }

        public IResourceVariantData GetDefaultResource(string[] resourceIDs)
        {
            if (!rootResourcesRepository.TryGet(
                resourceIDs[0].AddressToHash(),
                out var currentResource))
                return null;

            if (!GetNestedResourceRecursive(
                ref currentResource,
                resourceIDs))
                return null;

            return currentResource.DefaultVariant;
        }

        #endregion

        #region Try get default

        public bool TryGetDefaultRootResource(
            int resourceIDHash,
            out IResourceVariantData resource)
        {
            if (!rootResourcesRepository.TryGet(
                resourceIDHash,
                out var rootResource))
            {
                resource = null;

                return false;
            }

            resource = rootResource.DefaultVariant;

            return true;
        }

        public bool TryGetDefaultRootResource(
            string resourceID,
            out IResourceVariantData resource)
        {
            return TryGetDefaultRootResource(
                resourceID.AddressToHash(),
                out resource);
        }

        public bool TryGetDefaultResource(
            int[] resourceIDHashes,
            out IResourceVariantData resource)
        {
            if (!rootResourcesRepository.TryGet(
                resourceIDHashes[0],
                out var currentResource))
            {
                resource = null;

                return false;
            }

            if (!GetNestedResourceRecursive(
                ref currentResource,
                resourceIDHashes))
            {
                resource = null;

                return false;
            }

            resource = currentResource.DefaultVariant;

            return true;
        }

        public bool TryGetDefaultResource(
            string[] resourceIDs,
            out IResourceVariantData resource)
        {
            if (!rootResourcesRepository.TryGet(
                resourceIDs[0].AddressToHash(),
                out var currentResource))
            {
                resource = null;

                return false;
            }

            if (!GetNestedResourceRecursive(
                ref currentResource,
                resourceIDs))
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
            IReadOnlyResourceData resource,
            IProgress<float> progress = null)
        {
            progress?.Report(0f);

            if (!rootResourcesRepository.TryAdd(
                resource.Descriptor.IDHash,
                resource))
            {
                progress?.Report(1f);

                return;
            }

            ((IResourceData)resource).ParentResource = null;

            rootResourceIDHashToID.AddOrUpdate(
                resource.Descriptor.IDHash,
                resource.Descriptor.ID);

            progress?.Report(1f);
        }

        public async Task RemoveRootResource(
            int idHash = -1,
            bool free = true,
            IProgress<float> progress = null)
        {
            progress?.Report(0f);

            if (!rootResourcesRepository.TryGet(
                idHash,
                out var resource))
            {
                progress?.Report(1f);

                return;
            }

            rootResourcesRepository.TryRemove(idHash);

            rootResourceIDHashToID.TryRemove(idHash);

            if (free)
                await ((IResourceData)resource).Clear(
                    free,
                    progress);

            progress?.Report(1f);
        }

        public async Task RemoveRootResource(
            string resourceID,
            bool free = true,
            IProgress<float> progress = null)
        {
            await RemoveRootResource(
                resourceID.AddressToHash(),
                free,
                progress);
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
                    IProgress<float> localProgress = null;

                    if (progress != null)
                    {
                        var localProgressInstance = new Progress<float>();

                        localProgressInstance.ProgressChanged += (sender, value) =>
                        {
                            progress.Report((value + (float)counter) / (float)totalRootResourcesCount);
                        };

                        localProgress = localProgressInstance;
                    }

                    await ((IResourceData)rootResource).Clear(
                        free,
                        localProgress);
                }

                counter++;

                progress?.Report((float)counter / (float)totalRootResourcesCount);

            }

            rootResourceIDHashToID.Clear();

            rootResourcesRepository.Clear();

            progress?.Report(1f);
        }

        #endregion

        private bool GetNestedResourceRecursive(
            ref IReadOnlyResourceData currentData,
            int[] resourceIDHashes)
        {
            for (int i = 1; i < resourceIDHashes.Length; i++)
            {
                if (!currentData.TryGetNestedResource(
                    resourceIDHashes[i],
                    out var newCurrentData))
                    return false;

                currentData = newCurrentData;
            }

            return true;
        }

        private bool GetNestedResourceRecursive(
            ref IReadOnlyResourceData currentData,
            string[] resourceIDs)
        {
            for (int i = 1; i < resourceIDs.Length; i++)
            {
                if (!currentData.TryGetNestedResource(
                    resourceIDs[i],
                    out var newCurrentData))
                    return false;

                currentData = newCurrentData;
            }

            return true;
        }
    }
}