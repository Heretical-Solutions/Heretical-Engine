namespace HereticalSolutions.Time
{
    /// <summary>
    /// Represents an object that can be synchronized.
    /// </summary>
    public interface ISynchronizable
    {
        /// <summary>
        /// Synchronizes the object using the specified delta time.
        /// </summary>
        /// <param name="delta">The time interval since the last synchronization.</param>
        void Synchronize(float delta);
    }
}