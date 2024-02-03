using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Delegates.Pinging
{
    /// <summary>
    /// Represents a class that can publish events with no arguments.
    /// </summary>
    public class Pinger
        : IPublisherNoArgs,
          ISubscribableNoArgs,
          ICleanUppable,
          IDisposable
    {
        private Action multicastDelegate;

        #region IPublisherNoArgs

        /// <summary>
        /// Publishes the event to all subscribers.
        /// </summary>
        public void Publish()
        {
            //If any delegate that is invoked attempts to unsubscribe itself, it would produce an error because the collection
            //should NOT be changed during the invokation
            //That's why we'll copy the multicast delegate to a local variable and invoke it from there
            //multicastDelegate?.Invoke();

            var multicastDelegateCopy = multicastDelegate;

            multicastDelegateCopy?.Invoke();

            multicastDelegateCopy = null;
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

        IEnumerable<Action> ISubscribableNoArgs.AllSubscriptions
        {
            get
            {
                //Kudos to Copilot for Cast() and the part after the ?? operator
                return multicastDelegate?.GetInvocationList().Cast<Action>() ?? Enumerable.Empty<Action>();
            }
        }

        #region ISubscribable

        IEnumerable<object> ISubscribable.AllSubscriptions
        {
            get
            {
                //Kudos to Copilot for Cast() and the part after the ?? operator
                return multicastDelegate?.GetInvocationList().Cast<object>() ?? Enumerable.Empty<object>();
            }
        }

        public void UnsubscribeAll()
        {
            multicastDelegate = null;
        }

        #endregion

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            multicastDelegate = null;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            multicastDelegate = null;
        }

        #endregion
    }
}