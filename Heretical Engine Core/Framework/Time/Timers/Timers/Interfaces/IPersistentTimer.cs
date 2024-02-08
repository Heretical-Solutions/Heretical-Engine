using System;

using HereticalSolutions.Delegates;

namespace HereticalSolutions.Time
{
    /// <summary>
    /// Represents a persistent timer that can start and reset based on a given duration
    /// </summary>
    public interface IPersistentTimer
        : ITimer
    {
        #region Timer state

        /// <summary>
        /// Gets the start time of the timer
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// Gets the estimated finish time of the timer
        /// </summary>
        DateTime EstimatedFinishTime { get; }

        #endregion

        #region Countdown and Time elapsed

        /// <summary>
        /// Gets the time elapsed since the timer started
        /// </summary>
        TimeSpan TimeElapsedSpan { get; }
        
        /// <summary>
        /// Gets the remaining countdown time
        /// </summary>
        TimeSpan CountdownSpan { get; }

        #endregion

        #region Duration

        /// <summary>
        /// Gets the current duration of the timer
        /// </summary>
        TimeSpan CurrentDurationSpan { get; }

        /// <summary>
        /// Gets the default duration of the timer
        /// </summary>
        TimeSpan DefaultDurationSpan { get; }

        #endregion

        #region Controls

        /// <summary>
        /// Resets the timer with the specified duration
        /// </summary>
        /// <param name="duration">The duration for the timer.</param>
        void Reset(TimeSpan duration);

        /// <summary>
        /// Starts the timer with the specified duration
        /// </summary>
        /// <param name="duration">The duration for the timer.</param>
        void Start(TimeSpan duration);

        #endregion
        
        #region Callbacks

        /// <summary>
        /// Event that is triggered when the timer starts
        /// </summary>
        INonAllocSubscribableSingleArgGeneric<IPersistentTimer> OnStart { get; }
        
        /// <summary>
        /// Event that is triggered when the timer finishes
        /// </summary>
        INonAllocSubscribableSingleArgGeneric<IPersistentTimer> OnFinish { get; }

        #endregion
    }
}