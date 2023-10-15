namespace HereticalSolutions.Repositories
{
    /// <summary>
    /// Represents an interface for a clonable repository.
    /// </summary>
    /// <typeparam name="TKey">The type of the repository key.</typeparam>
    /// <typeparam name="TValue">The type of the repository value.</typeparam>
    public interface IClonableRepository<TKey, TValue>
    {
        /// <summary>
        /// Creates a clone of the repository.
        /// </summary>
        /// <returns>A clone of the repository as an <see cref="IRepository{TKey, TValue}"/> object.</returns>
        IRepository<TKey, TValue> Clone();
    }
}