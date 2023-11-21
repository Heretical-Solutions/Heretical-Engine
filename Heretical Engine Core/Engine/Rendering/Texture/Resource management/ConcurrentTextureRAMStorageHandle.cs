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

			//texture = Image.Load<Rgba32>(filePathSettings.FullPath);

			//The LoadAsync method is not thread safe somehow and throws exceptions if called not from the main thread.
			//Whatever, we have main thread commands now
			Func<Task> loadTextureDelegate = async () =>
			{
				//context.Logger?.Log<ConcurrentTextureRAMStorageHandle>(
				//	$"INITIATING ASYNC TEXTURE LOADING. THREAD ID: {Thread.CurrentThread.ManagedThreadId}");

				texture = await Image
					.LoadAsync<Rgba32>(
						filePathSettings.FullPath)
					.ThrowExceptions<Image<Rgba32>, ConcurrentTextureRAMStorageHandle>(context.Logger);

				//context.Logger?.Log<ConcurrentTextureRAMStorageHandle>(
				//	$"DONE. TEXTURE IS LOADED: {(texture != default).ToString()}");
			};

			await ExecuteOnMainThread(
				loadTextureDelegate)
				.ThrowExceptions<ConcurrentTextureRAMStorageHandle>(context.Logger);

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