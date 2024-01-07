using HereticalSolutions.Delegates;

namespace HereticalSolutions.Time
{
    public interface IPersistentTimerContext
        : ITimerWithState
    {
        DateTime StartTime { get; set; }
        
        DateTime EstimatedFinishTime { get; set; }

        TimeSpan SavedProgress { get; set; }
        
        TimeSpan CurrentDurationSpan { get; set; }
        
        TimeSpan DefaultDurationSpan { get; set; }
        
        bool Accumulate { get; }

        bool Repeat { get; }

        bool FlushTimeElapsedOnRepeat { get; }

        IPublisherSingleArgGeneric<IPersistentTimer> OnStartAsPublisher { get; }
        
        IPublisherSingleArgGeneric<IPersistentTimer> OnFinishAsPublisher { get; }
    }
}