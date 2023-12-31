using HereticalSolutions.Delegates.Subscriptions;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Factories
{
    public static partial class DelegatesFactory
    {
        #region Subscriptions

        public static SubscriptionNoArgs BuildSubscriptionNoArgs(
            Action @delegate,
            IFormatLogger logger)
        {
            return new SubscriptionNoArgs(
                @delegate,
                logger);
        }
        
        public static SubscriptionSingleArgGeneric<TValue> BuildSubscriptionSingleArgGeneric<TValue>(
            Action<TValue> @delegate,
            IFormatLogger logger)
        {
            return new SubscriptionSingleArgGeneric<TValue>(
                @delegate,
                logger);
        }
        
        public static SubscriptionMultipleArgs BuildSubscriptionMultipleArgs(
            Action<object[]> @delegate,
            IFormatLogger logger)
        {
            return new SubscriptionMultipleArgs(
                @delegate,
                logger);
        }

        #endregion
    }
}