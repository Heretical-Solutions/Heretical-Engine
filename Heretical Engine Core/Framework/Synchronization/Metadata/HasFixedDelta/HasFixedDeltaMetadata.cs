using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Time;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Synchronization
{
	public class HasFixedDeltaMetadata<TDelta>
		: IHasFixedDelta<TDelta>,
		  IPublisherDependencyRecipient<TDelta>
	{
		private readonly IRuntimeTimer fixedDeltaTimer;

		private readonly ITickable fixedDeltaTimerAsTickable;

		private readonly Func<TDelta, float> deltaToFloatDelegate;

		private readonly ISubscription synchronizeFixedScubscription;

		private IPublisherSingleArgGeneric<TDelta> broadcasterAsPublisher;

		public HasFixedDeltaMetadata(
			TDelta fixedDelta,
			IRuntimeTimer fixedDeltaTimer,
			ITickable fixedDeltaTimerAsTickable,
			Func<TDelta, float> deltaToFloatDelegate,
			IFormatLogger logger)
		{
			FixedDelta = fixedDelta;

			this.fixedDeltaTimer = fixedDeltaTimer;

			this.fixedDeltaTimerAsTickable = fixedDeltaTimerAsTickable;

			this.deltaToFloatDelegate = deltaToFloatDelegate;


			synchronizeFixedScubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<IRuntimeTimer>(
				SynchronizeFixed,
				logger);

			fixedDeltaTimer.Repeat = true;

			fixedDeltaTimer.FlushTimeElapsedOnRepeat = false;

			fixedDeltaTimer.Reset(
				deltaToFloatDelegate(
					FixedDelta));

			fixedDeltaTimer.Start();

			fixedDeltaTimer.OnFinish.Subscribe(
				(ISubscriptionHandler<
					INonAllocSubscribableSingleArgGeneric<IRuntimeTimer>,
					IInvokableSingleArgGeneric<IRuntimeTimer>>)
					synchronizeFixedScubscription);
		}

		#region IHasFixedDelta

		public TDelta FixedDelta { get; private set; }

		public void SetFixedDelta(TDelta fixedDelta)
		{
			FixedDelta = fixedDelta;

			fixedDeltaTimer.Reset(
				deltaToFloatDelegate(
					FixedDelta));

			fixedDeltaTimer.Start();
		}

		public void Tick(TDelta delta)
		{
			fixedDeltaTimerAsTickable.Tick(
				deltaToFloatDelegate(
					delta));
		}

		#endregion

		#region IPublisherDependencyRecipient

		public IPublisherSingleArgGeneric<TDelta> BroadcasterAsPublisher
		{
			set
			{
				broadcasterAsPublisher = value;
			}
		}

		#endregion

		private void SynchronizeFixed(IRuntimeTimer timer)
		{
			broadcasterAsPublisher?.Publish(FixedDelta);
		}

		public void Dispose()
		{
			if (fixedDeltaTimer != null
				&& synchronizeFixedScubscription != null
				&& synchronizeFixedScubscription.Active)
			{
				fixedDeltaTimer.OnFinish.Unsubscribe(
					(ISubscriptionHandler<
						INonAllocSubscribableSingleArgGeneric<IRuntimeTimer>,
						IInvokableSingleArgGeneric<IRuntimeTimer>>)
						synchronizeFixedScubscription);
			}
		}
	}
}