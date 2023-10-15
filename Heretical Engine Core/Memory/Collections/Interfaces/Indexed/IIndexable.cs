namespace HereticalSolutions.Collections
{
    /// <summary>
    /// Represents an indexable collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public interface IIndexable<T>
    {
        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The index of the element to retrieve.</param>
        /// <returns>The element at the specified index.</returns>
        T this[int index] { get; }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The index of the element to retrieve.</param>
        /// <returns>The element at the specified index.</returns>
        T Get(int index);
    }
}