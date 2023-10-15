using HereticalSolutions.Repositories;

namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents a runtime resource manager.
    /// </summary>
    public class RuntimeResourceManager : IReadOnlyRuntimeResourceManager, IRuntimeResourceManager
    {
        private readonly IRepository<int, IReadOnlyResourceData> resourceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeResourceManager"/> class.
        /// </summary>
        /// <param name="resourceRepository">The repository for storing the resources.</param>
        public RuntimeResourceManager(IRepository<int, IReadOnlyResourceData> resourceRepository)
        {
            this.resourceRepository = resourceRepository;
        }

        #region IRuntimeResourceManager

        #region IReadOnlyRuntimeResourceManager

        /// <summary>
        /// Checks if a resource with the specified resource ID hash exists in the manager.
        /// </summary>
        /// <param name="resourceIDHash">The hash value of the resource ID.</param>
        /// <returns>True if the resource exists, false otherwise.</returns>
        public bool HasResource(int resourceIDHash)
        {
            return resourceRepository.Has(resourceIDHash);
        }

        /// <summary>
        /// Checks if a resource with the specified resource ID exists in the manager.
        /// </summary>
        /// <param name="resourceID">The ID of the resource.</param>
        /// <returns>True if the resource exists, false otherwise.</returns>
        public bool HasResource(string resourceID)
        {
            return HasResource(resourceID.AddressToHash());
        }

        /// <summary>
        /// Gets the resource with the specified resource ID hash.
        /// </summary>
        /// <param name="resourceIDHash">The hash value of the resource ID.</param>
        /// <returns>The read-only resource data associated with the resource ID hash, or null if not found.</returns>
        public IReadOnlyResourceData GetResource(int resourceIDHash)
        {
            if (!resourceRepository.TryGet(resourceIDHash, out var resource))
                return null;

            return resource;
        }

        /// <summary>
        /// Gets the resource with the specified resource ID.
        /// </summary>
        /// <param name="resourceID">The ID of the resource.</param>
        /// <returns>The read-only resource data associated with the resource ID, or null if not found.</returns>
        public IReadOnlyResourceData GetResource(string resourceID)
        {
            return GetResource(resourceID.AddressToHash());
        }

        /// <summary>
        /// Gets the default resource storage handle for the resource with the specified resource ID hash.
        /// </summary>
        /// <param name="resourceIDHash">The hash value of the resource ID.</param>
        /// <returns>The default resource storage handle associated with the resource ID hash.</returns>
        public IResourceStorageHandle GetDefaultResource(int resourceIDHash)
        {
            if (!resourceRepository.TryGet(resourceIDHash, out var resource))
                return null;

            var defaultVariant = resource.DefaultVariant;

            if (defaultVariant == null)
                return null;

            return defaultVariant.StorageHandle;
        }

        /// <summary>
        /// Gets the default resource storage handle for the resource with the specified resource ID.
        /// </summary>
        /// <param name="resourceID">The ID of the resource.</param>
        /// <returns>The default resource storage handle associated with the resource ID.</returns>
        public IResourceStorageHandle GetDefaultResource(string resourceID)
        {
            return GetDefaultResource(resourceID.AddressToHash());
        }

        #endregion

        /// <summary>
        /// Adds a resource to the runtime resource manager.
        /// </summary>
        /// <param name="resource">The read-only resource data to add.</param>
        public void AddResource(IReadOnlyResourceData resource)
        {
            resourceRepository.TryAdd(resource.Descriptor.IDHash, resource);
        }

        /// <summary>
        /// Removes a resource from the runtime resource manager.
        /// </summary>
        /// <param name="idHash">The hash value of the resource ID to remove. If not specified, all resources will be removed.</param>
        public void RemoveResource(int idHash = -1)
        {
            resourceRepository.TryRemove(idHash);
        }
        
        /// <summary>
        /// Removes a resource from the runtime resource manager.
        /// </summary>
        /// <param name="resourceID">The ID of the resource to remove.</param>
        public void RemoveResource(string resourceID)
        {
            RemoveResource(resourceID.AddressToHash());
        }

        #endregion
    }
}