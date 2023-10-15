using System;
using HereticalSolutions.Collections;
using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;
using HereticalSolutions.Pools.Allocations;
using HereticalSolutions.Pools.Elements;
using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// A static class that provides methods for building pool elements and allocation commands for pool elements.
    /// </summary>
    public static partial class PoolsFactory
    {
        #region Pool element allocation command

        /// <summary>
        /// Builds an allocation command for a pool element with a callback.
        /// </summary>
        /// <typeparam name="T">The type of values stored in the pool element.</typeparam>
        /// <param name="descriptor">The allocation command descriptor.</param>
        /// <param name="valueAllocationDelegate">The delegate to use for allocating values.</param>
        /// <param name="metadataDescriptors">The descriptors for allocating metadata.</param>
        /// <param name="callback">The allocation callback.</param>
        /// <returns>An allocation command for a pool element with a callback.</returns>
        public static AllocationCommand<IPoolElement<T>> BuildPoolElementAllocationCommandWithCallback<T>(
            AllocationCommandDescriptor descriptor,
            Func<T> valueAllocationDelegate,
            MetadataAllocationDescriptor[] metadataDescriptors,
            IAllocationCallback<T> callback)
        {
            Func<IPoolElement<T>> poolElementAllocationDelegate = () =>
                BuildPoolElementWithAllocationCallback(
                    valueAllocationDelegate,
                    metadataDescriptors,
                    callback);

            var poolElementAllocationCommand = new AllocationCommand<IPoolElement<T>>
            {
                Descriptor = descriptor,
                AllocationDelegate = poolElementAllocationDelegate
            };

            return poolElementAllocationCommand;
        }

        /// <summary>
        /// Builds an allocation command for a pool element.
        /// </summary>
        /// <typeparam name="T">The type of values stored in the pool element.</typeparam>
        /// <param name="descriptor">The allocation command descriptor.</param>
        /// <param name="valueAllocationDelegate">The delegate to use for allocating values.</param>
        /// <param name="metadataDescriptors">The descriptors for allocating metadata.</param>
        /// <returns>An allocation command for a pool element.</returns>
        public static AllocationCommand<IPoolElement<T>> BuildPoolElementAllocationCommand<T>(
            AllocationCommandDescriptor descriptor,
            Func<T> valueAllocationDelegate,
            MetadataAllocationDescriptor[] metadataDescriptors)
        {
            Func<IPoolElement<T>> poolElementAllocationDelegate = () =>
                BuildPoolElement(
                    valueAllocationDelegate,
                    metadataDescriptors);

            var poolElementAllocationCommand = new AllocationCommand<IPoolElement<T>>
            {
                Descriptor = descriptor,
                AllocationDelegate = poolElementAllocationDelegate
            };

            return poolElementAllocationCommand;
        }

        #endregion

        #region Pool element

        /// <summary>
        /// Builds a pool element with an allocation callback.
        /// </summary>
        /// <typeparam name="T">The type of values stored in the pool element.</typeparam>
        /// <param name="allocationDelegate">The delegate to use for allocating values.</param>
        /// <param name="metadataDescriptors">The descriptors for allocating metadata.</param>
        /// <param name="callback">The allocation callback.</param>
        /// <returns>A pool element with an allocation callback.</returns>
        public static IPoolElement<T> BuildPoolElementWithAllocationCallback<T>(
            Func<T> allocationDelegate,
            MetadataAllocationDescriptor[] metadataDescriptors,
            IAllocationCallback<T> callback)
        {
            var metadata = BuildMetadataRepository(metadataDescriptors);

            var result = new PoolElement<T>(
                AllocationsFactory.FuncAllocationDelegate(allocationDelegate),
                metadata);

            callback?.OnAllocated(result);

            return result;
        }

        /// <summary>
        /// Builds a pool element.
        /// </summary>
        /// <typeparam name="T">The type of values stored in the pool element.</typeparam>
        /// <param name="allocationDelegate">The delegate to use for allocating values.</param>
        /// <param name="metadataDescriptors">The descriptors for allocating metadata.</param>
        /// <returns>A pool element.</returns>
        public static IPoolElement<T> BuildPoolElement<T>(
            Func<T> allocationDelegate,
            MetadataAllocationDescriptor[] metadataDescriptors)
        {
            var metadata = BuildMetadataRepository(metadataDescriptors);

            return new PoolElement<T>(
                AllocationsFactory.FuncAllocationDelegate(allocationDelegate),
                metadata);
        }

        #endregion

        #region Metadata

        /// <summary>
        /// Builds a repository for storing metadata.
        /// </summary>
        /// <param name="metadataDescriptors">The descriptors for allocating metadata.</param>
        /// <returns>A repository for storing metadata.</returns>
        public static IReadOnlyObjectRepository BuildMetadataRepository(MetadataAllocationDescriptor[] metadataDescriptors)
        {
            IRepository<Type, object> repository = RepositoriesFactory.BuildDictionaryRepository<Type, object>();

            if (metadataDescriptors != null)
                foreach (var descriptor in metadataDescriptors)
                {
                    if (descriptor == null)
                        continue;

                    repository.Add(
                        descriptor.BindingType,
                        AllocationsFactory.ActivatorAllocationDelegate(descriptor.ConcreteType));
                }

            return RepositoriesFactory.BuildDictionaryObjectRepository(repository);
        }

        /// <summary>
        /// Builds an indexed metadata object.
        /// </summary>
        /// <returns>An indexed metadata object.</returns>
        public static IndexedMetadata BuildIndexedMetadata()
        {
            return new IndexedMetadata();
        }

        /// <summary>
        /// Builds a metadata allocation descriptor for an indexed metadata object.
        /// </summary>
        /// <returns>A metadata allocation descriptor for an indexed metadata object.</returns>
        public static MetadataAllocationDescriptor BuildIndexedMetadataDescriptor()
        {
            return new MetadataAllocationDescriptor
            {
                BindingType = typeof(IIndexed),
                ConcreteType = typeof(IndexedMetadata)
            };
        }

        #endregion
    }
}