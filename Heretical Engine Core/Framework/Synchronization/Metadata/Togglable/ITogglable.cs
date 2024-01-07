namespace HereticalSolutions.Synchronization
{
	public interface ITogglable
	{
		bool Active { get; }

		void Toggle(bool active);
	}
}