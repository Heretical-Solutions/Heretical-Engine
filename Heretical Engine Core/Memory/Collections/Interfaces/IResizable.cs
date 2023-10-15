using HereticalSolutions.Allocations;

namespace HereticalSolutions.Collections
{
    /// <summary>
    /// Represents a resizable collection of items of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public interface IResizable<T>
    {
        /// <summary>
        /// Gets the allocation command for resizing the collection.
        /// </summary>
        /// <returns>The allocation command for resizing the collection.</returns>
        AllocationCommand<T> ResizeAllocationCommand { get; }

        /// <summary>
        /// Resizes the collection using the specified allocation command.
        /// </summary>
        void Resize();
    }
}