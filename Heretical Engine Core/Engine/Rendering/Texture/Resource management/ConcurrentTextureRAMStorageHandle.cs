#define LOAD_IMAGES_ASYNC

using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Persistence.IO;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ConcurrentTextureRAMStorageHandle : IReadOnlyResourceStorageHandle
	{
		private readonly FilePathSettings filePathSettings;

		private readonly ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer;

		private readonly ReaderWriterLockSlim readWriteLock;

		private readonly IFormatLogger logger;


		private bool allocated = false;

		private Image<Rgba32> texture = null;

		public ConcurrentTextureRAMStorageHandle(
			FilePathSettings filePathSettings,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			ReaderWriterLockSlim readWriteLock,
			IFormatLogger logger)
		{
			this.filePathSettings = filePathSettings;

			this.mainThreadCommandBuffer = mainThreadCommandBuffer;

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

#if LOAD_IMAGES_ASYNC
				//The LoadAsync method is not thread safe somehow and throws exceptions if called not from the main thread.
				//Whatever, we have main thread commands now
				Func<Task> loadTextureDelegate = async () =>
				{
					logger.Log<ConcurrentTextureRAMStorageHandle>(
						$"INITIATING ASYNC TEXTURE LOADING. THREAD ID: {Thread.CurrentThread.ManagedThreadId}");

					texture = await Image
						.LoadAsync<Rgba32>(
							filePathSettings.FullPath)
						.ThrowExceptions<Image<Rgba32>, ConcurrentTextureRAMStorageHandle>(logger);

					logger.Log<ConcurrentTextureRAMStorageHandle>(
						$"DONE. TEXTURE IS LOADED: {(texture != default).ToString()}");
				};

				var command = new MainThreadCommand(
					loadTextureDelegate);

				while (!mainThreadCommandBuffer.TryProduce(
					command))
				{
					await Task.Yield();
				}

				while (command.Status != ECommandStatus.DONE)
				{
					await Task.Yield();
				}
#else
                texture = Image.Load<Rgba32>(filePathSettings.FullPath);
#endif

				allocated = true;
			}
			finally
			{
				readWriteLock.ExitWriteLock();

				progress?.Report(1f);
			}
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
				
				texture.Dispose();
				
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

				return (TValue)(object)texture; //DO NOT REPEAT
			}
			finally
			{
				readWriteLock.ExitReadLock();
			}
		}

		#endregion
	}
}