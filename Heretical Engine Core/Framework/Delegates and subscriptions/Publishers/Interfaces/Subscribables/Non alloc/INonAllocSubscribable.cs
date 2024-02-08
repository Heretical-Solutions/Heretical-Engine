namespace HereticalSolutions.Delegates
{
	/// <summary>
	/// Represents a subscribable object that allows non-allocating subscriptions
	/// </summary>
	public interface INonAllocSubscribable
	{
		/// <summary>
		/// Gets all the subscriptions associated with this object
		/// </summary>
		IEnumerable<ISubscription> AllSubscriptions { get; }

		/// <summary>
		/// Unsubscribes all subscriptions associated with this object
		/// </summary>
		void UnsubscribeAll();
	}
}