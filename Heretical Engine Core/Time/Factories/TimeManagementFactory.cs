using HereticalSolutions.Delegates.Factories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Time.Factories
{
    /// <summary>
    /// Represents a factory for creating time-related objects.
    /// </summary>
    public static partial class TimeFactory
    {
        /// <summary>
        /// Builds a new <see cref="TimeManager"/> object.
        /// </summary>
        /// <returns>A new instance of <see cref="TimeManager"/>.</returns>
        public static TimeManager BuildTimeManager()
        {
            return new TimeManager(
                RepositoriesFactory.BuildDictionaryRepository<string, ISynchronizable>());
        }

        /// <summary>
        /// Builds a new <see cref="SynchronizationContext"/> object.
        /// </summary>
        /// <param name="id">The identifier for the synchronization context.</param>
        /// <param name="canBeToggled">A value indicating if the synchronization context can be toggled.</param>
        /// <param name="canScale">A value indicating if the synchronization context can be scaled.</param>
        /// <returns>A new instance of <see cref="SynchronizationContext"/>.</returns>
        public static SynchronizationContext BuildSynchronizationContext(
            string id,
            bool canBeToggled,
            bool canScale)
        {
            var broadcaster = DelegatesFactory.BuildNonAllocBroadcasterGeneric<float>();

            return new SynchronizationContext(
                new SynchronizationContextDescriptor(
                    id,
                    canBeToggled,
                    canScale),
                broadcaster,
                broadcaster);
        }
    }
}