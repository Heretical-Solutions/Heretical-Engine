namespace HereticalSolutions.Repositories
{
    /// <summary>
    /// Represents an interface for a clonable object repository.
    /// </summary>
    public interface ICloneableObjectRepository
    {
        /// <summary>
        /// Creates a clone of the object repository.
        /// </summary>
        /// <returns>A clone of the object repository as an <see cref="IObjectRepository"/> object.</returns>
        IObjectRepository Clone();
    }
}