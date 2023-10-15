namespace HereticalSolutions.Collections
{
    /// <summary>
    /// Represents an object that can be indexed.
    /// </summary>
    public interface IIndexed
    {
        /// <summary>
        /// Gets or sets the index of the object.
        /// </summary>
        int Index { get; set; }
    }
}