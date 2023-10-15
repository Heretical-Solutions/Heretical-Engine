namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents a variant of a resource.
    /// </summary>
    public class ResourceVariantData : IResourceVariantData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVariantData"/> class.
        /// </summary>
        /// <param name="descriptor">The descriptor of the resource variant.</param>
        /// <param name="storageHandle">The storage handle for the resource variant.</param>
        public ResourceVariantData(
            ResourceVariantDescriptor descriptor,
            IResourceStorageHandle storageHandle)
        {
            Descriptor = descriptor;
            StorageHandle = storageHandle;
        }
        
        #region IResourceVariantData
        
        /// <summary>
        /// Gets the descriptor of the resource variant.
        /// </summary>
        /// <value>The descriptor of the resource variant.</value>
        public ResourceVariantDescriptor Descriptor { get; private set; }
        
        /// <summary>
        /// Gets the storage handle for the resource variant.
        /// </summary>
        /// <value>The storage handle for the resource variant.</value>
        public IResourceStorageHandle StorageHandle { get; private set; }
        
        #endregion
    }
}