namespace HereticalSolutions.Pools
{
    /// <summary>
    /// Represents metadata for a variant.
    /// </summary>
    public class VariantMetadata : IContainsVariant
    {
        /// <summary>
        /// Gets or sets the variant value.
        /// </summary>
        public int Variant { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariantMetadata"/> class.
        /// </summary>
        public VariantMetadata()
        {
            Variant = -1;
        }
    }
}