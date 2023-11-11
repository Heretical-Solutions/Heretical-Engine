using HereticalSolutions.Collections.Managed;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Logging;
using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public abstract class AResourceStorageHandle<TResource>
		: IResourceStorageHandle
	{
		protected readonly string[] dependencies;

		protected readonly SemaphoreSlim semaphore;

		protected readonly ApplicationContext context;


		protected bool allocated = false;

		protected TResource resource = default;

		public AResourceStorageHandle(
			string[] dependencies,
			SemaphoreSlim semaphore,
			ApplicationContext context)
		{
			this.dependencies = dependencies;

			this.semaphore = semaphore;

			this.context = context;


			resource = default;

			allocated = false;
		}

		#region IReadOnlyResourceStorageHandle

		#region IAllocatable

		public bool Allocated
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					return allocated;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		public virtual async Task Allocate(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				if (allocated)
				{
					progress?.Report(1f);

					return;
				}

				context.Logger.Log(
					GetType(),
					$"ALLOCATING");

				foreach (var dependency in dependencies)
				{
					var resourceStorageHandle = context
						.RuntimeResourceManager
						.GetResourceWhenAvailable(
							resourceID.SplitAddressBySeparator())
						.GetVariantWhenAvailable(
							MeshRAMAssetImporter.MESH_RAM_VARIANT_ID)
						.StorageHandle;

					if (!geometryRAMStorageHandle.Allocated)
					{
						IProgress<float> localProgress = progress.CreateLocalProgress();

						await geometryRAMStorageHandle
							.Allocate(
								localProgress)
							.ThrowExceptions<ConcurrentGeometryOpenGLStorageHandle>(logger);
					}
				}

				if (!geometryRAMStorageHandle.Allocated)
				{
					IProgress<float> localProgress = progress.CreateLocalProgress();

					await geometryRAMStorageHandle
						.Allocate(
							localProgress)
						.ThrowExceptions<ConcurrentGeometryOpenGLStorageHandle>(logger);
				}

				if (!shaderStorageHandle.Allocated)
				{
					IProgress<float> localProgress = progress.CreateLocalProgress();

					await shaderStorageHandle
						.Allocate(
							localProgress)
						.ThrowExceptions<GeometryOpenGLStorageHandle>(logger);
				}

				Action buildShaderDelegate = () =>
				{
					geometry = GeometryFactory.BuildGeometryOpenGL(
						cachedGL,
						geometryRAMStorageHandle.GetResource<Geometry>(),
						shaderStorageHandle.GetResource<ShaderOpenGL>().Descriptor,
						logger);
				};

				var command = new MainThreadCommand(
					buildShaderDelegate);

				while (!mainThreadCommandBuffer.TryProduce(
					command))
				{
					await Task.Yield();
				}

				while (command.Status != ECommandStatus.DONE)
				{
					await Task.Yield();
				}

				allocated = true;

				logger.Log<ConcurrentGeometryOpenGLStorageHandle>(
					$"ALLOCATED");
			}
			finally
			{
				semaphore.Release(); // Release the semaphore

				progress?.Report(1f);
			}
		}

		public async Task Free(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				if (!allocated)
				{
					progress?.Report(1f);

					return;
				}

				logger.Log<ConcurrentGeometryOpenGLStorageHandle>(
					$"FREEING");

				//geometry.Dispose(cachedGL);

				Action deleteShaderDelegate = () =>
				{
					geometry.Dispose(cachedGL);
				};

				var command = new MainThreadCommand(
					deleteShaderDelegate);

				while (!mainThreadCommandBuffer.TryProduce(
					command))
				{
					await Task.Yield();
				}

				while (command.Status != ECommandStatus.DONE)
				{
					await Task.Yield();
				}

				geometry = null;

				allocated = false;

				logger.Log<ConcurrentGeometryOpenGLStorageHandle>(
					$"FREE");
			}
			finally
			{
				semaphore.Release(); // Release the semaphore

				progress?.Report(1f);
			}
		}

		#endregion

		public object RawResource
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					if (!allocated)
						throw new InvalidOperationException("Resource is not allocated.");

					return geometry;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		public TValue GetResource<TValue>()
		{
			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!allocated)
					throw new InvalidOperationException("Resource is not allocated.");

				return (TValue)(object)geometry;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		#endregion
	}
}