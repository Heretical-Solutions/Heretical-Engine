namespace HereticalSolutions.StateMachines
{
    /// <summary>
    /// Enumeration for the states of an asynchronous transition
    /// </summary>
    public enum EAsyncTransitionState
    {
        /// <summary>
        /// The transition is queued
        /// </summary>
        QUEUED,

        /// <summary>
        /// The transition is in progress
        /// </summary>
        IN_PROGRESS,

        /// <summary>
        /// The transition was aborted
        /// </summary>
        ABORTED,

        /// <summary>
        /// The transition has completed
        /// </summary>
        COMPLETED
    }
}