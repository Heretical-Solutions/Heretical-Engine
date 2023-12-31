using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.ResourceManagement
{
	public abstract class AReadOnlyResourceStorageHandle<TResource>
		: AResourceStorageHandle<TResource>,
		  IReadOnlyResourceStorageHandle
	{
		public AReadOnlyResourceStorageHandle(
			ApplicationContext context)
			: base (
				context)
		{
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

			context.Logger?.Log(
				GetType(),
				$"ALLOCATING");

			resource = await AllocateResource(
				progress)
				.ThrowExceptions(
					GetType(),
					context.Logger);

			allocated = true;

			context.Logger?.Log(
				GetType(),
				$"ALLOCATED");

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

			context.Logger?.Log(
				GetType(),
				$"FREEING");

			await FreeResource(
				resource,
				progress)
				.ThrowExceptions(
					GetType(),
					context.Logger);

			resource = default;

			allocated = false;

			context.Logger?.Log(
				GetType(),
				$"FREE");

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

			switch (resource)
			{
				case TValue targetTypeResource:

					return targetTypeResource;

				default:

					context.Logger?.ThrowException(
						GetType(),
						$"CANNOT GET RESOURCE OF TYPE {typeof(TValue).Name} FROM RESOURCE OF TYPE {typeof(TResource).Name}");

					return default;
			}
		}

		#endregion
	}
}