using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.ResourceManagement
{
	public abstract class AReadOnlyResourceStorageHandle<TResource>
		: IReadOnlyResourceStorageHandle
	{
		protected readonly ApplicationContext context;


		protected bool allocated = false;

		protected TResource resource = default;

		public AReadOnlyResourceStorageHandle(
			ApplicationContext context)
		{
			this.context = context;


			resource = default;

			allocated = false;
		}

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

			resource = AllocateResource(
				progress);

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

			FreeResource(
				resource,
				progress);

			resource = default;

			allocated = false;

			progress?.Report(1f);
		}

		#endregion

		public object RawResource
		{
			get
			{
				if (!allocated)
					context.Logger?.ThrowException(
						GetType(),
						"RESOURCE IS NOT ALLOCATED");

				return resource;
			}
		}

		public TValue GetResource<TValue>()
		{
			if (!allocated)
				context.Logger?.ThrowException(
					GetType(),
					"RESOURCE IS NOT ALLOCATED");

			return (TValue)(object)resource; //DO NOT REPEAT
		}

		#endregion

		protected abstract TResource AllocateResource(
			IProgress<float> progress = null);

		protected abstract void FreeResource(
			TResource resource,
			IProgress<float> progress = null);
	}
}