using HereticalSolutions.Repositories;

namespace HereticalSolutions.Pools
{
    /// <summary>
    /// Represents an element in a pool.
    /// </summary>
    /// <typeparam name="T">The type of the pool element.</typeparam>
    public interface IPoolElement<T>
    {
        /// <summary>
        /// Gets or sets the value of the pool element.
        /// </summary>
        T Value { get; set; }

        /// <summary>
        /// Gets the status of the pool element.
        /// </summary>
        EPoolElementStatus Status { get; }

        /// <summary>
        /// Gets the metadata of the pool element.
        /// </summary>
        IReadOnlyObjectRepository Metadata { get; }

        /// <summary>
        /// Pushes the pool element.
        /// </summary>
        void Push();
    }
}