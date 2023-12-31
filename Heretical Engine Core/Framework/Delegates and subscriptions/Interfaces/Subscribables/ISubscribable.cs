namespace HereticalSolutions.Delegates
{
	public interface ISubscribable
	{
		IEnumerable<object> AllSubscriptions { get; }

		void UnsubscribeAll();
	}
}