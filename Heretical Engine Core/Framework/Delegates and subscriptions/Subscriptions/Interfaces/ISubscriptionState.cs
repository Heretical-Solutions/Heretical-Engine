using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents the state of a subscription
    /// </summary>
    /// <typeparam name="TInvokable">The type of the invokable object</typeparam>
    public interface ISubscriptionState<TInvokable>
    {
        /// <summary>
        /// Gets the invokable object associated with the subscription state
        /// </summary>
        TInvokable Invokable { get; }

        /// <summary>
        /// Gets the pool element associated with the subscription state
        /// </summary>
        IPoolElement<ISubscription> PoolElement { get; }
    }
}