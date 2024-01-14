using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public class ReadWriteResourceStorageHandle<TResource>
		: AReadWriteResourceStorageHandle<TResource>
	{
		private TResource defaultValue;

		public ReadWriteResourceStorageHandle(
			TResource defaultValue,
			IRuntimeResourceManager runtimeResourceManager,
			IFormatLogger logger = null)
			: base(
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