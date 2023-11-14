using System;
using System.Threading.Tasks;
using System.Threading;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.ResourceManagement
{
	public class ConcurrentReadWriteResourceStorageHandle
		: AConcurrentReadWriteResourceStorageHandle<object>
	{
		private object defaultValue;

		public ConcurrentReadWriteResourceStorageHandle(
			object defaultValue,
			SemaphoreSlim semaphore,
			ApplicationContext context)
			: base(
				semaphore,
				context)
		{
			this.defaultValue = defaultValue;
		}

		protected override async Task<object> AllocateResource(
			IProgress<float> progress = null)
		{
			return defaultValue;
		}

		protected override async Task FreeResource(
			object resource,
			IProgress<float> progress = null)
		{
		}
	}
}