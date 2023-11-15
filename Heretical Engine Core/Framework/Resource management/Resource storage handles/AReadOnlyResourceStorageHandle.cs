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

		protected async Task<IReadOnlyResourceStorageHandle> LoadDependency(
			string path,
			string variantID = null,
			IProgress<float> progress = null)
		{
			#region Get resource data

			var concurrentResourceManager = ((IAsyncContainsRootResources)context.RuntimeResourceManager);

			IReadOnlyResourceData dependencyResource;

			if (concurrentResourceManager != null)
			{
				dependencyResource = await concurrentResourceManager
					.GetResourceWhenAvailable(
						path.SplitAddressBySeparator())
					.ThrowExceptions<IReadOnlyResourceData>(
						GetType(),
						context.Logger);
			}
			else
			{
				dependencyResource = context.RuntimeResourceManager.GetResource(
					path.SplitAddressBySeparator());

				if (dependencyResource == null)
					context.Logger?.ThrowException(
						GetType(),
						$"RESOURCE {path} DOES NOT EXIST");
			}

			#endregion

			#region Get variant

			var concurrentDependencyResource = ((IAsyncContainsResourceVariants)dependencyResource);

			IResourceVariantData dependencyVariantData;

			if (concurrentDependencyResource != null)
			{
				if (string.IsNullOrEmpty(variantID))
				{
					dependencyVariantData = await concurrentDependencyResource
						.GetDefaultVariantWhenAvailable()
						.ThrowExceptions<IResourceVariantData>(
							GetType(),
							context.Logger);
				}
				else
				{
					dependencyVariantData = await concurrentDependencyResource
						.GetVariantWhenAvailable(
							variantID)
						.ThrowExceptions<IResourceVariantData>(
							GetType(),
							context.Logger);
				}
			}
			else
			{
				if (string.IsNullOrEmpty(variantID))
				{
					dependencyVariantData = dependencyResource.DefaultVariant;
				}
				else
				{
					dependencyVariantData = dependencyResource.GetVariant(
						variantID);
				}

				if (dependencyVariantData == null)
					context.Logger?.ThrowException(
						GetType(),
						$"VARIANT {(string.IsNullOrEmpty(variantID) ? "NULL" : variantID)} DOES NOT EXIST");
			}

			#endregion

			progress?.Report(0.5f);

			var dependencyStorageHandle = dependencyVariantData.StorageHandle;

			if (!dependencyStorageHandle.Allocated)
			{
				IProgress<float> localProgress = progress.CreateLocalProgress(
					0.5f,
					1f);

				await dependencyStorageHandle
					.Allocate(
						localProgress)
					.ThrowExceptions(
						GetType(),
						context.Logger);
			}

			progress?.Report(1f);

			return dependencyStorageHandle;
		}
	}
}