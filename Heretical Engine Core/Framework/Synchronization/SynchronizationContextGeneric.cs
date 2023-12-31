using HereticalSolutions.Delegates;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Synchronization
{
	public class SynchronizationContextGeneric<TDelta>
		: ISynchronizableGeneric<TDelta>,
		  ISynchronizationProvider
	{
		private SynchronizationDescriptor descriptor;

		private readonly IPublisherSingleArgGeneric<TDelta> broadcasterAsPublisher;

		private readonly INonAllocSubscribableSingleArgGeneric<TDelta> broadcasterAsSubscribable;

		private readonly Func<TDelta, float, TDelta> scaleDeltaDelegate;

		private readonly IFormatLogger logger;

		public SynchronizationContextGeneric(
			SynchronizationDescriptor descriptor,
			IPublisherSingleArgGeneric<TDelta> broadcasterAsPublisher,
			INonAllocSubscribableSingleArgGeneric<TDelta> broadcasterAsSubscribable,
			Func<TDelta, float, TDelta> scaleDeltaDelegate,
			IFormatLogger logger)
		{
			this.descriptor = descriptor;
			
			this.broadcasterAsPublisher = broadcasterAsPublisher;

			this.broadcasterAsSubscribable = broadcasterAsSubscribable;

			this.scaleDeltaDelegate = scaleDeltaDelegate;

			this.logger = logger;
		}

		#region ISynchronizable

		public SynchronizationDescriptor Descriptor { get => descriptor; }

		public void Toggle(bool active)
		{
			if (!descriptor.CanBeToggled)
			{
				logger?.ThrowException<SynchronizationContextGeneric<TDelta>>(
					"SYNCHRONIZATION IS TOGGLED WHILE ITS CONSTRAINED NOT TO BE ABLE TO BE TOGGLED");
			}

			descriptor.Active = active;
		}

		public void SetScale(float scale)
		{
			if (!descriptor.CanScale)
			{
				logger?.ThrowException<SynchronizationContextGeneric<TDelta>>(
					"SYNCHRONIZATION IS SCALED WHILE ITS CONSTRAINED NOT TO BE ABLE TO BE SCALED");
			}

			descriptor.Scale = scale;
		}

		public void Synchronize(TDelta delta)
		{
			if (!descriptor.CanBeToggled && !descriptor.Active)
			{
				logger?.ThrowException<SynchronizationContextGeneric<TDelta>>(
					"SYNCHRONIZATION IS TOGGLED WHILE ITS CONSTRAINED NOT TO BE ABLE TO BE TOGGLED");
			}

			if (!descriptor.Active)
				return;

			if (descriptor.CanScale)
			{
				var scaledDelta = scaleDeltaDelegate(delta, descriptor.Scale);

				broadcasterAsPublisher.Publish(scaledDelta);
			}
			else
				broadcasterAsPublisher.Publish(delta);
		}

		#endregion

		#region ISynchronizationProvider

		public void Subscribe(ISubscription subscription)
		{
			broadcasterAsSubscribable.Subscribe(
				(ISubscriptionHandler<
					INonAllocSubscribableSingleArgGeneric<TDelta>,
					IInvokableSingleArgGeneric<TDelta>>)
				subscription);
		}

		public void Unsubscribe(ISubscription subscription)
		{
			broadcasterAsSubscribable.Unsubscribe(
				(ISubscriptionHandler<
					INonAllocSubscribableSingleArgGeneric<TDelta>,
					IInvokableSingleArgGeneric<TDelta>>)
				subscription);
		}

		//TODO: Add UnsubscribeAll
		#endregion
	}
}