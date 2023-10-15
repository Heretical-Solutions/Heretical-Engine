using HereticalSolutions.Delegates;

namespace HereticalSolutions.Time
{
    /// <summary>
    /// Provides synchronization functionality for time-related operations.
    /// </summary>
    public interface ISynchronizationProvider
    {
        /// <summary>
        /// Subscribes to receive synchronization updates.
        /// </summary>
        /// <param name="subscription">The subscription to be added.</param>
        void Subscribe(ISubscription subscription);

        /// <summary>
        /// Unsubscribes from receiving synchronization updates.
        /// </summary>
        /// <param name="subscription">The subscription to be removed.</param>
        void Unsubscribe(ISubscription subscription);
    }
}