using HereticalSolutions.Delegates.Subscriptions;
using HereticalSolutions.Time;

namespace HereticalSolutions.Pools
{
    /// <summary>
    /// Represents an interface for an object that contains a runtime timer.
    /// </summary>
    public interface IContainsRuntimeTimer
    {
        /// <summary>
        /// Gets the runtime timer associated with this object.
        /// </summary>
        IRuntimeTimer RuntimeTimer { get; }

        /// <summary>
        /// Gets the runtime timer as a tickable object.
        /// </summary>
        ITickable RuntimeTimerAsTickable { get; }

        /// <summary>
        /// Gets the subscription that is triggered when the update occurs.
        /// </summary>
        SubscriptionSingleArgGeneric<float> UpdateSubscription { get; }

        /// <summary>
        /// Gets the subscription that is triggered when the runtime timer is pushed.
        /// </summary>
        SubscriptionSingleArgGeneric<IRuntimeTimer> PushSubscription { get; }
    }
}