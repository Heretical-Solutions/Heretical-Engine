namespace HereticalSolutions.Pools
{
    /// <summary>
    /// Represents metadata associated with an address.
    /// </summary>
    public class AddressMetadata : IContainsAddress
    {
        /// <summary>
        /// Gets or sets the full address.
        /// </summary>
        public string FullAddress { get; set; }

        /// <summary>
        /// Gets or sets the array of address hashes.
        /// </summary>
        public int[] AddressHashes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressMetadata"/> class.
        /// </summary>
        public AddressMetadata()
        {
            FullAddress = string.Empty;
            AddressHashes = new int[0];
        }
    }
}