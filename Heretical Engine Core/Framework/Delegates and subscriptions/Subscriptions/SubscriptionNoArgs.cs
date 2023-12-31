using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Pools;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Subscriptions
{
    /// <summary>
    /// Represents a subscription without any arguments.
    /// </summary>
    public class SubscriptionNoArgs
        : ISubscription,
          ISubscriptionState<IInvokableNoArgs>,
          ISubscriptionHandler<INonAllocSubscribableNoArgs, IInvokableNoArgs>
    {
        private readonly IInvokableNoArgs invokable;

        private readonly IFormatLogger logger;
        
        private INonAllocSubscribableNoArgs publisher;

        private IPoolElement<ISubscription> poolElement;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionNoArgs"/> class.
        /// </summary>
        /// <param name="delegate">The action delegate.</param>
        public SubscriptionNoArgs(
            Action @delegate,
            IFormatLogger logger)
        {
            invokable = DelegatesFactory.BuildDelegateWrapperNoArgs(@delegate);

            this.logger = logger;

            Active = false;

            publisher = null;

            poolElement = null;
        }

        #region ISubscription
        
        /// <summary>
        /// Gets a value indicating whether the subscription is active.
        /// </summary>
        public bool Active { get; private set;  }
        
        #endregion

        #region ISubscriptionState

        /// <summary>
        /// Gets the invokable delegate.
        /// </summary>
        public IInvokableNoArgs Invokable
        {
            get => invokable;
        }

        /// <summary>
        /// Gets the pool element.
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
        /// <param name="publisher">The publisher to validate against.</param>
        /// <returns>True if the activation is valid, otherwise false.</returns>
        /// <exception cref="Exception">Thrown when the activation is not valid.</exception>
        public bool ValidateActivation(INonAllocSubscribableNoArgs publisher)
        {
            if (Active)
                logger?.ThrowException<SubscriptionNoArgs>("ATTEMPT TO ACTIVATE A SUBSCRIPTION THAT IS ALREADY ACTIVE");
			
            if (this.publisher != null)
                logger?.ThrowException<SubscriptionNoArgs>("SUBSCRIPTION ALREADY HAS A PUBLISHER");
			
            if (poolElement != null)
                logger?.ThrowException<SubscriptionNoArgs>("SUBSCRIPTION ALREADY HAS A POOL ELEMENT");
			
            if (invokable == null)
                logger?.ThrowException<SubscriptionNoArgs>("INVALID DELEGATE");

            return true;
        }

        /// <summary>
        /// Activates the subscription.
        /// </summary>
        /// <param name="publisher">The publisher to subscribe to.</param>
        /// <param name="poolElement">The pool element to assign.</param>
        public void Activate(
            INonAllocSubscribableNoArgs publisher,
            IPoolElement<ISubscription> poolElement)
        {
            this.poolElement = poolElement;

            this.publisher = publisher;
            
            Active = true;
        }

        /// <summary>
        /// Validates whether the subscription can be terminated.
        /// </summary>
        /// <param name="publisher">The publisher to validate against.</param>
        /// <returns>True if the termination is valid, otherwise false.</returns>
        /// <exception cref="Exception">Thrown when the termination is not valid.</exception>
        public bool ValidateTermination(INonAllocSubscribableNoArgs publisher)
        {
            if (!Active)
                logger?.ThrowException<SubscriptionNoArgs>("ATTEMPT TO TERMINATE A SUBSCRIPTION THAT IS ALREADY ACTIVE");
			
            if (this.publisher != publisher)
                logger?.ThrowException<SubscriptionNoArgs>("INVALID PUBLISHER");
			
            if (poolElement == null)
                logger?.ThrowException<SubscriptionNoArgs>("INVALID POOL ELEMENT");

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