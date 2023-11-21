using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;
using HereticalSolutions.Persistence.Factories;

namespace HereticalSolutions.Logging.Factories
{
    /// <summary>
    /// Class for creating instances of loggers.
    /// </summary>
    public static class LoggersFactory
    {
        /// <summary>
        /// Builds a default logger instance.
        /// </summary>
        /// <returns>A new instance of the DefaultLogger class.</returns>
        public static ConsoleLogger BuildDefaultLogger()
        {
            return new ConsoleLogger();
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

            return new ConsoleLoggerWithFileDump(
                serializationArgument,
                PersistenceFactory.BuildSimplePlainTextSerializer(),
                new List<string>());
        }
    }
}