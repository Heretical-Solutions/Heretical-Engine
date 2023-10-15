using System;

namespace HereticalSolutions.Logging
{
    /// <summary>
    /// Interface for a smart logger.
    /// </summary>
    public interface ISmartLogger
    {
        /// <summary>
        /// Gets whether the logger is active.
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// Checks if the logger will log for the specified log source.
        /// </summary>
        /// <param name="logSource">The type of the log source.</param>
        /// <returns>True if the logger will log for the specified log source, false otherwise.</returns>
        bool WillLog(Type logSource);

        /// <summary>
        /// Checks if the logger is active and will log for the specified log source.
        /// </summary>
        /// <param name="logSource">The type of the log source.</param>
        /// <returns>True if the logger is active and will log for the specified log source, false otherwise.</returns>
        bool ActiveAndWillLog(Type logSource);

        /// <summary>
        /// Logs the specified value for the specified log source.
        /// </summary>
        /// <param name="logSource">The type of the log source.</param>
        /// <param name="value">The log value to be logged.</param>
        void Log(
            Type logSource,
            string value);
        
        /// <summary>
        /// Logs the specified value with arguments for the specified log source.
        /// </summary>
        /// <param name="logSource">The type of the log source.</param>
        /// <param name="value">The log value to be logged.</param>
        /// <param name="arguments">The arguments to be formatted into the log value.</param>
        void Log(
            Type logSource,
            string value,
            object[] arguments);
        
        /// <summary>
        /// Logs the specified warning value for the specified log source.
        /// </summary>
        /// <param name="logSource">The type of the log source.</param>
        /// <param name="value">The warning value to be logged.</param>
        void LogWarning(
            Type logSource,
            string value);
        
        /// <summary>
        /// Logs the specified warning value with arguments for the specified log source.
        /// </summary>
        /// <param name="logSource">The type of the log source.</param>
        /// <param name="value">The warning value to be logged.</param>
        /// <param name="arguments">The arguments to be formatted into the warning value.</param>
        void LogWarning(
            Type logSource,
            string value,
            object[] arguments);
        
        /// <summary>
        /// Logs the specified error value for the specified log source.
        /// </summary>
        /// <param name="logSource">The type of the log source.</param>
        /// <param name="value">The error value to be logged.</param>
        void LogError(
            Type logSource,
            string value);
        
        /// <summary>
        /// Logs the specified error value with arguments for the specified log source.
        /// </summary>
        /// <param name="logSource">The type of the log source.</param>
        /// <param name="value">The error value to be logged.</param>
        /// <param name="arguments">The arguments to be formatted into the error value.</param>
        void LogError(
            Type logSource,
            string value,
            object[] arguments);
        
        /// <summary>
        /// Prepares the log value for the specified log source.
        /// </summary>
        /// <param name="logSource">The type of the log source.</param>
        /// <param name="value">The log value to be prepared.</param>
        /// <returns>The prepared log value.</returns>
        string PrepareLog(
            Type logSource,
            string value);

        /// <summary>
        /// Formats the specified string value using the provided options.
        /// </summary>
        /// <param name="value">The string value to be formatted.</param>
        /// <param name="options">The formatting options to be used.</param>
        /// <returns>The formatted string value.</returns>
        string Format(
            string value,
            EFormatOptions options);
        
        /// <summary>
        /// Logs the specified exception for the specified log source.
        /// </summary>
        /// <param name="logSource">The type of the log source.</param>
        /// <param name="value">The exception to be logged.</param>
        void Exception(
            Type logSource,
            string value);
    }
}