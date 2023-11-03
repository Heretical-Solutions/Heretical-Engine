using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ConcurrentGeometryOpenGLStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		private readonly IReadOnlyResourceStorageHandle geometryRAMStorageHandle = null;

		private readonly SemaphoreSlim semaphore;

		private readonly GL cachedGL = default;

		private readonly IFormatLogger logger;


		private bool allocated = false;

		private GeometryOpenGL geometry = null;

		public ConcurrentGeometryOpenGLStorageHandle(
			IReadOnlyResourceStorageHandle geometryRAMStorageHandle,
			SemaphoreSlim semaphore,
			GL gl,
			IFormatLogger logger)
		{
			this.geometryRAMStorageHandle = geometryRAMStorageHandle;

			this.semaphore = semaphore;

			cachedGL = gl;

			this.logger = logger;


			geometry = null;

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

		public async Task Allocate(
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

				if (!geometryRAMStorageHandle.Allocated)
				{
					IProgress<float> localProgress = null;

					if (progress != null)
					{
						var localProgressInstance = new Progress<float>();

						localProgressInstance.ProgressChanged += (sender, value) =>
						{
							progress.Report(value);
						};

						localProgress = localProgressInstance;
					}

					await geometryRAMStorageHandle
						.Allocate(
							localProgress)
						.ThrowExceptions<ConcurrentGeometryOpenGLStorageHandle>(logger);
				}

				geometry = GeometryFactory.BuildGeometryOpenGL(
					cachedGL,
					geometryRAMStorageHandle.GetResource<Geometry>());

				allocated = true;
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

				geometry.Dispose(cachedGL);

				geometry = null;

				allocated = false;
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