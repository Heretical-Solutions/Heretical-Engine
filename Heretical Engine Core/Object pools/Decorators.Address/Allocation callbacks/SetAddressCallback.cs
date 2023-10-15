using HereticalSolutions.Pools.Elements;

namespace HereticalSolutions.Pools.AllocationCallbacks
{
    /// <summary>
    /// Represents a callback for setting the address of an allocated pool element.
    /// </summary>
    /// <typeparam name="T">The type of the pool element.</typeparam>
    public class SetAddressCallback<T> : IAllocationCallback<T>
    {
        /// <summary>
        /// Gets or sets the full address of the allocated pool element.
        /// </summary>
        public string FullAddress { get; set; }

        /// <summary>
        /// Gets or sets the array of address hashes of the allocated pool element.
        /// </summary>
        public int[] AddressHashes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetAddressCallback{T}"/> class.
        /// </summary>
        /// <param name="fullAddress">The full address of the allocated pool element.</param>
        /// <param name="addressHashes">The array of address hashes of the allocated pool element.</param>
        public SetAddressCallback(string fullAddress = null, int[] addressHashes = null)
        {
            FullAddress = fullAddress;
            AddressHashes = addressHashes;
        }

        /// <summary>
        /// Called when a pool element is allocated.
        /// </summary>
        /// <param name="currentElement">The current pool element.</param>
        public void OnAllocated(IPoolElement<T> currentElement)
        {
            //SUPPLY AND MERGE POOLS DO NOT PRODUCE ELEMENTS WITH VALUES
            //if (currentElement.Value == null)
            //    return;

            if (FullAddress == null || AddressHashes == null)
                return;

            var addressMetadata = (AddressMetadata)currentElement.Metadata.Get<IContainsAddress>();

            addressMetadata.FullAddress = FullAddress;
            addressMetadata.AddressHashes = AddressHashes;
        }
    }
}