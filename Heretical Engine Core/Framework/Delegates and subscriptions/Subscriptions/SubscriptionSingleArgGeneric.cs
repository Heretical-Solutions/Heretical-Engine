using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Pools;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Subscriptions
{
    /// <summary>
    /// Represents a subscription with a single generic argument.
    /// </summary>
    /// <typeparam name="TValue">The type of the argument.</typeparam>
    public class SubscriptionSingleArgGeneric<TValue>
        : ISubscription,
          ISubscriptionState<IInvokableSingleArgGeneric<TValue>>,
          ISubscriptionState<IInvokableSingleArg>,
          ISubscriptionHandler<
              INonAllocSubscribableSingleArgGeneric<TValue>,
              IInvokableSingleArgGeneric<TValue>>,
          ISubscriptionHandler<
              INonAllocSubscribableSingleArg,
              IInvokableSingleArg>,
          ICleanUppable,
          IDisposable
    {
        private readonly IInvokableSingleArgGeneric<TValue> invokable;

        private readonly ILogger logger;

        private object publisher;

        private IPoolElement<ISubscription> poolElement;

        public SubscriptionSingleArgGeneric(
            Action<TValue> @delegate,
            ILoggerResolver loggerResolver = null,
            ILogger logger = null)
        {
            invokable = DelegatesFactory.BuildDelegateWrapperSingleArgGeneric(
                @delegate,
                loggerResolver);

            this.logger = logger;

            Active = false;

            publisher = null;

            poolElement = null;
        }

        #region ISubscription

        /// <summary>
        /// Gets a value indicating whether the subscription is active.
        /// </summary>
        public bool Active { get; private set; }

        #endregion

        #region ISubscriptionState (Generic)

        IInvokableSingleArgGeneric<TValue> ISubscriptionState<IInvokableSingleArgGeneric<TValue>>.Invokable =>
            (IInvokableSingleArgGeneric<TValue>)invokable;

        IPoolElement<ISubscription> ISubscriptionState<IInvokableSingleArgGeneric<TValue>>.PoolElement => poolElement;

        #endregion

        #region ISubscriptionState

        IInvokableSingleArg ISubscriptionState<IInvokableSingleArg>.Invokable =>
            (IInvokableSingleArg)invokable;

        IPoolElement<ISubscription> ISubscriptionState<IInvokableSingleArg>.PoolElement => poolElement;

        #endregion

        #region ISubscriptionHandler (Generic)

        /// <summary>
        /// Validates if the subscription can be activated.
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <returns>Always returns true.</returns>
        public bool ValidateActivation(INonAllocSubscribableSingleArgGeneric<TValue> publisher)
        {
            if (Active)
                throw new Exception(
                    logger.TryFormat<SubscriptionSingleArgGeneric<TValue>>(
                        "ATTEMPT TO ACTIVATE A SUBSCRIPTION THAT IS ALREADY ACTIVE"));

            if (this.publisher != null)
                throw new Exception(
                    logger.TryFormat<SubscriptionSingleArgGeneric<TValue>>(
                        "SUBSCRIPTION ALREADY HAS A PUBLISHER"));

            if (poolElement != null)
                throw new Exception(
                    logger.TryFormat<SubscriptionSingleArgGeneric<TValue>>(
                        "SUBSCRIPTION ALREADY HAS A POOL ELEMENT"));

            if (invokable == null)
                throw new Exception(
                    logger.TryFormat<SubscriptionSingleArgGeneric<TValue>>(
                        "INVALID DELEGATE"));

            return true;
        }

        /// <summary>
        /// Activates the subscription.
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <param name="poolElement">The pool element.</param>
        public void Activate(
            INonAllocSubscribableSingleArgGeneric<TValue> publisher,
            IPoolElement<ISubscription> poolElement)
        {
            this.poolElement = poolElement;

            this.publisher = publisher;

            Active = true;
        }

        /// <summary>
        /// Validates if the subscription can be terminated.
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <returns>Always returns true.</returns>
        public bool ValidateTermination(
            INonAllocSubscribableSingleArgGeneric<TValue> publisher)
        {
            if (!Active)
                throw new Exception(
                    logger.TryFormat<SubscriptionSingleArgGeneric<TValue>>(
                        "ATTEMPT TO TERMINATE A SUBSCRIPTION THAT IS ALREADY INACTIVE"));

            if (this.publisher != publisher)
                throw new Exception(
                    logger.TryFormat<SubscriptionSingleArgGeneric<TValue>>(
                        "INVALID PUBLISHER"));

            if (poolElement == null)
                throw new Exception(
                    logger.TryFormat<SubscriptionSingleArgGeneric<TValue>>(
                        "INVALID POOL ELEMENT"));

            return true;
        }

        #endregion

        #region ISubscriptionHandler

        /// <summary>
        /// Validates if the subscription can be activated.
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <returns>Always returns true.</returns>
        public bool ValidateActivation(
            INonAllocSubscribableSingleArg publisher)
        {
            if (Active)
                throw new Exception(
                    logger.TryFormat<SubscriptionSingleArgGeneric<TValue>>(
                        "ATTEMPT TO ACTIVATE A SUBSCRIPTION THAT IS ALREADY ACTIVE"));

            if (this.publisher != null)
                throw new Exception(
                    logger.TryFormat<SubscriptionSingleArgGeneric<TValue>>(
                        "SUBSCRIPTION ALREADY HAS A PUBLISHER"));

            if (poolElement != null)
                throw new Exception(
                    logger.TryFormat<SubscriptionSingleArgGeneric<TValue>>(
                        "SUBSCRIPTION ALREADY HAS A POOL ELEMENT"));

            if (invokable == null)
                throw new Exception(
                    logger.TryFormat<SubscriptionSingleArgGeneric<TValue>>(
                        "INVALID DELEGATE"));

            return true;
        }

        public void Activate(
            INonAllocSubscribableSingleArg publisher,
            IPoolElement<ISubscription> poolElement)
        {
            this.poolElement = poolElement;

            this.publisher = publisher;

            Active = true;

            logger?.Log<SubscriptionMultipleArgs>(
                "SUBSCRIPTION ACTIVATED");
        }

        public bool ValidateTermination(
            INonAllocSubscribableSingleArg publisher)
        {
            if (!Active)
                throw new Exception(
                    logger.TryFormat<SubscriptionSingleArgGeneric<TValue>>(
                        "ATTEMPT TO TERMINATE A SUBSCRIPTION THAT IS ALREADY ACTIVE"));

            if (this.publisher != publisher)
                throw new Exception(
                    logger.TryFormat<SubscriptionSingleArgGeneric<TValue>>(
                        "INVALID PUBLISHER"));

            if (poolElement == null)
                throw new Exception(
                    logger.TryFormat<SubscriptionSingleArgGeneric<TValue>>(
                        "INVALID POOL ELEMENT"));

            return true;
        }

        public void Terminate()
        {
            poolElement = null;

            publisher = null;

            Active = false;

            logger?.Log<SubscriptionNoArgs>(
                "SUBSCRIPTION TERMINATED");
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (Active)
            {
                switch (publisher)
                {
                    case INonAllocSubscribableSingleArgGeneric<TValue> genericPublisher:

                        genericPublisher.Unsubscribe(this);

                        break;

                    case INonAllocSubscribableSingleArg nonGenericPublisher:

                        nonGenericPublisher.Unsubscribe<TValue>(this);

                        break;
                }
            }

            if (invokable is ICleanUppable)
                (invokable as ICleanUppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (Active)
            {
                switch (publisher)
                {
                    case INonAllocSubscribableSingleArgGeneric<TValue> genericPublisher:

                        genericPublisher.Unsubscribe(this);

                        break;

                    case INonAllocSubscribableSingleArg nonGenericPublisher:
                    
                        nonGenericPublisher.Unsubscribe<TValue>(this);

                        break;
                }
            }

            if (invokable is IDisposable)
                (invokable as IDisposable).Dispose();
        }

        #endregion
    }
}