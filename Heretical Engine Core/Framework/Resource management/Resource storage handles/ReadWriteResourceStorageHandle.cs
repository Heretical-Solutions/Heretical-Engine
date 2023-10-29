using System;
using System.Threading.Tasks;

namespace HereticalSolutions.ResourceManagement
{
	public class ReadWriteResourceStorageHandle
		: IResourceStorageHandle
	{
		private bool allocated = false;

		private object rawResource;

		public ReadWriteResourceStorageHandle(
			object rawResource)
		{
			this.rawResource = rawResource;

			allocated = true;
		}

		#region IResourceStorageHandle

		#region IReadOnlyResourceStorageHandle

		#region IAllocatable

		public bool Allocated
		{
			get => allocated;
		}

		public virtual async Task Allocate(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			if (allocated)
			{
				progress?.Report(1f);

				return;
			}

			allocated = true;

			progress?.Report(1f);
		}

		public virtual async Task Free(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			if (!allocated)
			{
				progress?.Report(1f);

				return;
			}

			allocated = false;

			progress?.Report(1f);
		}

		#endregion

		public object RawResource
		{
			get
			{
				if (!allocated)
					throw new InvalidOperationException("Resource is not allocated.");

				return rawResource;
			}
		}

		public TValue GetResource<TValue>()
		{
			if (!allocated)
				throw new InvalidOperationException("Resource is not allocated.");

			return (TValue)rawResource;
		}

		#endregion

		public bool SetRawResource(object rawResource)
		{
			if (!allocated)
			{
				return false;
			}

			this.rawResource = rawResource;

			return true;
		}

		public bool SetResource<TValue>(TValue resource)
		{
			if (!allocated)
			{
				return false;
			}

			rawResource = resource;

			return true;
		}

		#endregion
	}
}