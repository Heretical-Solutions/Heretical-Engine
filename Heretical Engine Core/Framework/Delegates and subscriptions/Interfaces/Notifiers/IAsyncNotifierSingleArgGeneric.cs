using System;

namespace HereticalSolutions.Delegates.Notifiers
{
	public interface IAsyncNotifierSingleArgGeneric<TArgument, TValue>
	{
		Task<TValue> GetValueWhenNotified(TArgument argument = default);

		Task Notify(
			TArgument argument,
			TValue value);
	}
}