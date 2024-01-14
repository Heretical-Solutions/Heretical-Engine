using HereticalSolutions.Persistence.Arguments;

using HereticalSolutions.Persistence.IO;

using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Logging.Factories
{
    /// <summary>
    /// Class for creating instances of loggers.
    /// </summary>
    public static class LoggersFactory
    {
        public static ConsoleLogger BuildDefaultLogger()
        {
            return new ConsoleLogger();
        }

        public static SingleLoggerBuilder BuildDefaultLoggerBuilder(
            IFormatLogger logger = null)
        {
            return new SingleLoggerBuilder(
                logger,
                RepositoriesFactory.BuildDictionaryRepository<Type, bool>(),
                true);
        }

        public static ConsoleLoggerWithFileDump BuildDefaultLoggerWithFileDump(
            string applicationDataFolder,
            string relativePath)
        {
            var serializationArgument = new TextFileArgument();

            serializationArgument.Settings = new FilePathSettings
            {
                RelativePath = relativePath,
                ApplicationDataFolder = applicationDataFolder
            };

            var result = new ConsoleLoggerWithFileDump(
                new List<string>());

            result.Initialize(
                serializationArgument,
                PersistenceFactory.BuildSimplePlainTextSerializer(null)); //TODO: refactor

            return result;
        }
    }
}