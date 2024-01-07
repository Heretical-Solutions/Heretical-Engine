namespace HereticalSolutions.Synchronization
{
	public class ScalableMetadata<TDelta>
		: IScalable<TDelta>
	{
		//TODO:
		//https://stackoverflow.com/questions/5997107/is-there-a-generic-constraint-i-could-use-for-the-operator
		//https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/static-virtual-interface-members
		private readonly Func<TDelta, TDelta, TDelta> scaleDeltaDelegate;

		public ScalableMetadata(
			TDelta scale,
			Func<TDelta, TDelta, TDelta> scaleDeltaDelegate)
		{
			ScaleFactor = scale;

			this.scaleDeltaDelegate = scaleDeltaDelegate;
		}

		#region IScalable

		public TDelta ScaleFactor { get; private set; }

		public void SetScale(TDelta scale)
		{
			ScaleFactor = scale;
		}

		public TDelta Scale(TDelta value)
		{
			return scaleDeltaDelegate(
				value,
				ScaleFactor);
		}

		#endregion
	}
}