using HereticalSolutions.Delegates.Factories;
using HereticalSolutions.HereticalEngine.Application;
using HereticalSolutions.Logging;
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
        public static RuntimeResourceManager BuildRuntimeResourceManager(
            IFormatLogger logger)
        {
            return new RuntimeResourceManager(
                RepositoriesFactory.BuildDictionaryRepository<int, string>(),
                RepositoriesFactory.BuildDictionaryRepository<int, IReadOnlyResourceData>(),
                logger);
        }

        public static ConcurrentRuntimeResourceManager BuildConcurrentRuntimeResourceManager(
            IFormatLogger logger)
        {
            return new ConcurrentRuntimeResourceManager(
                RepositoriesFactory.BuildConcurrentDictionaryRepository<int, string>(),
                RepositoriesFactory.BuildConcurrentDictionaryRepository<int, IReadOnlyResourceData>(),
                NotifiersFactory.BuildAsyncNotifierSingleArgGeneric<int, IReadOnlyResourceData>(logger),
                new SemaphoreSlim(1, 1),
                logger);
        }

        public static ResourceData BuildResourceData(
            ResourceDescriptor descriptor,
            IFormatLogger logger)
        {
            return new ResourceData(
                descriptor,
                RepositoriesFactory.BuildDictionaryRepository<int, string>(),
                RepositoriesFactory.BuildDictionaryRepository<int, IResourceVariantData>(),
                RepositoriesFactory.BuildDictionaryRepository<int, string>(),
                RepositoriesFactory.BuildDictionaryRepository<int, IReadOnlyResourceData>(),
                logger);
        }

        public static ConcurrentResourceData BuildConcurrentResourceData(
            ResourceDescriptor descriptor,
            IFormatLogger logger)
        {
            return new ConcurrentResourceData(
                descriptor,
                RepositoriesFactory.BuildConcurrentDictionaryRepository<int, string>(),
                RepositoriesFactory.BuildConcurrentDictionaryRepository<int, IResourceVariantData>(),
                NotifiersFactory.BuildAsyncNotifierSingleArgGeneric<int, IResourceVariantData>(logger),
                RepositoriesFactory.BuildConcurrentDictionaryRepository<int, string>(),
                RepositoriesFactory.BuildConcurrentDictionaryRepository<int, IReadOnlyResourceData>(),
                NotifiersFactory.BuildAsyncNotifierSingleArgGeneric<int, IReadOnlyResourceData>(logger),
                new SemaphoreSlim(1, 1),
                logger);
        }

        /// <summary>
        /// Builds a new instance of the <see cref="ResourceVariantData"/> class.
        /// </summary>
        /// <param name="descriptor">The descriptor of the resource variant data.</param>
        /// <param name="storageHandle">The storage handle of the resource variant data.</param>
        /// <returns>A new instance of the <see cref="ResourceVariantData"/> class.</returns>
        public static ResourceVariantData BuildResourceVariantData(
            ResourceVariantDescriptor descriptor,
            IReadOnlyResourceStorageHandle storageHandle,
            IReadOnlyResourceData resource)
        {
            return new ResourceVariantData(
                descriptor,
                storageHandle,
                resource);
        }

        public static PreallocatedResourceStorageHandle<TResource> BuildPreallocatedResourceStorageHandle<TResource>(
            TResource resource,
            ApplicationContext context)
        {
            return new PreallocatedResourceStorageHandle<TResource>(
                resource,
                context);
        }

        public static ConcurrentPreallocatedResourceStorageHandle<TResource> BuildConcurrentPreallocatedResourceStorageHandle<TResource>(
            TResource resource,
            ApplicationContext context)
        {
            return new ConcurrentPreallocatedResourceStorageHandle<TResource>(
                resource,
                new SemaphoreSlim(1, 1),
                context);
        }

        public static ReadWriteResourceStorageHandle<TResource> BuildReadWriteResourceStorageHandle<TResource>(
            TResource resource,
            ApplicationContext context)
        {
            return new ReadWriteResourceStorageHandle<TResource>(
                resource,
                context);
        }

        public static ConcurrentReadWriteResourceStorageHandle<TResource> BuildConcurrentReadWriteResourceStorageHandle<TResource>(
            TResource resource,
            ApplicationContext context)
        {
            return new ConcurrentReadWriteResourceStorageHandle<TResource>(
                resource,
                new SemaphoreSlim(1, 1),
                context);
        }
    }
}