using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Time;
using HereticalSolutions.Time.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.AllocationCallbacks
{
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

        private IFormatLogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetRuntimeTimerCallback{T}"/> class with the specified ID and default duration.
        /// </summary>
        /// <param name="id">The ID of the runtime timer.</param>
        /// <param name="defaultDuration">The default duration for the runtime timer.</param>
        public SetRuntimeTimerCallback(
            string id = "Anonymous Timer",
            float defaultDuration = 0f,
            IFormatLogger logger = null)
        {
            ID = id;

            DefaultDuration = defaultDuration;

            this.logger = logger;
        }

        public void OnAllocated(IPoolElement<T> currentElement)
        {
            //SUPPLY AND MERGE POOLS DO NOT PRODUCE ELEMENTS WITH VALUES
            //if (currentElement.Value == null)
            //    return;

            var metadata = (RuntimeTimerMetadata)currentElement.Metadata.Get<IContainsRuntimeTimer>();

            // Set the runtime timer
            metadata.RuntimeTimer = TimeFactory.BuildRuntimeTimer(
                ID,
                DefaultDuration,
                logger);

            metadata.RuntimeTimerAsTickable = (ITickable)metadata.RuntimeTimer;

            // Subscribe to the runtime timer's tick event
            metadata.UpdateSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<float>(
                metadata.RuntimeTimerAsTickable.Tick,
                logger);

            Action<IRuntimeTimer> pushDelegate = (timer) => { currentElement.Push(); };

            metadata.PushSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<IRuntimeTimer>(
                pushDelegate,
                logger);
        }
    }
}