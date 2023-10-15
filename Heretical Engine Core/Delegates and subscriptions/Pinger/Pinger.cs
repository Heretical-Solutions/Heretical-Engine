using System;

namespace HereticalSolutions.Delegates.Pinging
{
    /// <summary>
    /// Represents a class that can publish events with no arguments.
    /// </summary>
    public class Pinger : IPublisherNoArgs, ISubscribableNoArgs
    {
        private Action multicastDelegate;

        #region IPublisherNoArgs

        /// <summary>
        /// Publishes the event to all subscribers.
        /// </summary>
        public void Publish()
        {
            multicastDelegate?.Invoke();
        }

        #endregion

        #region ISubscribableNoArgs

        /// <summary>
        /// Subscribes to the event.
        /// </summary>
        /// <param name="delegate">The delegate to subscribe.</param>
        public void Subscribe(Action @delegate)
        {
            multicastDelegate += @delegate;
        }

        /// <summary>
        /// Unsubscribes from the event.
        /// </summary>
        /// <param name="delegate">The delegate to unsubscribe.</param>
        public void Unsubscribe(Action @delegate)
        {
            multicastDelegate -= @delegate;
        }

        #endregion
    }
}