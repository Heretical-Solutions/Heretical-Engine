namespace HereticalSolutions.Pools.Arguments
{
    /// <summary>
    /// Represents an argument for decorating a pool with an address.
    /// </summary>
    public class AddressArgument : IPoolDecoratorArgument
    {
        /// <summary>
        /// Gets or sets the full address.
        /// </summary>
        public string FullAddress;

        /// <summary>
        /// Gets or sets the array of address hashes.
        /// </summary>
        public int[] AddressHashes;
    }
}