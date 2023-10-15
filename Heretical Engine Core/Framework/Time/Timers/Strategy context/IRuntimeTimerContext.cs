using HereticalSolutions.Delegates;

namespace HereticalSolutions.Time
{
    /// <summary>
    /// Represents the runtime timer context.
    /// </summary>
    public interface IRuntimeTimerContext : ITimerWithState
    {
        #region Variables
        
        /// <summary>
        /// The amount of time elapsed since the timer started.
        /// </summary>
        float CurrentTimeElapsed { get; set; }

        #endregion
        
        #region Duration
        
        /// <summary>
        /// The current duration of the timer.
        /// </summary>
        float CurrentDuration { get; set; }

        /// <summary>
        /// The default duration of the timer.
        /// </summary>
        float DefaultDuration { get; }

        #endregion
        
        #region Controls
        
        /// <summary>
        /// Indicates whether the timer should accumulate time beyond the defined duration.
        /// </summary>
        bool Accumulate { get; }

        /// <summary>
        /// Indicates whether the timer should repeat after completing the defined duration.
        /// </summary>
        bool Repeat { get; }

        #endregion

        #region Publishers
        
        /// <summary>
        /// Publisher for the Start event of the timer.
        /// </summary>
        IPublisherSingleArgGeneric<IRuntimeTimer> OnStartAsPublisher { get; }
        
        /// <summary>
        /// Publisher for the Finish event of the timer.
        /// </summary>
        IPublisherSingleArgGeneric<IRuntimeTimer> OnFinishAsPublisher { get; }
        
        #endregion
    }
}