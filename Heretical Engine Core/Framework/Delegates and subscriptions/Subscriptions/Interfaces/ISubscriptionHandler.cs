using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents a subscription handler for a subscribable entity.
    /// </summary>
    /// <typeparam name="TSubscribable">The type of the subscribable entity.</typeparam>
    /// <typeparam name="TInvokable">The type of the invokable entity.</typeparam>
    public interface ISubscriptionHandler<TSubscribable, TInvokable>
    {
        /// <summary>
        /// Validates the activation of a subscribable entity.
        /// </summary>
        /// <param name="publisher">The subscribable entity to validate.</param>
        /// <returns>True if the activation is valid, otherwise false.</returns>
        bool ValidateActivation(TSubscribable publisher);
        
        /// <summary>
        /// Activates the subscribable entity with the provided invokable entity.
        /// </summary>
        /// <param name="publisher">The subscribable entity to activate.</param>
        /// <param name="poolElement">The invokable entity to use for activation.</param>
        void Activate(
            TSubscribable publisher,
            IPoolElement<TInvokable> poolElement);

        /// <summary>
        /// Validates the termination of a subscribable entity.
        /// </summary>
        /// <param name="publisher">The subscribable entity to validate.</param>
        /// <returns>True if the termination is valid, otherwise false.</returns>
        bool ValidateTermination(TSubscribable publisher);
        
        /// <summary>
        /// Terminates the subscription handler.
        /// </summary>
        void Terminate();
    }
}