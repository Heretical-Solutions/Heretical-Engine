namespace HereticalSolutions.Logging
{
    public interface ILogger
    {
        #region Log

        void Log(
            string value);

        void Log<TSource>(
            string value);

        void Log(
            Type logSource,
            string value);

        void Log(
            string value,
            object[] arguments);

        void Log<TSource>(
            string value,
            object[] arguments);

        void Log(
            Type logSource,
            string value,
            object[] arguments);

        #endregion

        #region Warning

        void LogWarning(
            string value);

        void LogWarning<TSource>(
            string value);

        void LogWarning(
            Type logSource,
            string value);

        void LogWarning(
            string value,
            object[] arguments);

        void LogWarning<TSource>(
            string value,
            object[] arguments);

        void LogWarning(
            Type logSource,
            string value,
            object[] arguments);

        #endregion

        #region Error

        void LogError(
            string value);

        void LogError<TSource>(
            string value);

        void LogError(
            Type logSource,
            string value);

        void LogError(
            string value,
            object[] arguments);

        void LogError<TSource>(
            string value,
            object[] arguments);

        void LogError(
            Type logSource,
            string value,
            object[] arguments);

        #endregion

        #region Exception

        string FormatException(
            string value);

        string FormatException<TSource>(
            string value);

        string FormatException(
            Type logSource,
            string value);

        #endregion
    }
}