using System.Collections.Generic;

namespace HereticalSolutions.Pools.AllocationCallbacks
{
    /// <summary>
    /// Represents a queue for deferred callback execution.
    /// </summary>
    /// <typeparam name="T">The type of elements in the queue.</typeparam>
    public class DeferredCallbackQueue<T>
    {
        private Queue<IPoolElement<T>> elementsQueue;

        /// <summary>
        /// Gets or sets the allocation callback.
        /// </summary>
        public IAllocationCallback<T> Callback { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeferredCallbackQueue{T}"/> class.
        /// </summary>
        public DeferredCallbackQueue()
        {
            elementsQueue = new Queue<IPoolElement<T>>();
        }

        /// <summary>
        /// Enqueues an element to the queue.
        /// </summary>
        /// <param name="element">The element to enqueue.</param>
        public void Enqueue(IPoolElement<T> element)
        {
            elementsQueue.Enqueue(element);
        }

        /// <summary>
        /// Processes the elements in the queue and executes the allocation callback.
        /// </summary>
        public void Process()
        {
            while (elementsQueue.Count > 0)
            {
                Callback.OnAllocated(elementsQueue.Dequeue());
            }
        }
    }
}