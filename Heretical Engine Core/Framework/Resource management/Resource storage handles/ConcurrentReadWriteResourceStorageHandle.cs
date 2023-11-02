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

		private ReaderWriterLockSlim readWriteLock;

		public ConcurrentReadWriteResourceStorageHandle(
			object rawResource,
			ReaderWriterLockSlim readWriteLock)
		{
			this.rawResource = rawResource;

			this.readWriteLock = readWriteLock;

			allocated = true;
		}

		#region IResourceStorageHandle

		#region IReadOnlyResourceStorageHandle

		#region IAllocatable

		public bool Allocated
		{
			get
			{
				readWriteLock.EnterReadLock();

				try
				{
					return allocated;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
		}

		public async Task Allocate(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			readWriteLock.EnterWriteLock();

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
				readWriteLock.ExitWriteLock();

				progress?.Report(1f);
			}
		}

		public async Task Free(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			readWriteLock.EnterWriteLock();

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
				readWriteLock.ExitWriteLock();

				progress?.Report(1f);
			}
		}

		#endregion

		public object RawResource
		{
			get
			{
				readWriteLock.EnterReadLock();

				try
				{
					if (!allocated)
						throw new InvalidOperationException("Resource is not allocated");

					return rawResource;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
		}

		public TValue GetResource<TValue>()
		{
			readWriteLock.EnterReadLock();

			try
			{
				if (!allocated)
					throw new InvalidOperationException("Resource is not allocated");

				return (TValue)rawResource;
			}
			finally
			{
				readWriteLock.ExitReadLock();
			}
		}

		#endregion

		public bool SetRawResource(object rawResource)
		{
			readWriteLock.EnterWriteLock();

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
				readWriteLock.ExitWriteLock();
			}
		}

		public bool SetResource<TValue>(TValue resource)
		{
			readWriteLock.EnterWriteLock();

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
				readWriteLock.ExitWriteLock();
			}
		}

		#endregion
	}
}