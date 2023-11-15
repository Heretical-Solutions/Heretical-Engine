using System;
using System.Threading.Tasks;
using System.Threading;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.ResourceManagement
{
	public class ConcurrentPreallocatedResourceStorageHandle<TResource>
		: AConcurrentReadOnlyResourceStorageHandle<TResource>
	{
		private TResource value;

		public ConcurrentPreallocatedResourceStorageHandle(
			TResource value,
			SemaphoreSlim semaphore,
			ApplicationContext context)
			: base(
				semaphore,
				context)
		{
			this.value = value;
		}

        protected override async Task<TResource> AllocateResource(
			IProgress<float> progress = null)
        {
			return value;
        }

        protected override async Task FreeResource(
			TResource resource,
			IProgress<float> progress = null)
        {
        }
    }
}