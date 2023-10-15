using System;
using HereticalSolutions.Delegates.Factories;
using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates.Subscriptions
{
    /// <summary>
    /// Represents a subscription with a single generic argument.
    /// </summary>
    /// <typeparam name="TValue">The type of the argument.</typeparam>
    public class SubscriptionSingleArgGeneric<TValue> : ISubscription,
        ISubscriptionState<IInvokableSingleArgGeneric<TValue>>,
        ISubscriptionState<IInvokableSingleArg>,
        ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>>,
        ISubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg>
    {
        private readonly IInvokableSingleArgGeneric<TValue> invokable;

        private object publisher;
        private object poolElement;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionSingleArgGeneric{TValue}"/> class. 
        /// </summary>
        /// <param name="delegate">The delegate representing the method to be invoked when the subscription is triggered.</param>
        public SubscriptionSingleArgGeneric(Action<TValue> @delegate)
        {
            invokable = DelegatesFactory.BuildDelegateWrapperSingleArgGeneric(@delegate);

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

        /// <summary>
        /// Gets the invokable object associated with the subscription.
        /// </summary>
        IInvokableSingleArgGeneric<TValue> ISubscriptionState<IInvokableSingleArgGeneric<TValue>>.Invokable =>
            (IInvokableSingleArgGeneric<TValue>)invokable;

        /// <summary>
        /// Gets the pool element associated with the subscription.
        /// </summary>
        IPoolElement<IInvokableSingleArg> ISubscriptionState<IInvokableSingleArg>.PoolElement =>
            (IPoolElement<IInvokableSingleArg>)poolElement;

        #endregion

        #region ISubscriptionState

        /// <summary>
        /// Gets the invokable object associated with the subscription.
        /// </summary>
        IInvokableSingleArg ISubscriptionState<IInvokableSingleArg>.Invokable =>
            (IInvokableSingleArg)invokable;

        /// <summary>
        /// Gets the pool element associated with the subscription.
        /// </summary>
        IPoolElement<IInvokableSingleArgGeneric<TValue>> ISubscriptionState<IInvokableSingleArgGeneric<TValue>>.PoolElement =>
            (IPoolElement<IInvokableSingleArgGeneric<TValue>>)poolElement;

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
                throw new Exception("[SubscriptionSingleArgGeneric] ATTEMPT TO ACTIVATE A SUBSCRIPTION THAT IS ALREADY ACTIVE");

            if (this.publisher != null)
                throw new Exception("[SubscriptionSingleArgGeneric] SUBSCRIPTION ALREADY HAS A PUBLISHER");

            if (poolElement != null)
                throw new Exception("[SubscriptionSingleArgGeneric] SUBSCRIPTION ALREADY HAS A POOL ELEMENT");

            if (invokable == null)
                throw new Exception("[SubscriptionSingleArgGeneric] INVALID DELEGATE");

            return true;
        }

        /// <summary>
        /// Activates the subscription.
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <param name="poolElement">The pool element.</param>
        public void Activate(
            INonAllocSubscribableSingleArgGeneric<TValue> publisher,
            IPoolElement<IInvokableSingleArgGeneric<TValue>> poolElement)
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
        public bool ValidateTermination(INonAllocSubscribableSingleArgGeneric<TValue> publisher)
        {
            if (!Active)
                throw new Exception("[SubscriptionSingleArgGeneric] ATTEMPT TO TERMINATE A SUBSCRIPTION THAT IS ALREADY INACTIVE");

            if (this.publisher != publisher)
                throw new Exception("[SubscriptionSingleArgGeneric] INVALID PUBLISHER");

            if (poolElement == null)
                throw new Exception("[SubscriptionSingleArgGeneric] INVALID POOL ELEMENT");

            return true;
        }

        #endregion

        #region ISubscriptionHandler

        /// <summary>
        /// Validates if the subscription can be activated.
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <returns>Always returns true.</returns>
        public bool ValidateActivation(INonAllocSubscribableSingleArg publisher)
        {
            if (Active)
                throw new Exception("[SubscriptionSingleArgGeneric] ATTEMPT TO ACTIVATE A SUBSCRIPTION THAT IS ALREADY ACTIVE");

            if (this.publisher != null)
                throw new Exception("[SubscriptionSingleArgGeneric] SUBSCRIPTION ALREADY HAS A PUBLISHER");

            if (poolElement != null)
                throw new Exception("[SubscriptionSingleArgGeneric] SUBSCRIPTION ALREADY HAS A POOL ELEMENT");

            if (invokable == null)
                throw new Exception("[SubscriptionSingleArgGeneric] INVALID DELEGATE");

            return true;
        }

        /// <summary>
        /// Activates the subscription.
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <param name="poolElement">The pool element.</param>
        public void Activate(INonAllocSubscribableSingleArg publisher, IPoolElement<IInvokableSingleArg> poolElement)
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
        public bool ValidateTermination(INonAllocSubscribableSingleArg publisher)
        {
            if (!Active)
                throw new Exception("[SubscriptionSingleArgGeneric] ATTEMPT TO TERMINATE A SUBSCRIPTION THAT IS ALREADY ACTIVE");

            if (this.publisher != publisher)
                throw new Exception("[SubscriptionSingleArgGeneric] INVALID PUBLISHER");

            if (poolElement == null)
                throw new Exception("[SubscriptionSingleArgGeneric] INVALID POOL ELEMENT");

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