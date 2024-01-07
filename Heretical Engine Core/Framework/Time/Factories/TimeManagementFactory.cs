using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Synchronization;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Time.Factories
{
    public static partial class TimeFactory
    {
        public const string APPLICATION_RUNTIME_TIMER_ID = "Application runtime timer";

        public const string APPLICATION_PERSISTENT_TIMER_ID = "Application persistent timer";

        public static TimeManager BuildTimeManager(
            IFormatLogger logger)
        {
            var applicationActiveTimer = TimeFactory.BuildRuntimeTimer(
                APPLICATION_RUNTIME_TIMER_ID,
                0f,
                logger);

            applicationActiveTimer.Accumulate = true;

            applicationActiveTimer.Start();

            var applicationPersistentTimer = TimeFactory.BuildPersistentTimer(
                APPLICATION_PERSISTENT_TIMER_ID,
                default,
                logger);

            applicationActiveTimer.Accumulate = true;

            applicationPersistentTimer.Start();

            return new TimeManager(
                RepositoriesFactory.BuildDictionaryRepository<string, ISynchronizableGenericArg<float>>(),
                applicationActiveTimer,
                applicationPersistentTimer);
        }
    }
}