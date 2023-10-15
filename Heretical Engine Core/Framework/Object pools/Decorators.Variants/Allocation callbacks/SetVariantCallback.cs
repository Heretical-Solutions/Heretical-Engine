using HereticalSolutions.Pools.Elements;

namespace HereticalSolutions.Pools.AllocationCallbacks
{
    /// <summary>
    /// Represents a callback for setting the variant of an allocated object in a pool.
    /// </summary>
    /// <typeparam name="T">The type of object in the pool.</typeparam>
    public class SetVariantCallback<T> : IAllocationCallback<T>
    {
        /// <summary>
        /// Gets or sets the variant value to set.
        /// </summary>
        public int Variant { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetVariantCallback{T}"/> class with a specified variant value.
        /// </summary>
        /// <param name="variant">The variant value to set. Default is -1.</param>
        public SetVariantCallback(int variant = -1)
        {
            Variant = variant;
        }

        /// <summary>
        /// Performs the action of setting the variant of an allocated object in a pool.
        /// </summary>
        /// <param name="currentElement">The currently allocated object.</param>
        public void OnAllocated(IPoolElement<T> currentElement)
        {
            //SUPPLY AND MERGE POOLS DO NOT PRODUCE ELEMENTS WITH VALUES
            //if (currentElement.Value == null)
            //    return;

            ((VariantMetadata)currentElement.Metadata.Get<IContainsVariant>()).Variant = Variant;
        }
    }
}