using System;

using HereticalSolutions.Delegates;

namespace HereticalSolutions.Time
{
    public interface IPersistentTimerContext : ITimerWithState
    {
        /// <summary>
        /// Gets or sets the start time of the timer.
        /// </summary>
        DateTime StartTime { get; set; }
        
        /// <summary>
        /// Gets or sets the estimated finish time of the timer.
        /// </summary>
        DateTime EstimatedFinishTime { get; set; }

        /// <summary>
        /// Gets or sets the saved progress of the timer.
        /// </summary>
        TimeSpan SavedProgress { get; set; }
        
        /// <summary>
        /// Gets or sets the current duration of the timer in the form of a <see cref="TimeSpan"/>.
        /// </summary>
        TimeSpan CurrentDurationSpan { get; set; }
        
        /// <summary>
        /// Gets or sets the default duration of the timer in the form of a <see cref="TimeSpan"/>.
        /// </summary>
        TimeSpan DefaultDurationSpan { get; set; }
        
        /// <summary>
        /// Gets a value indicating whether the timer should accumulate progress.
        /// </summary>
        bool Accumulate { get; }

        /// <summary>
        /// Gets a value indicating whether the timer should repeat after finishing.
        /// </summary>
        bool Repeat { get; }
        
        /// <summary>
        /// Gets the publisher for the OnStart event of the timer with a single argument of type <see cref="IPersistentTimer"/>.
        /// </summary>
        IPublisherSingleArgGeneric<IPersistentTimer> OnStartAsPublisher { get; }
        
        /// <summary>
        /// Gets the publisher for the OnFinish event of the timer with a single argument of type <see cref="IPersistentTimer"/>.
        /// </summary>
        IPublisherSingleArgGeneric<IPersistentTimer> OnFinishAsPublisher { get; }
    }
}