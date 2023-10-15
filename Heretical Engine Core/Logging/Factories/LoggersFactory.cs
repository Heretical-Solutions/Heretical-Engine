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
        public static DefaultLogger BuildDefaultLogger()
        {
            return new DefaultLogger();
        }
    }
}