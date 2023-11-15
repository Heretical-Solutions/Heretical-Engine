using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.ResourceManagement
{
	public abstract class AConcurrentReadWriteResourceStorageHandle<TResource>
		: AConcurrentReadOnlyResourceStorageHandle<TResource>,
		  IResourceStorageHandle
	{
		public AConcurrentReadWriteResourceStorageHandle(
			SemaphoreSlim semaphore,
			ApplicationContext context)
			: base(
				semaphore,
				context)
		{
		}

		#region IResourceStorageHandle

		public bool SetRawResource(object rawResource)
		{
			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!allocated)
				{
					return false;
				}

				this.resource = (TResource)rawResource;

				return true;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		public bool SetResource<TValue>(TValue resource)
		{
			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!allocated)
				{
					return false;
				}

				this.resource = (TResource)(object)resource; //DO NOT REPEAT

				return true;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		#endregion
	}
}