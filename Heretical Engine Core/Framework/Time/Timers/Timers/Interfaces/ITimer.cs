namespace HereticalSolutions.Time
{
    /// <summary>
    /// Represents a timer.
    /// </summary>
    public interface ITimer
    {
        /// <summary>
        /// Gets the unique identifier of the timer.
        /// </summary>
        string ID { get; }

        #region Timer state

        /// <summary>
        /// Gets the current state of the timer.
        /// </summary>
        ETimerState State { get; }

        #endregion

        #region Progress

        /// <summary>
        /// Gets the progress of the timer (a value between 0 and 1).
        /// </summary>
        float Progress { get; }

        #endregion

        #region Controls

        /// <summary>
        /// Gets or sets a value indicating whether the timer works as an accumulative counter of time.
        /// </summary>
        bool Accumulate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the timer starts another cycle and fires callbacks when it times out.
        /// </summary>
        bool Repeat { get; set; }

        /// <summary>
        /// Resets the timer to its initial state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Starts the timer.
        /// </summary>
        void Start();

        /// <summary>
        /// Pauses the timer.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes the timer.
        /// </summary>
        void Resume();

        /// <summary>
        /// Aborts the timer.
        /// </summary>
        void Abort();

        /// <summary>
        /// Finishes the timer operation prematurely.
        /// </summary>
        void Finish();

        #endregion
    }
}