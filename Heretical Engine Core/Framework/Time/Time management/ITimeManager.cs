namespace HereticalSolutions.Time
{
	public interface ITimeManager
	{
		IRuntimeTimer ApplicationRuntimeTimer { get; }

		IPersistentTimer ApplicationPersistentTimer { get; }
	}
}