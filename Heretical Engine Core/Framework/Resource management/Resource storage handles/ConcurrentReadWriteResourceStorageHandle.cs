using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public class ConcurrentReadWriteResourceStorageHandle<TResource>
		: AConcurrentReadWriteResourceStorageHandle<TResource>
	{
		private TResource defaultValue;

		public ConcurrentReadWriteResourceStorageHandle(
			TResource defaultValue,
			SemaphoreSlim semaphore,
			IRuntimeResourceManager runtimeResourceManager,
			ILogger logger = null)
			: base(
				semaphore,
				runtimeResourceManager,
				logger)
		{
			this.defaultValue = defaultValue;
		}

		protected override async Task<TResource> AllocateResource(
			IProgress<float> progress = null)
		{
			return defaultValue;
		}

		protected override async Task FreeResource(
			TResource resource,
			IProgress<float> progress = null)
		{
		}
	}
}