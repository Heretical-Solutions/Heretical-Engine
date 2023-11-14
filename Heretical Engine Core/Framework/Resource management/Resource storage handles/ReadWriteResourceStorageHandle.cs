using System;
using System.Threading.Tasks;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.ResourceManagement
{
	public class ReadWriteResourceStorageHandle
		: AReadWriteResourceStorageHandle<object>
	{
		private object defaultValue;

		public ReadWriteResourceStorageHandle(
			object defaultValue,
			ApplicationContext context)
			: base(
				context)
		{
			this.defaultValue = defaultValue;
		}

		protected override object AllocateResource(
			IProgress<float> progress = null)
		{
			return defaultValue;
		}

		protected override void FreeResource(
			object resource,
			IProgress<float> progress = null)
		{
		}
	}
}