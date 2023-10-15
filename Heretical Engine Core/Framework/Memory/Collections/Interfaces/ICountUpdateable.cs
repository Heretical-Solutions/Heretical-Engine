namespace HereticalSolutions.Collections
{
    /// <summary>
    /// Represents an object that can be updated with a new count.
    /// </summary>
    public interface ICountUpdateable
    {
        /// <summary>
        /// Updates the count with a new value.
        /// </summary>
        /// <param name="newCount">The new count value.</param>
        void UpdateCount(int newCount);
    }
}