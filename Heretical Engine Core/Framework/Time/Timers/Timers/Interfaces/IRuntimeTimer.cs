using HereticalSolutions.Delegates;

namespace HereticalSolutions.Time
{
    /// <summary>
    /// Represents an interface for a timer that runs during runtime
    /// </summary>
    public interface IRuntimeTimer
        : ITimer
    {
        #region Countdown and Time elapsed

        /// <summary>
        /// Gets the time elapsed since the timer started
        /// </summary>
        float TimeElapsed { get; }
        
        /// <summary>
        /// Gets the remaining time until the timer finishes
        /// </summary>
        float Countdown { get; }

        #endregion

        #region Duration

        /// <summary>
        /// Gets or sets the current duration of the timer
        /// </summary>
        float CurrentDuration { get; }

        /// <summary>
        /// Gets the default duration of the timer
        /// </summary>
        float DefaultDuration { get; }

        #endregion

        #region Controls

        /// <summary>
        /// Resets the timer with the specified duration
        /// </summary>
        /// <param name="duration">The duration to set for the timer.</param>
        void Reset(float duration);

        /// <summary>
        /// Starts the timer with the specified duration
        /// </summary>
        /// <param name="duration">The duration to set for the timer.</param>
        void Start(float duration);

        #endregion
        
        #region Callbacks

        /// <summary>
        /// Event that is triggered when the timer starts
        /// </summary>
        INonAllocSubscribableSingleArgGeneric<IRuntimeTimer> OnStart { get; }
        
        /// <summary>
        /// Event that is triggered when the timer finishes
        /// </summary>
        INonAllocSubscribableSingleArgGeneric<IRuntimeTimer> OnFinish { get; }

        #endregion
    }
}