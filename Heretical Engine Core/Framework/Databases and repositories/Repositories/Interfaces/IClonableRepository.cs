namespace HereticalSolutions.Repositories
{
    /// <summary>
    /// Represents a repository that can be cloned.
    /// </summary>
    /// <typeparam name="TKey">The type of the repository's keys.</typeparam>
    /// <typeparam name="TValue">The type of the repository's values.</typeparam>
    public interface IClonableRepository<TKey, TValue>
    {
        /// <summary>
        /// Creates a clone of the repository.
        /// </summary>
        /// <returns>A clone of the repository.</returns>
        IRepository<TKey, TValue> Clone();
    }
}