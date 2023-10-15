using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents a subscription state for a generic invokable.
    /// </summary>
    /// <typeparam name="TInvokable">The type of the invokable.</typeparam>
    public interface ISubscriptionState<TInvokable>
    {
        /// <summary>
        /// Gets the invokable associated with the subscription state.
        /// </summary>
        TInvokable Invokable { get; }

        /// <summary>
        /// Gets the pool element associated with the subscription state.
        /// </summary>
        IPoolElement<TInvokable> PoolElement { get; }
    }
}