using System;

namespace HereticalSolutions.Logging
{
    /// <summary>
    /// Interface for configuring logging behavior.
    /// </summary>
    public interface ILoggingConfigurable
    {
        /// <summary>
        /// Toggles the active state of the logging.
        /// </summary>
        /// <param name="value">The value to set the active state to.</param>
        void ToggleActive(bool value);

        /// <summary>
        /// Allows logging for the specified log source type.
        /// </summary>
        /// <param name="logSource">The type of log source to allow logging for.</param>
        void Allow(Type logSource);

        /// <summary>
        /// Denies logging for the specified log source type.
        /// </summary>
        /// <param name="logSource">The type of log source to deny logging for.</param>
        void Deny(Type logSource);
    }
}