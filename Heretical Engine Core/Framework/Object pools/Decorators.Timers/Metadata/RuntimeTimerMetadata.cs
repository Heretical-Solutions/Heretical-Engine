using HereticalSolutions.Delegates.Subscriptions;
using HereticalSolutions.Time;

namespace HereticalSolutions.Pools
{
    /// <summary>
    /// Represents the metadata associated with a runtime timer.
    /// </summary>
    public class RuntimeTimerMetadata : IContainsRuntimeTimer
    {
        /// <summary>
        /// Gets or sets the runtime timer.
        /// </summary>
        public IRuntimeTimer RuntimeTimer { get; set; }
        
        /// <summary>
        /// Gets or sets the runtime timer as a tickable object.
        /// </summary>
        public ITickable RuntimeTimerAsTickable { get; set; }

        /// <summary>
        /// Gets or sets the subscription for updating the runtime timer.
        /// </summary>
        public SubscriptionSingleArgGeneric<float> UpdateSubscription { get; set; }
        
        /// <summary>
        /// Gets or sets the subscription for pushing the runtime timer.
        /// </summary>
        public SubscriptionSingleArgGeneric<IRuntimeTimer> PushSubscription { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeTimerMetadata"/> class.
        /// </summary>
        public RuntimeTimerMetadata()
        {
            RuntimeTimer = null;
            RuntimeTimerAsTickable = null;
            UpdateSubscription = null;
            PushSubscription = null;
        }
    }
}