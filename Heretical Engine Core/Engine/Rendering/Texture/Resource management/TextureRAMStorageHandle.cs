using System;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Persistence.IO;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureRAMStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		private readonly FilePathSettings filePathSettings;

		private readonly IFormatLogger logger;


		private bool allocated = false;

		private Image<Rgba32> texture = null;

		public TextureRAMStorageHandle(
			FilePathSettings filePathSettings,
			IFormatLogger logger)
		{
			this.filePathSettings = filePathSettings;

			this.logger = logger;


			texture = null;

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

			logger.Log<TextureRAMStorageHandle>($"ALLOCATING. CURRENT THREAD ID: {Thread.CurrentThread.ManagedThreadId}");

			//For some reason async version silently throws a task cancelled exception
			/*
			await Image
				.LoadAsync<Rgba32>(
					filePathSettings.FullPath)
				.ThrowExceptions<Image<Rgba32>, TextureRAMStorageHandle>(logger);
			*/

			texture = Image.Load<Rgba32>(filePathSettings.FullPath);

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

			texture.Dispose();

			texture = null;


			allocated = false;

			progress?.Report(1f);
		}

		#endregion

		public object RawResource
		{
			get
			{
				if (!allocated)
					throw new InvalidOperationException("Resource is not allocated.");

				return texture;
			}
		}

		public TValue GetResource<TValue>()
		{
			if (!allocated)
				throw new InvalidOperationException("Resource is not allocated.");

			return (TValue)(object)texture; //DO NOT REPEAT
		}

		#endregion
	}
}