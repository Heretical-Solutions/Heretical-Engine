using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Pools;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Subscriptions
{
    /// <summary>
    /// Represents a subscription with multiple arguments.
    /// </summary>
    public class SubscriptionMultipleArgs
        : ISubscription,
          ISubscriptionState<IInvokableMultipleArgs>,
          ISubscriptionHandler<INonAllocSubscribableMultipleArgs, IInvokableMultipleArgs>
    {
        private readonly IInvokableMultipleArgs invokable;

        private readonly ILogger logger;

        private INonAllocSubscribableMultipleArgs publisher;

        private IPoolElement<ISubscription> poolElement;
        
        public SubscriptionMultipleArgs(
            Action<object[]> @delegate,
            ILogger logger = null)
        {
            invokable = DelegatesFactory.BuildDelegateWrapperMultipleArgs(@delegate);

            this.logger = logger;

            Active = false;

            publisher = null;

            poolElement = null;
        }

        #region ISubscription
        
        /// <summary>
        /// Gets or sets the active state of the subscription.
        /// </summary>
        public bool Active { get; private set;  }

        #endregion
        
        #region ISubscriptionState

        /// <summary>
        /// Gets the delegate that the subscription can invoke.
        /// </summary>
        public IInvokableMultipleArgs Invokable
        {
            get => invokable;
        }

        /// <summary>
        /// Gets the pool element associated with the subscription.
        /// </summary>
        public IPoolElement<ISubscription> PoolElement
        {
            get => poolElement;
        }

        #endregion

        #region ISubscriptionHandler
        
        /// <summary>
        /// Validates whether the subscription can be activated.
        /// </summary>
        /// <param name="publisher">The publisher to subscribe to.</param>
        /// <returns>True if the subscription can be activated, false otherwise.</returns>
        public bool ValidateActivation(INonAllocSubscribableMultipleArgs publisher)
        {
            if (Active)
                throw new Exception(
                    logger.TryFormat<SubscriptionMultipleArgs>(
                        "ATTEMPT TO ACTIVATE A SUBSCRIPTION THAT IS ALREADY ACTIVE"));
			
            if (this.publisher != null)
                throw new Exception(
                    logger.TryFormat<SubscriptionMultipleArgs>(
                        "SUBSCRIPTION ALREADY HAS A PUBLISHER"));
			
            if (poolElement != null)
                throw new Exception(
                    logger.TryFormat<SubscriptionMultipleArgs>(
                        "SUBSCRIPTION ALREADY HAS A POOL ELEMENT"));
			
            if (invokable == null)
                throw new Exception(
                    logger.TryFormat<SubscriptionMultipleArgs>(
                        "INVALID DELEGATE"));

            return true;
        }
        
        /// <summary>
        /// Activates the subscription.
        /// </summary>
        /// <param name="publisher">The publisher to subscribe to.</param>
        /// <param name="poolElement">The pool element associated with the subscription.</param>
        public void Activate(
            INonAllocSubscribableMultipleArgs publisher,
            IPoolElement<ISubscription> poolElement)
        {
            this.poolElement = poolElement;

            this.publisher = publisher;
            
            Active = true;
        }
        
        /// <summary>
        /// Validates whether the subscription can be terminated.
        /// </summary>
        /// <param name="publisher">The publisher to unsubscribe from.</param>
        /// <returns>True if the subscription can be terminated, false otherwise.</returns>
        public bool ValidateTermination(INonAllocSubscribableMultipleArgs publisher)
        {
            if (!Active)
                throw new Exception(
                    logger.TryFormat<SubscriptionMultipleArgs>(
                        "ATTEMPT TO TERMINATE A SUBSCRIPTION THAT IS ALREADY ACTIVE"));
			
            if (this.publisher != publisher)
                throw new Exception(
                    logger.TryFormat<SubscriptionMultipleArgs>(
                        "INVALID PUBLISHER"));
			
            if (poolElement == null)
                throw new Exception(
                    logger.TryFormat<SubscriptionMultipleArgs>(
                        "INVALID POOL ELEMENT"));

            return true;
        }
        
        /// <summary>
        /// Terminates the subscription.
        /// </summary>
        public void Terminate()
        {
            poolElement = null;
            
            publisher = null;
            
            Active = false;
        }

        #endregion
    }
}