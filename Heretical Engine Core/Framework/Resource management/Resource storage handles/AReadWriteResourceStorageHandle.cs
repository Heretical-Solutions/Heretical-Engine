using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.ResourceManagement
{
	public abstract class AReadWriteResourceStorageHandle<TResource>
		: AReadOnlyResourceStorageHandle<TResource>,
		  IResourceStorageHandle
	{
		public AReadWriteResourceStorageHandle(
			ApplicationContext context)
			: base(context)
		{
		}

		#region IResourceStorageHandle

		public bool SetRawResource(object rawResource)
		{
			if (!allocated)
			{
				return false;
			}

			this.resource = (TResource)rawResource;

			return true;
		}

		public bool SetResource<TValue>(TValue resource)
		{
			if (!allocated)
			{
				return false;
			}

			this.resource = (TResource)(object)resource; //DO NOT REPEAT

			return true;
		}

		#endregion
	}
}