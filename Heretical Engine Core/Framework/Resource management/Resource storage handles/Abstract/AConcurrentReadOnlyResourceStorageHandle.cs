using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public abstract class AConcurrentReadOnlyResourceStorageHandle<TResource>
		: AResourceStorageHandle<TResource>,
		  IReadOnlyResourceStorageHandle,
		  ICleanUppable,
		  IDisposable
	{
		protected readonly SemaphoreSlim semaphore;

		public AConcurrentReadOnlyResourceStorageHandle(
			SemaphoreSlim semaphore,
			IRuntimeResourceManager runtimeResourceManager,
			ILogger logger = null)
			: base(
				runtimeResourceManager,
				logger)
		{
			this.semaphore = semaphore;
		}

		#region IReadOnlyResourceStorageHandle

		#region IAllocatable

		public bool Allocated
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					return allocated;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		public virtual async Task Allocate(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				if (allocated)
				{
					progress?.Report(1f);

					return;
				}

				logger?.Log(
					GetType(),
					$"ALLOCATING");

				resource = await AllocateResource(
					progress)
					.ThrowExceptions(
						GetType(),
						logger);

				allocated = true;

				logger?.Log(
					GetType(),
					$"ALLOCATED");
			}
			finally
			{
				semaphore.Release(); // Release the semaphore

				progress?.Report(1f);
			}
		}

		public async Task Free(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				if (!allocated)
				{
					progress?.Report(1f);

					return;
				}

				logger?.Log(
					GetType(),
					$"FREEING");

				await FreeResource(
					resource,
					progress)
					.ThrowExceptions(
						GetType(),
						logger);

				resource = default;

				allocated = false;

				logger?.Log(
					GetType(),
					$"FREE");
			}
			finally
			{
				semaphore.Release(); // Release the semaphore

				progress?.Report(1f);
			}
		}

		#endregion

		public object RawResource
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					if (!allocated)
						throw new Exception(
							logger.TryFormat(
								GetType(),
								"RESOURCE IS NOT ALLOCATED"));

					return resource;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		public TValue GetResource<TValue>()
		{
			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!allocated)
					throw new Exception(
						logger.TryFormat(
							GetType(),
							"RESOURCE IS NOT ALLOCATED"));

				switch (resource)
				{
					case TValue value:

						return value;

					default:

						throw new Exception(
							logger.TryFormat(
								GetType(),
								$"RESOURCE IS NOT OF TYPE {typeof(TValue).Name}"));
				}
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		#endregion

		#region ICleanUppable

		public new void Cleanup()
		{
			Free();
		}

		#endregion

		#region IDisposable

		public new void Dispose()
		{
			Free();
		}

		#endregion
	}
}