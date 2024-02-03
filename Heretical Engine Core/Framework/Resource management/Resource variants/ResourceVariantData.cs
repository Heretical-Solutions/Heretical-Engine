using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents a variant of a resource.
    /// </summary>
    public class ResourceVariantData
        : IResourceVariantData,
          ICleanUppable,
          IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVariantData"/> class.
        /// </summary>
        /// <param name="descriptor">The descriptor of the resource variant.</param>
        /// <param name="storageHandle">The storage handle for the resource variant.</param>
        public ResourceVariantData(
            ResourceVariantDescriptor descriptor,
            IReadOnlyResourceStorageHandle storageHandle,
            IReadOnlyResourceData resource)
        {
            Descriptor = descriptor;
            
            StorageHandle = storageHandle;

            Resource = resource;
        }
        
        #region IResourceVariantData
        
        /// <summary>
        /// Gets the descriptor of the resource variant.
        /// </summary>
        /// <value>The descriptor of the resource variant.</value>
        public ResourceVariantDescriptor Descriptor { get; private set; }
        
        public IReadOnlyResourceStorageHandle StorageHandle { get; private set; }

        public IReadOnlyResourceData Resource { get; private set; }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (StorageHandle is ICleanUppable)
                (StorageHandle as ICleanUppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (StorageHandle is IDisposable)
                (StorageHandle as IDisposable).Dispose();
        }

        #endregion
    }
}