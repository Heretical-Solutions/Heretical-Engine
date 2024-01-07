namespace HereticalSolutions.Synchronization
{
	public interface ISynchronizableGenericArg<TDelta>
		: ISynchronizable
	{
		void Synchronize(TDelta delta);
	}
}