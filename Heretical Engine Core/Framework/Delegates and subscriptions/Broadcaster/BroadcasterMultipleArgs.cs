using System;

namespace HereticalSolutions.Delegates.Broadcasting
{
    /// <summary>
    /// Class representing a broadcaster for multiple arguments. Implements IPublisherMultipleArgs and ISubscribableMultipleArgs interfaces.
    /// </summary>
    public class BroadcasterMultipleArgs
        : IPublisherMultipleArgs,
          ISubscribableMultipleArgs
    {
        private readonly BroadcasterGeneric<object[]> innerBroadcaster;

        /// <summary>
        /// Constructor for BroadcasterMultipleArgs class.
        /// </summary>
        /// <param name="innerBroadcaster">An instance of BroadcasterGeneric&lt;object[]&gt;.</param>
        public BroadcasterMultipleArgs(BroadcasterGeneric<object[]> innerBroadcaster)
        {
            this.innerBroadcaster = innerBroadcaster;
        }

        #region IPublisherMultipleArgs

        /// <summary>
        /// Publishes the provided values to the inner broadcaster's subscribers.
        /// </summary>
        /// <param name="values">An array of objects representing the values to be published.</param>
        public void Publish(object[] values)
        {
            innerBroadcaster.Publish(values);
        }

        #endregion

        #region ISubscribableMultipleArgs
        
        /// <summary>
        /// Subscribes a delegate to the inner broadcaster.
        /// </summary>
        /// <param name="delegate">The delegate to be subscribed.</param>
        public void Subscribe(Action<object[]> @delegate)
        {
            innerBroadcaster.Subscribe(@delegate);
        }

        /// <summary>
        /// Unsubscribes a delegate from the inner broadcaster.
        /// </summary>
        /// <param name="delegate">The delegate to be unsubscribed.</param>
        public void Unsubscribe(Action<object[]> @delegate)
        {
            innerBroadcaster.Unsubscribe(@delegate);
        }

        #endregion
    }
}