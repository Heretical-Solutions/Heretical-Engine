using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Synchronization.Factories
{
	public static partial class SynchronizationFactory
	{
		public static SynchronizationContext BuildSynchronizationContext(
			string id,
			bool canBeToggled,
			IFormatLogger logger)
		{
			var pinger = DelegatesFactory.BuildNonAllocPinger(logger);

			return new SynchronizationContext(
				new SynchronizationDescriptor(
					id,
					canBeToggled,
					false),
				pinger,
				pinger,
				logger);
		}

		public static SynchronizationContextGeneric<TDelta> BuildSynchronizationContextGeneric<TDelta>(
			string id,
			bool canBeToggled,
			bool canScale,
			Func<TDelta, float, TDelta> scaleDeltaDelegate,
			IFormatLogger logger)
		{
			var broadcaster = DelegatesFactory.BuildNonAllocBroadcasterGeneric<TDelta>(
				logger);

			return new SynchronizationContextGeneric<TDelta>(
				new SynchronizationDescriptor(
					id,
					canBeToggled,
					canScale),
				broadcaster,
				broadcaster,
				scaleDeltaDelegate,
				logger);
		}
	}
}