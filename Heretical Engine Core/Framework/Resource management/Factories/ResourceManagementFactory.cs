using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.ResourceManagement.Factories
{
    /// <summary>
    /// Factory class for creating instances related to the runtime resource manager.
    /// </summary>
    public static class ResourceManagementFactory
    {
        /// <summary>
        /// Builds a new instance of the <see cref="RuntimeResourceManager"/> class.
        /// </summary>
        /// <returns>A new instance of the <see cref="RuntimeResourceManager"/> class.</returns>
        public static RuntimeResourceManager BuildRuntimeResourceManager()
        {
            return new RuntimeResourceManager(
                RepositoriesFactory.BuildDictionaryRepository<int, string>(),
                RepositoriesFactory.BuildDictionaryRepository<int, IReadOnlyResourceData>());
        }

        public static ResourceData BuildResourceData(ResourceDescriptor descriptor)
        {
            return new ResourceData(
                descriptor,
                RepositoriesFactory.BuildDictionaryRepository<int, string>(),
                RepositoriesFactory.BuildDictionaryRepository<int, IResourceVariantData>(),
                RepositoriesFactory.BuildDictionaryRepository<int, string>(),
                RepositoriesFactory.BuildDictionaryRepository<int, IReadOnlyResourceData>());
        }

        public static ConcurrentResourceData BuildConcurrentResourceData(ResourceDescriptor descriptor)
        {
            return new ConcurrentResourceData(
                descriptor,
                RepositoriesFactory.BuildDictionaryRepository<int, string>(),
                RepositoriesFactory.BuildDictionaryRepository<int, IResourceVariantData>(),
                RepositoriesFactory.BuildDictionaryRepository<int, string>(),
                RepositoriesFactory.BuildDictionaryRepository<int, IReadOnlyResourceData>(),
                new ReaderWriterLockSlim());
        }

        /// <summary>
        /// Builds a new instance of the <see cref="ResourceVariantData"/> class.
        /// </summary>
        /// <param name="descriptor">The descriptor of the resource variant data.</param>
        /// <param name="storageHandle">The storage handle of the resource variant data.</param>
        /// <returns>A new instance of the <see cref="ResourceVariantData"/> class.</returns>
        public static ResourceVariantData BuildResourceVariantData(
            ResourceVariantDescriptor descriptor,
            IReadOnlyResourceStorageHandle storageHandle)
        {
            return new ResourceVariantData(
                descriptor,
                storageHandle);
        }

        public static PreallocatedResourceStorageHandle BuildPreallocatedResourceStorageHandle(object resource)
        {
            return new PreallocatedResourceStorageHandle(resource);
        }

        public static ConcurrentPreallocatedResourceStorageHandle BuildConcurrentPreallocatedResourceStorageHandle(object resource)
        {
            return new ConcurrentPreallocatedResourceStorageHandle(
                resource,
                new ReaderWriterLockSlim());
        }

        public static ReadWriteResourceStorageHandle BuildReadWriteResourceStorageHandle(object resource)
        {
            return new ReadWriteResourceStorageHandle(resource);
        }

        public static ConcurrentReadWriteResourceStorageHandle BuildConcurrentReadWriteResourceStorageHandle(object resource)
        {
            return new ConcurrentReadWriteResourceStorageHandle(
                resource,
                new ReaderWriterLockSlim());
        }
    }
}