using System;
using System.Threading.Tasks;

namespace HereticalSolutions.ResourceManagement
{
    public class PreallocatedRuntimeResourceStorageHandle
        : IResourceStorageHandle
    {
        private bool allocated = false;

        private object rawResource;

        public PreallocatedRuntimeResourceStorageHandle(
            object rawResource)
        {
            this.rawResource = rawResource;

            allocated = true;
        }

        #region IResourceStorageHandle

        /// <summary>
        /// Gets a value indicating whether the resource is allocated.
        /// </summary>
        public bool Allocated
        {
            get => allocated;
        }

        public virtual async Task Allocate(
            IProgress<float> progress = null)
        {
            allocated = true;

            progress?.Report(1f);
        }

        public object RawResource
        {
            get => rawResource;
        }

        /// <summary>
        /// Gets the resource object of the specified type.
        /// </summary>
        /// <typeparam name="TValue">The type of the resource object.</typeparam>
        /// <returns>The resource object of the specified type.</returns>
        public TValue GetResource<TValue>()
        {
            return (TValue)rawResource;
        }

        /// <summary>
        /// Frees the allocated resource asynchronously.
        /// </summary>
        /// <param name="progress">An optional progress reporter for tracking the free operation progress.</param>
        /// <returns>A task representing the asynchronous free operation.</returns>
        public virtual async Task Free(
            IProgress<float> progress = null)
        {
            allocated = false;

            progress?.Report(1f);
        }

        #endregion
    }
}