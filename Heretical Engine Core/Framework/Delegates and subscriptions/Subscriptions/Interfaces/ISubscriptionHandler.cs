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
        bool ValidateActivation(TSubscribable publisher);
        
        void Activate(
            TSubscribable publisher,
            IPoolElement<ISubscription> poolElement);

        bool ValidateTermination(TSubscribable publisher);
        
        void Terminate();
    }
}