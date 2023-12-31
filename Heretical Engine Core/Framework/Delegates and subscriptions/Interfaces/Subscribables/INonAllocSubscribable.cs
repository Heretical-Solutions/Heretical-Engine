namespace HereticalSolutions.Delegates
{
	public interface INonAllocSubscribable
	{
		IEnumerable<ISubscription> AllSubscriptions { get; }

		void UnsubscribeAll();
	}
}