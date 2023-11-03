using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

using Silk.NET.Assimp;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ConcurrentTextureOpenGLStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		private readonly TextureRAMStorageHandle textureRAMStorageHandle = null;

		private readonly TextureType textureType;

		private readonly GL cachedGL = default;

		private readonly SemaphoreSlim semaphore;

		private readonly IFormatLogger logger;


		private bool allocated = false;

		private TextureOpenGL texture = null;

		public ConcurrentTextureOpenGLStorageHandle(
			TextureRAMStorageHandle textureRAMStorageHandle,
			TextureType textureType,
			GL gl,
			SemaphoreSlim semaphore,
			IFormatLogger logger)
		{
			this.textureRAMStorageHandle = textureRAMStorageHandle;

			this.textureType = textureType;

			cachedGL = gl;

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

				bool result = await LoadTexture(
					textureRAMStorageHandle,
					textureType,
					cachedGL,
					true,
					progress)
					.ThrowExceptions<bool, ConcurrentTextureOpenGLStorageHandle>(logger);

				if (!result)
				{
					progress?.Report(1f);

					return;
				}

				allocated = true;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore

				progress?.Report(1f);
			}
		}

		private async Task<bool> LoadTexture(
			TextureRAMStorageHandle textureRAMStorageHandle,
			TextureType textureType,
			GL gl,
			bool freeRamAfterAllocation = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			if (!textureRAMStorageHandle.Allocated)
			{
				IProgress<float> localProgress = null;

				if (progress != null)
				{
					var localProgressInstance = new Progress<float>();

					localProgressInstance.ProgressChanged += (sender, value) =>
					{
						progress.Report(value * 0.5f);
					};

					localProgress = localProgressInstance;
				}

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
				IProgress<float> localProgress = null;

				if (progress != null)
				{
					var localProgressInstance = new Progress<float>();

					localProgressInstance.ProgressChanged += (sender, value) =>
					{
						progress.Report(value * 0.5f + 0.5f);
					};

					localProgress = localProgressInstance;
				}

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

				cachedGL.DeleteTexture(texture.Handle);

				texture = null;

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
