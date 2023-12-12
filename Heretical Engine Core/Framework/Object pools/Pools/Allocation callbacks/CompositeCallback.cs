namespace HereticalSolutions.Pools.AllocationCallbacks
{
    /// <summary>
    /// Represents a composite allocation callback that combines multiple callbacks.
    /// </summary>
    /// <typeparam name="T">The type of the pool elements.</typeparam>
    public class CompositeCallback<T> : IAllocationCallback<T>
    {
        private readonly IAllocationCallback<T>[] callbacks;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeCallback{T}"/> class.
        /// </summary>
        /// <param name="callbacks">The array of allocation callbacks to be combined.</param>
        public CompositeCallback(IAllocationCallback<T>[] callbacks)
        {
            this.callbacks = callbacks;
        }

        /// <summary>
        /// Calls the OnAllocated method of each allocation callback in the composite.
        /// </summary>
        /// <param name="element">The allocated pool element.</param>
        public void OnAllocated(IPoolElement<T> element)
        {
            if (callbacks != null)
                foreach (var processor in callbacks)
                    processor.OnAllocated(element);
        }
    }
}