using System;
using System.Threading.Tasks;
using System.Threading;

namespace HereticalSolutions.ResourceManagement
{
	public class ConcurrentReadWriteResourceStorageHandle
		: IResourceStorageHandle
	{
		private bool allocated = false;

		private object rawResource;

		private SemaphoreSlim semaphore;

		public ConcurrentReadWriteResourceStorageHandle(
			object rawResource,
			SemaphoreSlim semaphore)
		{
			this.rawResource = rawResource;

			this.semaphore = semaphore;

			allocated = true;
		}

		#region IResourceStorageHandle

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

		public async Task Allocate(
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

				allocated = true;
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

				allocated = false;
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
						throw new InvalidOperationException("Resource is not allocated");

					return rawResource;
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
					throw new InvalidOperationException("Resource is not allocated");

				return (TValue)rawResource;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		#endregion

		public bool SetRawResource(object rawResource)
		{
			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!allocated)
				{
					return false;
				}

				this.rawResource = rawResource;

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

				rawResource = resource;

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