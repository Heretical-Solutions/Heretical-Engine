using HereticalSolutions.Delegates.Subscriptions;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Factories
{
    public static partial class DelegatesFactory
    {
        #region Subscriptions

        public static SubscriptionNoArgs BuildSubscriptionNoArgs(
            Action @delegate,
            ILoggerResolver loggerResolver = null)
        {
            IFormatLogger logger =
                loggerResolver?.GetLogger<SubscriptionNoArgs>()
                ?? null;

            return new SubscriptionNoArgs(
                @delegate,
                logger);
        }
        
        public static SubscriptionSingleArgGeneric<TValue> BuildSubscriptionSingleArgGeneric<TValue>(
            Action<TValue> @delegate,
            ILoggerResolver loggerResolver = null)
        {
            IFormatLogger logger =
                loggerResolver?.GetLogger<SubscriptionSingleArgGeneric<TValue>>()
                ?? null;

            return new SubscriptionSingleArgGeneric<TValue>(
                @delegate,
                loggerResolver,
                logger);
        }
        
        public static SubscriptionMultipleArgs BuildSubscriptionMultipleArgs(
            Action<object[]> @delegate,
            ILoggerResolver loggerResolver = null)
        {
            IFormatLogger logger =
                loggerResolver?.GetLogger<SubscriptionMultipleArgs>()
                ?? null;

            return new SubscriptionMultipleArgs(
                @delegate,
                logger);
        }

        #endregion
    }
}