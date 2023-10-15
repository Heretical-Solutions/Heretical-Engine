namespace HereticalSolutions.Pools.AllocationCallbacks
{
    /// <summary>
    /// Represents a callback that pushes an allocated element to a pool.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the pool.</typeparam>
    public class PushToPoolCallback<T> : IAllocationCallback<T>
    {
        private INonAllocPool<T> root;

        /// <summary>
        /// Gets or sets the root pool that elements will be pushed into.
        /// </summary>
        public INonAllocPool<T> Root
        {
            get => root;
            set
            {
                root = value;

                if (deferredCallbackQueue != null)
                {
                    // Process any enqueued callbacks.
                    deferredCallbackQueue.Process();

                    // Clear the reference to this callback in the deferred callback queue.
                    deferredCallbackQueue.Callback = null;

                    // Release the reference to the deferred callback queue.
                    deferredCallbackQueue = null;
                }
            }
        }

        private DeferredCallbackQueue<T> deferredCallbackQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="PushToPoolCallback{T}"/> class with a specific root pool.
        /// </summary>
        /// <param name="root">The root pool that elements will be pushed into.</param>
        public PushToPoolCallback(INonAllocPool<T> root = null)
        {
            this.root = root;

            deferredCallbackQueue = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PushToPoolCallback{T}"/> class with a specific deferred callback queue.
        /// </summary>
        /// <param name="deferredCallbackQueue">The deferred callback queue that the callback should be associated with.</param>
        public PushToPoolCallback(DeferredCallbackQueue<T> deferredCallbackQueue)
        {
            root = null;

            this.deferredCallbackQueue = deferredCallbackQueue;

            this.deferredCallbackQueue.Callback = this;
        }

        /// <summary>
        /// Method called when an element is allocated from a pool.
        /// </summary>
        /// <param name="currentElement">The allocated element.</param>
        public void OnAllocated(IPoolElement<T> currentElement)
        {
            //SUPPLY AND MERGE POOLS DO NOT PRODUCE ELEMENTS WITH VALUES
            //if (currentElement.Value == null)
            //    return;

            // If the root pool is not set, enqueue the allocated element in the deferred callback queue.
            if (root == null)
            {
                deferredCallbackQueue?.Enqueue(currentElement);

                return;
            }

            // Push the allocated element into the root pool.
            root.Push(currentElement);
        }
    }
}