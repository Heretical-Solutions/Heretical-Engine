namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents a variant of a resource
    /// </summary>
    public interface IResourceVariantData
    {
        /// <summary>
        /// Gets the descriptor of the resource variant
        /// </summary>
        /// <value>The descriptor of the resource variant.</value>
        ResourceVariantDescriptor Descriptor { get; }

        IReadOnlyResourceStorageHandle StorageHandle { get; }

        IReadOnlyResourceData Resource { get; }
    }
}