using System;

namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents an interface for a single argument subscription handler that does not allocate memory.
    /// </summary>
    public interface INonAllocSubscribableSingleArg
    {
        /// <summary>
        /// Subscribes to the event with a specific value type.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="subscription">The subscription handler.</param>
        void Subscribe<TValue>(ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>> subscription);
        
        /// <summary>
        /// Subscribes to the event with a specific value type.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="subscription">The subscription handler.</param>
        void Subscribe(Type valueType, ISubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg> subscription);

        /// <summary>
        /// Unsubscribes from the event with a specific value type.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="subscription">The subscription handler.</param>
        void Unsubscribe<TValue>(ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>> subscription);
        
        /// <summary>
        /// Unsubscribes from the event with a specific value type.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="subscription">The subscription handler.</param>
        void Unsubscribe(Type valueType, ISubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg> subscription);
    }
}