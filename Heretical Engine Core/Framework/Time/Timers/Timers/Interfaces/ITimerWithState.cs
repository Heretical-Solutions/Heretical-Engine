namespace HereticalSolutions.Time
{
    /// <summary>
    /// Interface for a timer that can change its state
    /// </summary>
    public interface ITimerWithState
    {
        /// <summary>
        /// Sets the state of the timer
        /// </summary>
        /// <param name="state">The new state of the timer.</param>
        void SetState(ETimerState state);
    }
}