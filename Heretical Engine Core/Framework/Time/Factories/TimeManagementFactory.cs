using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Synchronization;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Time.Factories
{
    public static partial class TimeFactory
    {
        public static TimeManager BuildTimeManager(
            IFormatLogger logger)
        {
            var applicationActiveTimer = TimeFactory.BuildRuntimeTimer(
                "Application active",
                0f,
                logger);

            applicationActiveTimer.Accumulate = true;

            applicationActiveTimer.Start();

            var applicationPersistentTimer = TimeFactory.BuildPersistentTimer(
                "Application persistent",
                default,
                logger);

            applicationActiveTimer.Accumulate = true;

            applicationPersistentTimer.Start();

            return new TimeManager(
                RepositoriesFactory.BuildDictionaryRepository<string, ISynchronizableGeneric<float>>(),
                applicationActiveTimer,
                applicationPersistentTimer);
        }
    }
}