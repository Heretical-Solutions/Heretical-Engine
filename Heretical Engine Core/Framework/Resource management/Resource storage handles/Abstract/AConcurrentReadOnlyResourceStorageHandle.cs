using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.ResourceManagement
{
	public abstract class AConcurrentReadOnlyResourceStorageHandle<TResource>
		: AResourceStorageHandle<TResource>,
		  IReadOnlyResourceStorageHandle
	{
		protected readonly SemaphoreSlim semaphore;

		public AConcurrentReadOnlyResourceStorageHandle(
			SemaphoreSlim semaphore,
			ApplicationContext context)
			: base(
				context)
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

				context.Logger?.Log(
					GetType(),
					$"ALLOCATING");

				resource = await AllocateResource(
					progress)
					.ThrowExceptions(
						GetType(),
						context.Logger);

				allocated = true;

				context.Logger?.Log(
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

				context.Logger?.Log(
					GetType(),
					$"FREEING");

				await FreeResource(
					resource,
					progress)
					.ThrowExceptions(
						GetType(),
						context.Logger);

				resource = default;

				allocated = false;

				context.Logger?.Log(
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
						context.Logger?.ThrowException(
							GetType(),
							"RESOURCE IS NOT ALLOCATED");

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
					context.Logger?.ThrowException(
						GetType(),
						"RESOURCE IS NOT ALLOCATED");

				return (TValue)(object)resource;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		#endregion
	}
}