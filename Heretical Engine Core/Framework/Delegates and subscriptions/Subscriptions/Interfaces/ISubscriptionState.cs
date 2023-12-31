using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates
{
    public interface ISubscriptionState<TInvokable>
    {
        /// <summary>
        /// Gets the invokable associated with the subscription state.
        /// </summary>
        TInvokable Invokable { get; }

        IPoolElement<ISubscription> PoolElement { get; }
    }
}