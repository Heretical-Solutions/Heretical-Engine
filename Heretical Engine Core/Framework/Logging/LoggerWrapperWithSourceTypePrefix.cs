namespace HereticalSolutions.Logging
{
    public class LoggerWrapperWithSourceTypePrefix
        : ILogger,
          ILoggerWrapper
    {
        private readonly ILogger innerLogger;

        public LoggerWrapperWithSourceTypePrefix(
            ILogger innerLogger)
        {
            this.innerLogger = innerLogger;
        }

        #region ILoggerWrapper

        public ILogger InnerLogger { get => innerLogger; }

        #endregion

        #region ILogger

        #region Log

        public void Log(
            string value)
        {
            innerLogger.Log(value);
        }

        public void Log<TSource>(
            string value)
        {
            value = FormatLogWithSourceType(
                value,
                typeof(TSource));

            innerLogger.Log<TSource>(value);
        }

        public void Log(
            Type logSource,
            string value)
        {
            value = FormatLogWithSourceType(
                value,
                logSource);

            innerLogger.Log(
                logSource,
                value);
        }

        public void Log(
            string value,
            object[] arguments)
        {
            innerLogger.Log(
                value,
                arguments);
        }

        public void Log<TSource>(
            string value,
            object[] arguments)
        {
            value = FormatLogWithSourceType(
                value,
                typeof(TSource));

            innerLogger.Log<TSource>(
                value,
                arguments);
        }

        public void Log(
            Type logSource,
            string value,
            object[] arguments)
        {
            value = FormatLogWithSourceType(
                value,
                logSource);

            innerLogger.Log(
                logSource,
                value,
                arguments);
        }

        #endregion

        #region Warning

        public void LogWarning(
            string value)
        {
            innerLogger.LogWarning(
                value);
        }

        public void LogWarning<TSource>(
            string value)
        {
            value = FormatLogWithSourceType(
                value,
                typeof(TSource));

            innerLogger.LogWarning<TSource>(value);
        }

        public void LogWarning(
            Type logSource,
            string value)
        {
            value = FormatLogWithSourceType(
                value,
                logSource);

            innerLogger.LogWarning(
                logSource,
                value);
        }

        public void LogWarning(
            string value,
            object[] arguments)
        {
            innerLogger.LogWarning(
                value,
                arguments);
        }

        public void LogWarning<TSource>(
            string value,
            object[] arguments)
        {
            value = FormatLogWithSourceType(
                value,
                typeof(TSource));

            innerLogger.LogWarning<TSource>(
                value,
                arguments);
        }

        public void LogWarning(
            Type logSource,
            string value,
            object[] arguments)
        {
            value = FormatLogWithSourceType(
                value,
                logSource);

            innerLogger.LogWarning(
                logSource,
                value,
                arguments);
        }

        #endregion

        #region Error

        public void LogError(
            string value)
        {
            innerLogger.LogError(
                value);
        }

        public void LogError<TSource>(
            string value)
        {
            value = FormatLogWithSourceType(
                value,
                typeof(TSource));

            innerLogger.LogError<TSource>(value);
        }

        public void LogError(
            Type logSource,
            string value)
        {
            value = FormatLogWithSourceType(
                value,
                logSource);

            innerLogger.LogError(
                logSource,
                value);
        }

        public void LogError(
            string value,
            object[] arguments)
        {
            innerLogger.LogError(
                value,
                arguments);
        }

        public void LogError<TSource>(
            string value,
            object[] arguments)
        {
            value = FormatLogWithSourceType(
                value,
                typeof(TSource));

            innerLogger.LogError<TSource>(
                value,
                arguments);
        }

        public void LogError(
            Type logSource,
            string value,
            object[] arguments)
        {
            value = FormatLogWithSourceType(
                value,
                logSource);

            innerLogger.LogError(
                logSource,
                value,
                arguments);
        }

        #endregion

        #region Exception

        public string FormatException(
            string value)
        {
            return innerLogger.FormatException(value);
        }

        public string FormatException<TSource>(
            string value)
        {
            value = FormatLogWithSourceType(
                value,
                typeof(TSource));

            return innerLogger.FormatException<TSource>(value);
        }

        public string FormatException(
            Type logSource,
            string value)
        {
            value = FormatLogWithSourceType(
                value,
                logSource);

            return innerLogger.FormatException(
                logSource,
                value);
        }

        #endregion

        #endregion

        private string FormatLogWithSourceType(
            string value,
            Type logSource)
        {
            return $"[{logSource.Name}] {value}";
        }
    }
}