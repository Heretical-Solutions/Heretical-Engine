namespace HereticalSolutions.Collections
{
    /// <summary>
    /// Represents a modifiable collection with contents of type <typeparamref name="TCollection"/>.
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection contents.</typeparam>
    public interface IModifiable<TCollection>
    {
        /// <summary>
        /// Gets or sets the contents of the collection.
        /// </summary>
        TCollection Contents { get; }

        /// <summary>
        /// Updates the contents of the collection.
        /// </summary>
        /// <param name="newContents">The new contents of the collection.</param>
        void UpdateContents(TCollection newContents);
    }
}