using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents a handler for managing subscriptions to a specific type of subscribable object
    /// </summary>
    /// <typeparam name="TSubscribable">The type of the subscribable object</typeparam>
    /// <typeparam name="TInvokable">The type of the invokable object</typeparam>
    public interface ISubscriptionHandler<TSubscribable, TInvokable>
    {
        /// <summary>
        /// Validates if the subscription can be activated for the specified publisher
        /// </summary>
        /// <param name="publisher">The publisher object</param>
        /// <returns><c>true</c> if the activation is valid; otherwise, <c>false</c></returns>
        bool ValidateActivation(TSubscribable publisher);
        
        /// <summary>
        /// Activates the subscription for the specified publisher
        /// </summary>
        /// <param name="publisher">The publisher object</param>
        /// <param name="poolElement">The pool element associated with the subscription</param>
        void Activate(
            TSubscribable publisher,
            IPoolElement<ISubscription> poolElement);

        /// <summary>
        /// Validates if the subscription can be terminated for the specified publisher
        /// </summary>
        /// <param name="publisher">The publisher object</param>
        /// <returns><c>true</c> if the termination is valid; otherwise, <c>false</c></returns>
        bool ValidateTermination(TSubscribable publisher);
        
        /// <summary>
        /// Terminates the subscription
        /// </summary>
        void Terminate();
    }
}