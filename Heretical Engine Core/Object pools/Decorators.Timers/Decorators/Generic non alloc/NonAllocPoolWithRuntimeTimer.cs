using System;

using HereticalSolutions.Pools.Arguments;

using HereticalSolutions.Time;

namespace HereticalSolutions.Pools.Decorators
{
	/// <summary>
	/// A decorator pool that adds runtime timer functionality to a non-allocating pool.
	/// </summary>
	/// <typeparam name="T">The type of object held by the pool.</typeparam>
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
			ISynchronizationProvider provider)
			: base(innerPool)
		{
			this.provider = provider;
		}
		
		/// <summary>
		/// Callback method called after an object is popped from the pool.
		/// </summary>
		/// <param name="instance">The instance that was popped from the pool.</param>
		/// <param name="args">The arguments associated with the instance.</param>
		protected override void OnAfterPop(
			IPoolElement<T> instance,
			IPoolDecoratorArgument[] args)
		{
			if (!instance.Metadata.Has<IContainsRuntimeTimer>())
				throw new Exception("[NonAllocPoolWithRuntimeTimer] INVALID INSTANCE");

			var metadata = instance.Metadata.Get<IContainsRuntimeTimer>();
			
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
				throw new Exception("[NonAllocPoolWithRuntimeTimer] INVALID INSTANCE");

			var metadata = instance.Metadata.Get<IContainsRuntimeTimer>();
			
			metadata.RuntimeTimer.Reset();
			
			if (metadata.UpdateSubscription.Active)
				provider.Unsubscribe(metadata.UpdateSubscription);
			
			if (metadata.PushSubscription.Active)
				metadata.RuntimeTimer.OnFinish.Unsubscribe(metadata.PushSubscription);
		}
	}
}