using HereticalSolutions.Pools.AllocationCallbacks;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Class that provides methods for creating pool allocation callbacks.
    /// </summary>
    public static partial class PoolsFactory
    {
        #region Allocation callbacks

        /// <summary>
        /// Builds a push to decorated pool callback for a given root pool.
        /// </summary>
        /// <typeparam name="T">The type of object stored in the pool.</typeparam>
        /// <param name="root">The root pool to push objects to.</param>
        /// <returns>The push to decorated pool callback.</returns>
        public static PushToDecoratedPoolCallback<T> BuildPushToDecoratedPoolCallback<T>(INonAllocDecoratedPool<T> root = null)
        {
            return new PushToDecoratedPoolCallback<T>(root);
        }
        
        /// <summary>
        /// Builds a push to decorated pool callback for a given deferred callback queue.
        /// </summary>
        /// <typeparam name="T">The type of object stored in the pool.</typeparam>
        /// <param name="deferredCallbackQueue">The deferred callback queue to push objects to.</param>
        /// <returns>The push to decorated pool callback.</returns>
        public static PushToDecoratedPoolCallback<T> BuildPushToDecoratedPoolCallback<T>(DeferredCallbackQueue<T> deferredCallbackQueue)
        {
            return new PushToDecoratedPoolCallback<T>(deferredCallbackQueue);
        }
        
        #endregion
    }
}