using HereticalSolutions.Pools.AllocationCallbacks;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// A factory class for creating object pool allocation callbacks.
    /// </summary>
    public static partial class PoolsFactory
    {
        #region Allocation callbacks

        /// <summary>
        /// Builds a composite allocation callback that combines multiple allocation callbacks.
        /// </summary>
        /// <typeparam name="T">The type of object being allocated.</typeparam>
        /// <param name="callbacks">An array of allocation callbacks to combine.</param>
        /// <returns>A composite allocation callback.</returns>
        public static CompositeCallback<T> BuildCompositeCallback<T>(IAllocationCallback<T>[] callbacks)
        {
            return new CompositeCallback<T>(callbacks);
        }
        
        /// <summary>
        /// Builds a push to pool allocation callback that pushes allocated objects back to the specified object pool.
        /// </summary>
        /// <typeparam name="T">The type of object being allocated.</typeparam>
        /// <param name="root">The object pool to push the allocated objects back to. If null, a new object pool is created.</param>
        /// <returns>A push to pool allocation callback.</returns>
        public static PushToPoolCallback<T> BuildPushToPoolCallback<T>(INonAllocPool<T> root = null)
        {
            return new PushToPoolCallback<T>(root);
        }
        
        /// <summary>
        /// Builds a push to pool allocation callback that pushes allocated objects back to the specified deferred callback queue, instead of immediately returning them to the pool.
        /// </summary>
        /// <typeparam name="T">The type of object being allocated.</typeparam>
        /// <param name="deferredCallbackQueue">The deferred callback queue to push the allocated objects to.</param>
        /// <returns>A push to pool allocation callback.</returns>
        public static PushToPoolCallback<T> BuildPushToPoolCallback<T>(DeferredCallbackQueue<T> deferredCallbackQueue)
        {
            return new PushToPoolCallback<T>(deferredCallbackQueue);
        }

        /// <summary>
        /// Builds a deferred callback queue for storing allocated objects and returning them to the pool later.
        /// </summary>
        /// <typeparam name="T">The type of object being allocated.</typeparam>
        /// <returns>A deferred callback queue.</returns>
        public static DeferredCallbackQueue<T> BuildDeferredCallbackQueue<T>()
        {
            return new DeferredCallbackQueue<T>();
        }

        #endregion
    }
}