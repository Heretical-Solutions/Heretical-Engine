using HereticalSolutions.Delegates;

namespace HereticalSolutions.Time
{
    /// <summary>
    /// Represents the runtime timer context
    /// </summary>
    public interface IRuntimeTimerContext
        : ITimerWithState
    {
        #region Variables
        
        float CurrentTimeElapsed { get; set; }

        #endregion
        
        #region Duration
        
        float CurrentDuration { get; set; }

        float DefaultDuration { get; }

        #endregion
        
        #region Controls
        
        bool Accumulate { get; }

        bool Repeat { get; }

        bool FlushTimeElapsedOnRepeat { get; }

        #endregion

        #region Publishers
        
        IPublisherSingleArgGeneric<IRuntimeTimer> OnStartAsPublisher { get; }
        
        IPublisherSingleArgGeneric<IRuntimeTimer> OnFinishAsPublisher { get; }
        
        #endregion
    }
}