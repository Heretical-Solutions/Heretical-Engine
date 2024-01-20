using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

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
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<RuntimeResourceManager>()
                ?? null;

            return new RuntimeResourceManager(
                RepositoriesFactory.BuildDictionaryRepository<int, string>(),
                RepositoriesFactory.BuildDictionaryRepository<int, IReadOnlyResourceData>(),
                logger);
        }

        public static ConcurrentRuntimeResourceManager BuildConcurrentRuntimeResourceManager(
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentRuntimeResourceManager>()
                ?? null;

            return new ConcurrentRuntimeResourceManager(
                RepositoriesFactory.BuildConcurrentDictionaryRepository<int, string>(),
                RepositoriesFactory.BuildConcurrentDictionaryRepository<int, IReadOnlyResourceData>(),
                NotifiersFactory.BuildAsyncNotifierSingleArgGeneric<int, IReadOnlyResourceData>(loggerResolver),
                new SemaphoreSlim(1, 1),
                logger);
        }

        public static ResourceData BuildResourceData(
            ResourceDescriptor descriptor,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ResourceData>()
                ?? null;

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
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentResourceData>()
                ?? null;

            return new ConcurrentResourceData(
                descriptor,
                RepositoriesFactory.BuildConcurrentDictionaryRepository<int, string>(),
                RepositoriesFactory.BuildConcurrentDictionaryRepository<int, IResourceVariantData>(),
                NotifiersFactory.BuildAsyncNotifierSingleArgGeneric<int, IResourceVariantData>(loggerResolver),
                RepositoriesFactory.BuildConcurrentDictionaryRepository<int, string>(),
                RepositoriesFactory.BuildConcurrentDictionaryRepository<int, IReadOnlyResourceData>(),
                NotifiersFactory.BuildAsyncNotifierSingleArgGeneric<int, IReadOnlyResourceData>(loggerResolver),
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
            IRuntimeResourceManager runtimeResourceManager,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<PreallocatedResourceStorageHandle<TResource>>()
                ?? null;

            return new PreallocatedResourceStorageHandle<TResource>(
                resource,
                runtimeResourceManager,
                logger);
        }

        public static ConcurrentPreallocatedResourceStorageHandle<TResource> BuildConcurrentPreallocatedResourceStorageHandle<TResource>(
            TResource resource,
            IRuntimeResourceManager runtimeResourceManager,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentPreallocatedResourceStorageHandle<TResource>>()
                ?? null;

            return new ConcurrentPreallocatedResourceStorageHandle<TResource>(
                resource,
                new SemaphoreSlim(1, 1),
                runtimeResourceManager,
                logger);
        }

        public static ReadWriteResourceStorageHandle<TResource> BuildReadWriteResourceStorageHandle<TResource>(
            TResource resource,
            IRuntimeResourceManager runtimeResourceManager,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ReadWriteResourceStorageHandle<TResource>>()
                ?? null;

            return new ReadWriteResourceStorageHandle<TResource>(
                resource,
                runtimeResourceManager,
                logger);
        }

        public static ConcurrentReadWriteResourceStorageHandle<TResource> BuildConcurrentReadWriteResourceStorageHandle<TResource>(
            TResource resource,
            IRuntimeResourceManager runtimeResourceManager,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentReadWriteResourceStorageHandle<TResource>>()
                ?? null;

            return new ConcurrentReadWriteResourceStorageHandle<TResource>(
                resource,
                new SemaphoreSlim(1, 1),
                runtimeResourceManager,
                logger);
        }
    }
}