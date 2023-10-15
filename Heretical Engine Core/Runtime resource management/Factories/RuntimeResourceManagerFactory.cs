using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.ResourceManagement.Factories
{
    /// <summary>
    /// Factory class for creating instances related to the runtime resource manager.
    /// </summary>
    public static class RuntimeResourceManagerFactory
    {
        /// <summary>
        /// Builds a new instance of the <see cref="RuntimeResourceManager"/> class.
        /// </summary>
        /// <returns>A new instance of the <see cref="RuntimeResourceManager"/> class.</returns>
        public static RuntimeResourceManager BuildRuntimeResourceManager()
        {
            return new RuntimeResourceManager(
                RepositoriesFactory.BuildDictionaryRepository<int, IReadOnlyResourceData>());
        }

        /// <summary>
        /// Builds a new instance of the <see cref="ResourceData"/> class.
        /// </summary>
        /// <param name="descriptor">The descriptor of the resource data.</param>
        /// <returns>A new instance of the <see cref="ResourceData"/> class.</returns>
        public static ResourceData BuildResourceData(ResourceDescriptor descriptor)
        {
            return new ResourceData(
                descriptor,
                RepositoriesFactory.BuildDictionaryRepository<int, IResourceVariantData>());
        }
        
        /// <summary>
        /// Builds a new instance of the <see cref="ResourceVariantData"/> class.
        /// </summary>
        /// <param name="descriptor">The descriptor of the resource variant data.</param>
        /// <param name="storageHandle">The storage handle of the resource variant data.</param>
        /// <returns>A new instance of the <see cref="ResourceVariantData"/> class.</returns>
        public static ResourceVariantData BuildResourceVariantData(
            ResourceVariantDescriptor descriptor,
            IResourceStorageHandle storageHandle)
        {
            return new ResourceVariantData(
                descriptor,
                storageHandle);
        }
    }
}