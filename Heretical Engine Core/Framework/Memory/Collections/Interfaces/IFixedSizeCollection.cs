namespace HereticalSolutions.Collections
{
    /// <summary>
    /// Represents a fixed-size collection of elements of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public interface IFixedSizeCollection<T>
    {
        /// <summary>
        /// Gets the capacity of the collection, i.e., the maximum number of elements it can hold.
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Gets the element at the specified index in the collection.
        /// </summary>
        /// <param name="index">The index of the element to retrieve.</param>
        /// <returns>The element at the specified index.</returns>
        T ElementAt(int index);
    }
}