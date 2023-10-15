using HereticalSolutions.Allocations;

namespace HereticalSolutions.Collections
{
    /// <summary>
    /// Represents a collection that supports appending elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public interface IAppendable<T>
    {
        /// <summary>
        /// Gets the allocation command used for appending elements.
        /// </summary>
        AllocationCommand<T> AppendAllocationCommand { get; }

        /// <summary>
        /// Appends an element to the collection.
        /// </summary>
        /// <returns>The appended element.</returns>
        T Append();
    }
}