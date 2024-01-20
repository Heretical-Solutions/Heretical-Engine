using HereticalSolutions.Delegates.Notifiers;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Factories
{
	public static partial class NotifiersFactory
	{
		public static AsyncNotifierSingleArgGeneric<TArgument, TValue> BuildAsyncNotifierSingleArgGeneric<TArgument, TValue>(
			ILoggerResolver loggerResolver = null)
			where TArgument : IEquatable<TArgument>
		{
			ILogger logger =
				loggerResolver?.GetLogger<AsyncNotifierSingleArgGeneric<TArgument, TValue>>()
				?? null;

			return new AsyncNotifierSingleArgGeneric<TArgument, TValue>(
				new List<NotifyRequestSingleArgGeneric<TArgument, TValue>>(),
				new SemaphoreSlim(1, 1),
				logger);
		}
	}
}