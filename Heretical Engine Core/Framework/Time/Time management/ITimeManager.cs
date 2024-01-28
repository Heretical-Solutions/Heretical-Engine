namespace HereticalSolutions.Time
{
	public interface ITimeManager
		: ITickable
	{
		IRuntimeTimer ApplicationRuntimeTimer { get; }

		IPersistentTimer ApplicationPersistentTimer { get; }
	}
}