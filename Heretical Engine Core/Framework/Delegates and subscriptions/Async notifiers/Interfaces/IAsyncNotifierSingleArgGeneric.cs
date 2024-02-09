namespace HereticalSolutions.Delegates.Notifiers
{
	/// <summary>
	/// Represents an interface for an asynchronous notifier with a single argument and a generic value
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument</typeparam>
	/// <typeparam name="TValue">The type of the value</typeparam>
	public interface IAsyncNotifierSingleArgGeneric<TArgument, TValue>
	{
		/// <summary>
		/// Gets the value when notified
		/// </summary>
		/// <param name="argument">The argument</param>
		/// <param name="ignoreKey">A flag indicating whether to ignore the key</param>
		/// <returns>A task representing the asynchronous operation and containing the value</returns>
		Task<TValue> GetValueWhenNotified(
			TArgument argument = default,
			bool ignoreKey = false);

		/// <summary>
		/// Gets the task that waits for notification
		/// </summary>
		/// <param name="argument">The argument</param>
		/// <param name="ignoreKey">A flag indicating whether to ignore the key</param>
		/// <returns>A task representing the asynchronous operation and containing the task that waits for notification</returns>
		Task<Task<TValue>> GetWaitForNotificationTask(
			TArgument argument = default,
			bool ignoreKey = false);

		/// <summary>
		/// Notifies with the specified argument and value
		/// </summary>
		/// <param name="argument">The argument</param>
		/// <param name="value">The value</param>
		/// <returns>A task representing the asynchronous operation</returns>
		Task Notify(
			TArgument argument,
			TValue value);
	}
}