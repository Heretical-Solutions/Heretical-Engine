using HereticalSolutions.Synchronization;

namespace HereticalSolutions.Time
{
	public interface ITimeManager
	{
		IRuntimeTimer ApplicationActiveTimer { get; }

		IPersistentTimer ApplicationPersistentTimer { get; }
	}
}