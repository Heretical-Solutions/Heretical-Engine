using HereticalSolutions.Pools.AllocationCallbacks;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Factory class for creating address decorators pools.
    /// </summary>
    public static partial class AddressDecoratorsPoolsFactory
    {
        #region Allocation callbacks

        /// <summary>
        /// Builds a set address callback.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="fullAddress">The full address for the object.</param>
        /// <param name="addressHashes">The array of address hashes.</param>
        /// <returns>The set address callback.</returns>
        public static SetAddressCallback<T> BuildSetAddressCallback<T>(
            string fullAddress = null,
            int[] addressHashes = null)
        {
            return new SetAddressCallback<T>(fullAddress, addressHashes);
        }
        
        #endregion
    }
}