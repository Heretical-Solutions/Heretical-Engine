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

		private readonly ReaderWriterLockSlim readWriteLock;

		private readonly IFormatLogger logger;


		private bool allocated = false;

		private TextureOpenGL texture = null;

		public ConcurrentTextureOpenGLStorageHandle(
			TextureRAMStorageHandle textureRAMStorageHandle,
			TextureType textureType,
			GL gl,
			ReaderWriterLockSlim readWriteLock,
			IFormatLogger logger)
		{
			this.textureRAMStorageHandle = textureRAMStorageHandle;

			this.textureType = textureType;

			cachedGL = gl;

			this.readWriteLock = readWriteLock;

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
				readWriteLock.EnterReadLock();

				try
				{
					return allocated;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
		}

		public virtual async Task Allocate(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			readWriteLock.EnterWriteLock();

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
				readWriteLock.ExitWriteLock();

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

			readWriteLock.EnterWriteLock();

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
				readWriteLock.ExitWriteLock();

				progress?.Report(1f);
			}
		}

		#endregion

		public object RawResource
		{
			get
			{
				readWriteLock.EnterReadLock();

				try
				{
					if (!allocated)
						throw new InvalidOperationException("Resource is not allocated.");

					return texture;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
		}

		public TValue GetResource<TValue>()
		{
			readWriteLock.EnterReadLock();

			try
			{
				if (!allocated)
					throw new InvalidOperationException("Resource is not allocated.");

				return (TValue)(object)texture;
			}
			finally
			{
				readWriteLock.ExitReadLock();
			}
		}

		#endregion
	}
}
