using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public abstract class AReadOnlyResourceStorageHandle<TResource>
		: AResourceStorageHandle<TResource>,
		  IReadOnlyResourceStorageHandle
	{
		public AReadOnlyResourceStorageHandle(
			IRuntimeResourceManager runtimeResourceManager,
			IFormatLogger logger = null)
			: base (
				runtimeResourceManager,
				logger)
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

			logger?.Log(
				GetType(),
				$"ALLOCATING");

			resource = await AllocateResource(
				progress)
				.ThrowExceptions(
					GetType(),
					logger);

			allocated = true;

			logger?.Log(
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

			logger?.Log(
				GetType(),
				$"FREEING");

			await FreeResource(
				resource,
				progress)
				.ThrowExceptions(
					GetType(),
					logger);

			resource = default;

			allocated = false;

			logger?.Log(
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
					logger?.ThrowException(
						GetType(),
						"RESOURCE IS NOT ALLOCATED");

				return resource;
			}
		}

		public TValue GetResource<TValue>()
		{
			if (!allocated)
				logger?.ThrowException(
					GetType(),
					"RESOURCE IS NOT ALLOCATED");

			switch (resource)
			{
				case TValue targetTypeResource:

					return targetTypeResource;

				default:

					logger?.ThrowException(
						GetType(),
						$"CANNOT GET RESOURCE OF TYPE {typeof(TValue).Name} FROM RESOURCE OF TYPE {typeof(TResource).Name}");

					return default;
			}
		}

		#endregion
	}
}