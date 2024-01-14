using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Time.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Synchronization.Factories
{
	public static partial class SynchronizationFactory
	{
		public const string FIXED_DELTA_TIMER_ID = "Fixed delta timer";

		public static SynchronizationContext BuildSynchronizationContext(
			string id,
			bool canBeToggled,
			ILoggerResolver loggerResolver = null)
		{
			var descriptor = new SynchronizationDescriptor(
				id);

			var metadata = RepositoriesFactory.BuildDictionaryObjectRepository();

			if (canBeToggled)
				metadata.Add(
					typeof(ITogglable),
					new TogglableMetadata());

			var pinger = DelegatesFactory.BuildNonAllocPinger(loggerResolver);

			return new SynchronizationContext(
				descriptor,
				metadata,
				pinger,
				pinger);
		}

		public static SynchronizationContextGeneric<TDelta> BuildSynchronizationContextGeneric<TDelta>(
			string id,

			bool canBeToggled = true,
			bool active = true,
			
			bool canScale = false,
			TDelta scale = default,
			Func<TDelta, TDelta, TDelta> scaleDeltaDelegate = null,

			ILoggerResolver loggerResolver = null)
		{
			var descriptor = new SynchronizationDescriptor(
				id);

			var metadata = RepositoriesFactory.BuildDictionaryObjectRepository();

			if (canBeToggled)
				metadata.Add(
					typeof(ITogglable),
					new TogglableMetadata(active));

			if (canScale)
				metadata.Add(
					typeof(IScalable<TDelta>),
					new ScalableMetadata<TDelta>(
						scale,
						scaleDeltaDelegate));

			var broadcaster = DelegatesFactory.BuildNonAllocBroadcasterGeneric<TDelta>(
				loggerResolver);

			return new SynchronizationContextGeneric<TDelta>(
				descriptor,
				metadata,
				broadcaster,
				broadcaster);
		}

		public static SynchronizationContextGeneric<TDelta> BuildFixedDeltaSynchronizationContextGeneric<TDelta>(
			string id,

			TDelta fixedDelta,
			Func<TDelta, float> deltaToFloatDelegate,

			bool canBeToggled = true,
			bool active = true,

			bool canScale = false,
			TDelta scale = default,
			Func<TDelta, TDelta, TDelta> scaleDeltaDelegate = null,

			ILoggerResolver loggerResolver = null)
		{
			var descriptor = new SynchronizationDescriptor(
				id);

			var metadata = RepositoriesFactory.BuildDictionaryObjectRepository();

			if (canBeToggled)
				metadata.Add(
					typeof(ITogglable),
					new TogglableMetadata(active));

			if (canScale)
				metadata.Add(
					typeof(IScalable<TDelta>),
					new ScalableMetadata<TDelta>(
						scale,
						scaleDeltaDelegate));

			var fixedDeltaTimer = TimeFactory.BuildRuntimeTimer(
				FIXED_DELTA_TIMER_ID,
				deltaToFloatDelegate(fixedDelta),
				loggerResolver);

			metadata.Add(
				typeof(IHasFixedDelta<TDelta>),
				new HasFixedDeltaMetadata<TDelta>(
					fixedDelta,
					fixedDeltaTimer,
					fixedDeltaTimer,
					deltaToFloatDelegate,
					loggerResolver));

			var broadcaster = DelegatesFactory.BuildNonAllocBroadcasterGeneric<TDelta>(
				loggerResolver);

			return new SynchronizationContextGeneric<TDelta>(
				descriptor,
				metadata,
				broadcaster,
				broadcaster);
		}

		public static SynchronizationManager BuildSynchronizationManager()
		{
			var synchroRepository = RepositoriesFactory.BuildDictionaryRepository<string, ISynchronizableNoArgs>();

			return new SynchronizationManager(
				synchroRepository);
		}
	}
}