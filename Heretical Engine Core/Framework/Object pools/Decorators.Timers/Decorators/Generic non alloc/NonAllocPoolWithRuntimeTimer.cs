using HereticalSolutions.Pools.Arguments;

using HereticalSolutions.Synchronization;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Decorators
{
	public class NonAllocPoolWithRuntimeTimer<T> : ANonAllocDecoratorPool<T>
	{
		private readonly ISynchronizationProvider provider;

		/// <summary>
		/// Initializes a new instance of the <see cref="NonAllocPoolWithRuntimeTimer{T}"/> class.
		/// </summary>
		/// <param name="innerPool">The inner pool to decorate.</param>
		/// <param name="provider">The synchronization provider for the runtime timer.</param>
		public NonAllocPoolWithRuntimeTimer(
			INonAllocDecoratedPool<T> innerPool,
			ISynchronizationProvider provider,
			ILogger logger = null)
			: base(
				innerPool,
				logger)
		{
			this.provider = provider;
		}
		
		protected override void OnAfterPop(
			IPoolElement<T> instance,
			IPoolDecoratorArgument[] args)
		{
			if (!instance.Metadata.Has<IContainsRuntimeTimer>())
				throw new Exception(
					logger.TryFormat<NonAllocPoolWithRuntimeTimer<T>>(
						"INVALID INSTANCE"));

			var metadata = instance.Metadata.Get<IContainsRuntimeTimer>();

			if (args.TryGetArgument<DurationArgument>(out var arg))
			{
				float newDuration = arg.Duration;
				
				if (newDuration < 0f)
					return;

				metadata.RuntimeTimer.OnFinish.Subscribe(metadata.PushSubscription);

				provider.Subscribe(metadata.UpdateSubscription);

				metadata.RuntimeTimer.Start(newDuration);

				return;
			}

			if (metadata.RuntimeTimer.DefaultDuration < 0f)
				return;

			metadata.RuntimeTimer.OnFinish.Subscribe(metadata.PushSubscription);
			
			provider.Subscribe(metadata.UpdateSubscription);
			
			metadata.RuntimeTimer.Start();
		}

		/// <summary>
		/// Callback method called before an object is pushed back into the pool.
		/// </summary>
		/// <param name="instance">The instance that will be pushed back into the pool.</param>
		protected override void OnBeforePush(IPoolElement<T> instance)
		{
			if (!instance.Metadata.Has<IContainsRuntimeTimer>())
				throw new Exception(
					logger.TryFormat<NonAllocPoolWithRuntimeTimer<T>>(
						"INVALID INSTANCE"));

			var metadata = instance.Metadata.Get<IContainsRuntimeTimer>();
			
			metadata.RuntimeTimer.Reset();
			
			if (metadata.UpdateSubscription.Active)
				provider.Unsubscribe(metadata.UpdateSubscription);
			
			if (metadata.PushSubscription.Active)
				metadata.RuntimeTimer.OnFinish.Unsubscribe(metadata.PushSubscription);
		}
	}
}