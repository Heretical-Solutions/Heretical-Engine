using System;
using System.Threading.Tasks;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.ResourceManagement
{
	public class ReadWriteResourceStorageHandle<TResource>
		: AReadWriteResourceStorageHandle<TResource>
	{
		private TResource defaultValue;

		public ReadWriteResourceStorageHandle(
			TResource defaultValue,
			ApplicationContext context)
			: base(
				context)
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