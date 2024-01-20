using HereticalSolutions.Persistence.Arguments;

using HereticalSolutions.Persistence.IO;

using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Logging.Factories
{
    public static class LoggersFactory
    {
        public static LoggerBuilder BuildLoggerBuilder()
        {
            return new LoggerBuilder(
                RepositoriesFactory.BuildDictionaryRepository<Type, bool>());
        }

        public static ConsoleLogger BuildConsoleLogger()
        {
            return new ConsoleLogger();
        }

        public static LoggerWrapperWithSourceTypePrefix BuildLoggerWrapperWithSourceTypePrefix(
            ILogger innerLogger)
        {
            return new LoggerWrapperWithSourceTypePrefix(innerLogger);
        }

        public static LoggerWrapperWithLogTypePrefix BuildLoggerWrapperWithLogTypePrefix(
            ILogger innerLogger)
        {
            return new LoggerWrapperWithLogTypePrefix(innerLogger);
        }

        public static LoggerWrapperWithFileDump BuildLoggerWrapperWithFileDump(
            string applicationDataFolder,
            string relativePath,
            ILoggerResolver loggerResolver,
            ILogger innerLogger)
        {
            var serializationArgument = new TextFileArgument();

            serializationArgument.Settings = new FilePathSettings
            {
                RelativePath = relativePath,
                ApplicationDataFolder = applicationDataFolder
            };

            var result = new LoggerWrapperWithFileDump(
                innerLogger,
                serializationArgument,
                PersistenceFactory.BuildSimplePlainTextSerializer(loggerResolver),
                new List<string>());

            return result;
        }
    }
}