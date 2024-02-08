namespace HereticalSolutions.Delegates.Notifiers
{
	public interface IAsyncNotifierSingleArgGeneric<TArgument, TValue>
	{
		Task<TValue> GetValueWhenNotified(
			TArgument argument = default,
			bool ignoreKey = false);

		Task<Task<TValue>> GetWaitForNotificationTask(
			TArgument argument = default,
			bool ignoreKey = false);

		Task Notify(
			TArgument argument,
			TValue value);
	}
}