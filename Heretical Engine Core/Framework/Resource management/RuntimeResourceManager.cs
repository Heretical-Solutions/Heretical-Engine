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
            if (!HasRootResource(resourceIDHashes[0]))
                return false;
            
            IReadOnlyResourceData currentData = GetRootResource(resourceIDHashes[0]);

            for (int i = 1; i < resourceIDHashes.Length; i++)
            {
                if (!currentData.HasNestedResource(resourceIDHashes[i]))
                    return false;

                currentData = currentData.GetNestedResource(resourceIDHashes[i]);
            }

            return true;
        }

        public bool HasResource(string[] resourceIDs)
        {
            if (!HasRootResource(resourceIDs[0]))
                return false;

            IReadOnlyResourceData currentData = GetRootResource(resourceIDs[0]);

            for (int i = 1; i < resourceIDs.Length; i++)
            {
                if (!currentData.HasNestedResource(resourceIDs[i]))
                    return false;

                currentData = currentData.GetNestedResource(resourceIDs[i]);
            }

            return true;
        }

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

            for (int i = 1; i < resourceIDHashes.Length; i++)
            {
                currentResource = currentResource.GetNestedResource(resourceIDHashes[i]);

                if (currentResource == null)
                    return null;
            }

            return currentResource;
        }

        public IReadOnlyResourceData GetResource(string[] resourceIDs)
        {
            if (!rootResourcesRepository.TryGet(
                resourceIDs[0].AddressToHash(),
                out var currentResource))
                return null;

            for (int i = 1; i < resourceIDs.Length; i++)
            {
                currentResource = currentResource.GetNestedResource(resourceIDs[i]);

                if (currentResource == null)
                    return null;
            }

            return currentResource;
        }

        public IResourceVariantData GetDefaultRootResource(int resourceIDHash)
        {
            if (!rootResourcesRepository.TryGet(resourceIDHash, out var resource))
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

            for (int i = 1; i < resourceIDHashes.Length; i++)
            {
                currentResource = currentResource.GetNestedResource(resourceIDHashes[i]);

                if (currentResource == null)
                    return null;
            }

            return currentResource.DefaultVariant;
        }

        public IResourceVariantData GetDefaultResource(string[] resourceIDs)
        {
            if (!rootResourcesRepository.TryGet(
                resourceIDs[0].AddressToHash(),
                out var currentResource))
                return null;

            for (int i = 1; i < resourceIDs.Length; i++)
            {
                currentResource = currentResource.GetNestedResource(resourceIDs[i]);

                if (currentResource == null)
                    return null;
            }

            return currentResource.DefaultVariant;
        }

        public IEnumerable<int> RootResourceIDHashes { get => rootResourcesRepository.Keys; }

        public IEnumerable<string> RootResourceIDs { get => rootResourceIDHashToID.Values; }

        public IEnumerable<IReadOnlyResourceData> AllRootResources { get => rootResourcesRepository.Values; }

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

            rootResourceIDHashToID.TryAdd(
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
                if (!rootResourcesRepository.TryGet(
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
    }
}