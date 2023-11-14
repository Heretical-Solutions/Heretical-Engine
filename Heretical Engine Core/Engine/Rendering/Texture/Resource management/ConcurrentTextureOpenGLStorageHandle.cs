using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

using Silk.NET.Assimp;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ConcurrentTextureOpenGLStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		private readonly IReadOnlyResourceStorageHandle textureRAMStorageHandle = null;

		private readonly TextureType textureType;

		private readonly GL cachedGL = default;

		private readonly ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer;

		private readonly SemaphoreSlim semaphore;

		private readonly IFormatLogger logger;


		private bool allocated = false;

		private TextureOpenGL texture = null;

		public ConcurrentTextureOpenGLStorageHandle(
			IReadOnlyResourceStorageHandle textureRAMStorageHandle,
			TextureType textureType,
			GL gl,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			SemaphoreSlim semaphore,
			IFormatLogger logger)
		{
			this.textureRAMStorageHandle = textureRAMStorageHandle;

			this.textureType = textureType;

			cachedGL = gl;

			this.mainThreadCommandBuffer = mainThreadCommandBuffer;

			this.semaphore = semaphore;

			this.logger = logger;


			texture = null;

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

				logger?.Log<ConcurrentTextureOpenGLStorageHandle>(
					$"ALLOCATING");

				Func<Task> loadTextureDelegate = async () =>
				{
					bool result = await LoadTexture(
						textureRAMStorageHandle,
						textureType,
						cachedGL,
						true,
						progress)
						.ThrowExceptions<bool, TextureOpenGLStorageHandle>(logger);

					if (!result)
					{
						progress?.Report(1f);

						return;
					}
				};

				var command = new MainThreadCommand(loadTextureDelegate);

				while (!mainThreadCommandBuffer.TryProduce(command))
				{
					await Task.Yield();
				}

				while (command.Status != ECommandStatus.DONE)
				{
					await Task.Yield();
				}

				allocated = true;

				logger?.Log<ConcurrentTextureOpenGLStorageHandle>(
					$"ALLOCATED");
			}
			finally
			{
				semaphore.Release(); // Release the semaphore

				progress?.Report(1f);
			}
		}

		private async Task<bool> LoadTexture(
			IReadOnlyResourceStorageHandle textureRAMStorageHandle,
			TextureType textureType,
			GL gl,
			bool freeRamAfterAllocation = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			if (!textureRAMStorageHandle.Allocated)
			{
				IProgress<float> localProgress = progress.CreateLocalProgress(
					0f,
					0.5f);

				await textureRAMStorageHandle
					.Allocate(
						localProgress)
					.ThrowExceptions<ConcurrentTextureOpenGLStorageHandle>(logger);
			}

			var ramTexture = textureRAMStorageHandle.GetResource<Image<Rgba32>>();

			texture = TextureFactory.BuildTextureOpenGL(
				gl,
				ramTexture,
				textureType);

			progress?.Report(0.5f);

			if (freeRamAfterAllocation)
			{
				IProgress<float> localProgress = progress.CreateLocalProgress(
					0.5f,
					1f);

				await textureRAMStorageHandle
					.Free(
						localProgress)
					.ThrowExceptions<ConcurrentTextureOpenGLStorageHandle>(logger);
			}

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

				logger?.Log<ConcurrentTextureOpenGLStorageHandle>(
					$"FREEING");

				//cachedGL.DeleteTexture(texture.Handle);

				Action deleteShaderDelegate = () =>
				{
					cachedGL.DeleteTexture(texture.Handle);
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

				texture = null;

				allocated = false;

				logger?.Log<ConcurrentTextureOpenGLStorageHandle>(
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

					return texture;
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

				return (TValue)(object)texture;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		#endregion
	}
}
