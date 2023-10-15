using System;
using System.Threading.Tasks;

namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents a storage handle for a resource.
    /// </summary>
    public interface IResourceStorageHandle
    {
        /// <summary>
        /// Gets a value indicating whether the resource is allocated.
        /// </summary>
        bool Allocated { get; }

        /// <summary>
        /// Allocates the resource asynchronously.
        /// </summary>
        /// <param name="progress">An optional progress reporter for tracking the allocation progress.</param>
        /// <returns>A task representing the asynchronous allocation operation.</returns>
        Task Allocate(IProgress<float> progress = null);

        /// <summary>
        /// Gets the resource object.
        /// </summary>
        object Resource { get; }

        /// <summary>
        /// Gets the resource object of the specified type.
        /// </summary>
        /// <typeparam name="TValue">The type of the resource object.</typeparam>
        /// <returns>The resource object of the specified type, or the default value if not allocated.</returns>
        TValue GetResource<TValue>();

        /// <summary>
        /// Frees the allocated resource asynchronously.
        /// </summary>
        /// <param name="progress">An optional progress reporter for tracking the free operation progress.</param>
        /// <returns>A task representing the asynchronous free operation.</returns>
        Task Free(IProgress<float> progress = null);
    }
}