namespace HereticalSolutions.Delegates
{
	/// <summary>
	/// Represents an object that can be subscribed to by other objects
	/// </summary>
	public interface ISubscribable
	{
		/// <summary>
		/// Gets all the current subscriptions to this object
		/// </summary>
		IEnumerable<object> AllSubscriptions { get; }

		/// <summary>
		/// Unsubscribes all objects from this object
		/// </summary>
		void UnsubscribeAll();
	}
}