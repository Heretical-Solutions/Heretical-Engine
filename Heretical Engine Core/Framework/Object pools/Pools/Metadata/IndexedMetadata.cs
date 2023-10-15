using HereticalSolutions.Collections;

namespace HereticalSolutions.Pools
{
    /// <summary>
    /// Represents indexed metadata.
    /// </summary>
    public class IndexedMetadata : IIndexed
    {
        /// <summary>
        /// Gets or sets the index value.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexedMetadata"/> class.
        /// </summary>
        public IndexedMetadata()
        {
            Index = -1;
        }
    }
}