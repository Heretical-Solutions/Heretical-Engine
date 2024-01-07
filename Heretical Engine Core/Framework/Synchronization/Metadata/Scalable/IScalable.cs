namespace HereticalSolutions.Synchronization
{
	public interface IScalable<TDelta>
	{
		TDelta ScaleFactor { get; }

		void SetScale(TDelta scale);

		TDelta Scale(TDelta value);
	}
}