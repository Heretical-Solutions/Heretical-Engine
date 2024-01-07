namespace HereticalSolutions.Synchronization
{
	public interface IHasFixedDelta<TDelta>
	{
		TDelta FixedDelta { get; }

		void SetFixedDelta(TDelta fixedDelta);

		void Tick(TDelta delta);
	}
}