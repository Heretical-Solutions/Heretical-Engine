using System;
using System.Threading.Tasks;
using System.Threading;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.ResourceManagement
{
	public class ConcurrentPreallocatedResourceStorageHandle
		: AConcurrentReadOnlyResourceStorageHandle<object>
	{
		private object value;

		public ConcurrentPreallocatedResourceStorageHandle(
			object value,
			SemaphoreSlim semaphore,
			ApplicationContext context)
			: base(
				semaphore,
				context)
		{
			this.value = value;
		}

        protected override async Task<object> AllocateResource(
			IProgress<float> progress = null)
        {
			return value;
        }

        protected override async Task FreeResource(
			object resource,
			IProgress<float> progress = null)
        {
        }
    }
}