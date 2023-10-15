namespace HereticalSolutions.Pools
{
    /// <summary>
    /// Defines an interface for an object that contains an address.
    /// </summary>
    public interface IContainsAddress
    {
        /// <summary>
        /// Gets the full address of the object.
        /// </summary>
        string FullAddress { get; }

        /// <summary>
        /// Gets an array of hash codes generated from the address.
        /// </summary>
        int[] AddressHashes { get; }
    }
}