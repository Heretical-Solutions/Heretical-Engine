#define LOAD_IMAGES_ASYNC

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Persistence.IO;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ConcurrentTextureRAMStorageHandle
		: AConcurrentReadOnlyResourceStorageHandle<Image<Rgba32>>
	{
		private readonly FilePathSettings filePathSettings;

		public ConcurrentTextureRAMStorageHandle(
			FilePathSettings filePathSettings,
			SemaphoreSlim semaphore,
			ApplicationContext context)
			: base(
				semaphore,
				context)
		{
			this.filePathSettings = filePathSettings;
		}

		protected override async Task<Image<Rgba32>> AllocateResource(
			IProgress<float> progress = null)
		{
			Image<Rgba32> texture = default;

#if LOAD_IMAGES_ASYNC
			//The LoadAsync method is not thread safe somehow and throws exceptions if called not from the main thread.
			//Whatever, we have main thread commands now
			Func<Task> loadTextureDelegate = async () =>
			{
				context.Logger?.Log<TextureRAMStorageHandle>(
					$"INITIATING ASYNC TEXTURE LOADING. THREAD ID: {Thread.CurrentThread.ManagedThreadId}");

				texture = await Image
					.LoadAsync<Rgba32>(
						filePathSettings.FullPath)
					.ThrowExceptions<Image<Rgba32>, TextureRAMStorageHandle>(context.Logger);

				context.Logger?.Log<TextureRAMStorageHandle>(
					$"DONE. TEXTURE IS LOADED: {(texture != default).ToString()}");
			};

			await ExecuteOnMainThread(
				loadTextureDelegate)
				.ThrowExceptions<TextureRAMStorageHandle>(context.Logger);
#else
			texture = Image.Load<Rgba32>(filePathSettings.FullPath);
#endif

			return texture;
		}

		protected override async Task FreeResource(
			Image<Rgba32> resource,
			IProgress<float> progress = null)
		{
			resource.Dispose();
		}
	}
}