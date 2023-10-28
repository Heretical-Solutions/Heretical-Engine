namespace HereticalSolutions.Pools.AllocationCallbacks
{
    /// <summary>
    /// Callback implementation for pushing allocated elements to a decorated pool.
    /// </summary>
    /// <typeparam name="T">The type of elements in the pool.</typeparam>
    public class PushToDecoratedPoolCallback<T> : IAllocationCallback<T>
    {
        private INonAllocDecoratedPool<T> root;

        /// <summary>
        /// Gets or sets the root pool to push the allocated elements.
        /// </summary>
        public INonAllocDecoratedPool<T> Root
        {
            get => root;
            set
            {
                root = value;

                if (deferredCallbackQueue != null)
                {
                    deferredCallbackQueue.Process();

                    deferredCallbackQueue.Callback = null;

                    deferredCallbackQueue = null;
                }
            }
        }

        //TODO: eliminate. Let the DryRun's go without this shit
        private DeferredCallbackQueue<T> deferredCallbackQueue;

        /// <summary>
        /// Initializes a new instance of the PushToDecoratedPoolCallback class.
        /// </summary>
        /// <param name="root">The root pool to push the allocated elements.</param>
        public PushToDecoratedPoolCallback(INonAllocDecoratedPool<T> root = null)
        {
            this.root = root;

            deferredCallbackQueue = null;
        }

        /// <summary>
        /// Initializes a new instance of the PushToDecoratedPoolCallback class.
        /// </summary>
        /// <param name="deferredCallbackQueue">The deferred callback queue.</param>
        public PushToDecoratedPoolCallback(DeferredCallbackQueue<T> deferredCallbackQueue)
        {
            root = null;

            this.deferredCallbackQueue = deferredCallbackQueue;

            this.deferredCallbackQueue.Callback = this;
        }

        /// <summary>
        /// Called when an element is allocated.
        /// </summary>
        /// <param name="currentElement">The currently allocated element.</param>
        public void OnAllocated(IPoolElement<T> currentElement)
        {
            if (root == null)
            {
                deferredCallbackQueue?.Enqueue(currentElement);

                return;
            }

            root.Push(currentElement, true);
        }
    }
}