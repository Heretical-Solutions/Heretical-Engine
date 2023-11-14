using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Notifiers
{
	public class AsyncNotifierSingleArgGeneric<TArgument, TValue>
		: IAsyncNotifierSingleArgGeneric<TArgument, TValue>
		  where TArgument : IEquatable<TArgument>
	{
		private readonly List<NotifyRequestSingleArgGeneric<TArgument, TValue>> requests;

		private readonly SemaphoreSlim semaphore;

		private readonly IFormatLogger logger;

		public AsyncNotifierSingleArgGeneric(
			List<NotifyRequestSingleArgGeneric<TArgument, TValue>> requests,
			SemaphoreSlim semaphore,
			IFormatLogger logger)
		{
			this.requests = requests;

			this.semaphore = semaphore;

			this.logger = logger;
		}

		#region IAsyncPropertySingleArgGeneric

		public async Task<TValue> GetValueWhenNotified(
			TArgument argument = default)
		{
			TaskCompletionSource<TValue> completionSource = new TaskCompletionSource<TValue>();

			var request = new NotifyRequestSingleArgGeneric<TArgument, TValue>(
				argument,
				completionSource);


			await semaphore.WaitAsync();

			//logger.Log<AsyncNotifierSingleArgGeneric<TArgument, TValue>>($"SEMAPHORE ACQUIRED");

			requests.Add(request);

			semaphore.Release();

			//logger.Log<AsyncNotifierSingleArgGeneric<TArgument, TValue>>($"SEMAPHORE RELEASED");


			await completionSource.Task;

			return completionSource.Task.Result;
		}

		public async Task Notify(
			TArgument argument,
			TValue value)
		{
			await semaphore.WaitAsync();

			//logger.Log<AsyncNotifierSingleArgGeneric<TArgument, TValue>>($"SEMAPHORE ACQUIRED");

			for (int i = requests.Count - 1; i >= 0; i--)
			{
				var request = requests[i];

				if (EqualityComparer<TArgument>.Default.Equals(request.Key, argument)) //if (request.Key.Equals(argument)) - bad for strings
				{
					requests.RemoveAt(i);

					request.TaskCompletionSource.TrySetResult(value);					
				}
			}

			semaphore.Release();

			//logger.Log<AsyncNotifierSingleArgGeneric<TArgument, TValue>>($"SEMAPHORE RELEASED");
		}

		#endregion
	}
}