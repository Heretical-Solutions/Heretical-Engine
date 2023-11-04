using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Messaging;

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

		private readonly ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer;

		private readonly IFormatLogger logger;


		private bool allocated = false;

		private GeometryOpenGL geometry = null;

		public ConcurrentGeometryOpenGLStorageHandle(
			IReadOnlyResourceStorageHandle geometryRAMStorageHandle,
			SemaphoreSlim semaphore,
			GL gl,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
		{
			this.geometryRAMStorageHandle = geometryRAMStorageHandle;

			this.semaphore = semaphore;

			cachedGL = gl;

			this.mainThreadCommandBuffer = mainThreadCommandBuffer;

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

				logger.Log<ConcurrentGeometryOpenGLStorageHandle>(
					$"ALLOCATING");

				if (!geometryRAMStorageHandle.Allocated)
				{
					IProgress<float> localProgress = progress.CreateLocalProgress();

					await geometryRAMStorageHandle
						.Allocate(
							localProgress)
						.ThrowExceptions<ConcurrentGeometryOpenGLStorageHandle>(logger);
				}

				/*
				geometry = GeometryFactory.BuildGeometryOpenGL(
					cachedGL,
					geometryRAMStorageHandle.GetResource<Geometry>());
				*/

				Action buildShaderDelegate = () =>
				{
					geometry = GeometryFactory.BuildGeometryOpenGL(
						cachedGL,
						geometryRAMStorageHandle.GetResource<Geometry>());
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