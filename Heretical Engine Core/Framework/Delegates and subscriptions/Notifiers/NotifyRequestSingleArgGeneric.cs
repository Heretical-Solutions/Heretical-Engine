namespace HereticalSolutions.Delegates.Notifiers
{
	public class NotifyRequestSingleArgGeneric<TArgument, TValue>
	{
		public TArgument Key { get; private set; }

		public bool IgnoreKey { get; private set; }

		public TaskCompletionSource<TValue> TaskCompletionSource { get; private set; }

		public NotifyRequestSingleArgGeneric(
			TArgument key,
			bool ignoreKey,
			TaskCompletionSource<TValue> taskCompletionSource)
		{
			Key = key;

			IgnoreKey = ignoreKey;

			TaskCompletionSource = taskCompletionSource;
		}
	}
}