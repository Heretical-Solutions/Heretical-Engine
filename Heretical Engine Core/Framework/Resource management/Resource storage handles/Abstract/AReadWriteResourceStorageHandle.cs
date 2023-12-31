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

			switch (resource)
			{
				case TResource targetTypeResource:

					this.resource = targetTypeResource;

					break;

				default:

					context.Logger?.ThrowException(
							GetType(),
							$"CANNOT SET RESOURCE OF TYPE {typeof(TValue).Name} TO RESOURCE OF TYPE {typeof(TResource).Name}");

					break;
			}

			return true;
		}

		#endregion
	}
}