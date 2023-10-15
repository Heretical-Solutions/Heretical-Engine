using System;

using HereticalSolutions.Delegates.Subscriptions;

namespace HereticalSolutions.Delegates.Factories
{
    public static partial class DelegatesFactory
    {
        #region Subscriptions

        /// <summary>
        /// Builds a subscription with no arguments.
        /// </summary>
        /// <param name="delegate">The action delegate to subscribe to.</param>
        /// <returns>A new instance of SubscriptionNoArgs.</returns>
        public static SubscriptionNoArgs BuildSubscriptionNoArgs(Action @delegate)
        {
            return new SubscriptionNoArgs(@delegate);
        }
        
        /// <summary>
        /// Builds a subscription with a single generic argument.
        /// </summary>
        /// <typeparam name="TValue">The type of the argument.</typeparam>
        /// <param name="delegate">The action delegate to subscribe to.</param>
        /// <returns>A new instance of SubscriptionSingleArgGeneric&lt;TValue&gt;.</returns>
        public static SubscriptionSingleArgGeneric<TValue> BuildSubscriptionSingleArgGeneric<TValue>(Action<TValue> @delegate)
        {
            return new SubscriptionSingleArgGeneric<TValue>(@delegate);
        }
        
        /// <summary>
        /// Builds a subscription with multiple arguments.
        /// </summary>
        /// <param name="delegate">The action delegate to subscribe to.</param>
        /// <returns>A new instance of SubscriptionMultipleArgs.</returns>
        public static SubscriptionMultipleArgs BuildSubscriptionMultipleArgs(Action<object[]> @delegate)
        {
            return new SubscriptionMultipleArgs(@delegate);
        }

        #endregion
    }
}