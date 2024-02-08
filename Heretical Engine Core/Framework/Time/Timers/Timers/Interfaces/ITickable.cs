namespace HereticalSolutions.Time
{
    /// <summary>
    /// Interface for objects that need to receive game ticks
    /// </summary>
    public interface ITickable
    {
        /// <summary>
        /// Called each game frame to update the object
        /// </summary>
        /// <param name="delta">The time in seconds since the last frame.</param>
        void Tick(float delta);
    }
}