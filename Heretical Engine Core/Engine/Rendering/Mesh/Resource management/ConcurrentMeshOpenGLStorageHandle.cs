using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ConcurrentMeshOpenGLStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		private readonly IRuntimeResourceManager resourceManager = null;

		private readonly IReadOnlyResourceStorageHandle meshRAMStorageHandle = null;

		private readonly SemaphoreSlim semaphore;

		private readonly IFormatLogger logger;


		private bool allocated = false;

		private MeshOpenGL mesh = null;

		public ConcurrentMeshOpenGLStorageHandle(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle meshRAMStorageHandle,
			SemaphoreSlim semaphore,
			IFormatLogger logger)
		{
			this.resourceManager = resourceManager;

			this.meshRAMStorageHandle = meshRAMStorageHandle;

			this.semaphore = semaphore;

			this.logger = logger;


			mesh = null;

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

				logger.Log<ConcurrentMeshOpenGLStorageHandle>(
					$"ALLOCATING");

				bool result = await LoadMesh(
					resourceManager,
					meshRAMStorageHandle,
					progress)
					.ThrowExceptions<bool, ConcurrentMeshOpenGLStorageHandle>(logger);

				if (!result)
				{
					progress?.Report(1f);

					return;
				}

				allocated = true;

				logger.Log<ConcurrentMeshOpenGLStorageHandle>(
					$"ALLOCATED");
			}
			finally
			{
				semaphore.Release(); // Release the semaphore

				progress?.Report(1f);
			}
		}

		private async Task<bool> LoadMesh(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle meshRAMStorageHandle,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			if (!meshRAMStorageHandle.Allocated)
			{
				IProgress<float> localProgress = progress.CreateLocalProgress(
					0f,
					0.333f);

				await meshRAMStorageHandle
					.Allocate(
						localProgress)
					.ThrowExceptions<ConcurrentMeshOpenGLStorageHandle>(logger);
			}

			var meshDTO = meshRAMStorageHandle.GetResource<MeshDTO>();

			progress?.Report(0.333f);

			var geometryStorageHandle = resourceManager
				.GetResource(
					meshDTO.GeometryResourceID.SplitAddressBySeparator())
				.GetVariant(
					GeometryOpenGLAssetImporter.GEOMETRY_OPENGL_VARIANT_ID)
				.StorageHandle;

			if (!geometryStorageHandle.Allocated)
			{
				IProgress<float> localProgress = progress.CreateLocalProgress(
					0.333f,
					0.666f);

				await geometryStorageHandle
					.Allocate(
						localProgress)
					.ThrowExceptions<ConcurrentMeshOpenGLStorageHandle>(logger);
			}

			var geometry = geometryStorageHandle.GetResource<GeometryOpenGL>();

			progress?.Report(0.666f);

			var materialOpenGLStorageHandle = resourceManager
				.GetResource(
					meshDTO.MaterialResourceID.SplitAddressBySeparator())
				.GetVariant(
					MaterialOpenGLAssetImporter.MATERIAL_OPENGL_VARIANT_ID)
				.StorageHandle;

			if (!materialOpenGLStorageHandle.Allocated)
			{
				IProgress<float> localProgress = progress.CreateLocalProgress(
					0.666f,
					1f);

				await materialOpenGLStorageHandle
					.Allocate(
						localProgress)
					.ThrowExceptions<ConcurrentMeshOpenGLStorageHandle>(logger);
			}

			var material = materialOpenGLStorageHandle.GetResource<MaterialOpenGL>();

			mesh = new MeshOpenGL(
				geometry,
				material);

			progress?.Report(1f);

			return true;
		}

		public virtual async Task Free(
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

				logger.Log<ConcurrentMeshOpenGLStorageHandle>(
					$"FREEING");

				mesh = null;

				allocated = false;

				logger.Log<ConcurrentMeshOpenGLStorageHandle>(
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
						throw new InvalidOperationException("Resource is not allocated");

					return mesh;
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
					throw new InvalidOperationException("Resource is not allocated");

				return (TValue)(object)mesh;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		#endregion
	}
}