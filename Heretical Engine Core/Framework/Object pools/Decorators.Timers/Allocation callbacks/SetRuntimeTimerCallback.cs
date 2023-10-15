using System;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Time;
using HereticalSolutions.Time.Factories;

namespace HereticalSolutions.Pools.AllocationCallbacks
{
    /// <summary>
    /// Represents an allocation callback that sets a runtime timer for each allocated item in a pool.
    /// </summary>
    /// <typeparam name="T">The type of the pool elements.</typeparam>
    public class SetRuntimeTimerCallback<T> : IAllocationCallback<T>
    {
        /// <summary>
        /// Gets or sets the ID of the runtime timer.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the default duration for the runtime timer.
        /// </summary>
        public float DefaultDuration { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetRuntimeTimerCallback{T}"/> class with the specified ID and default duration.
        /// </summary>
        /// <param name="id">The ID of the runtime timer.</param>
        /// <param name="defaultDuration">The default duration for the runtime timer.</param>
        public SetRuntimeTimerCallback(
            string id = "Anonymous Timer",
            float defaultDuration = 0f)
        {
            ID = id;
            DefaultDuration = defaultDuration;
        }

        /// <summary>
        /// Sets the runtime timer for the allocated item in the pool when it is allocated.
        /// </summary>
        /// <param name="currentElement">The currently allocated element in the pool.</param>
        public void OnAllocated(IPoolElement<T> currentElement)
        {
            //SUPPLY AND MERGE POOLS DO NOT PRODUCE ELEMENTS WITH VALUES
            //if (currentElement.Value == null)
            //    return;

            var metadata = (RuntimeTimerMetadata)currentElement.Metadata.Get<IContainsRuntimeTimer>();

            // Set the runtime timer
            metadata.RuntimeTimer = TimeFactory.BuildRuntimeTimer(
                ID,
                DefaultDuration);

            metadata.RuntimeTimerAsTickable = (ITickable)metadata.RuntimeTimer;

            // Subscribe to the runtime timer's tick event
            metadata.UpdateSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<float>(metadata.RuntimeTimerAsTickable.Tick);

            Action<IRuntimeTimer> pushDelegate = (timer) => { currentElement.Push(); };

            metadata.PushSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<IRuntimeTimer>(pushDelegate);
        }
    }
}