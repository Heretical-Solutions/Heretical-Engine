namespace HereticalSolutions.Time
{
    /// <summary>
    /// Represents the state of a timer
    /// </summary>
    public enum ETimerState
    {
        /// <summary>
        /// The timer is inactive
        /// </summary>
        INACTIVE,

        /// <summary>
        /// The timer has been started
        /// </summary>
        STARTED,

        /// <summary>
        /// The timer has been paused
        /// </summary>
        PAUSED,

        /// <summary>
        /// The timer has finished
        /// </summary>
        FINISHED
    }
}