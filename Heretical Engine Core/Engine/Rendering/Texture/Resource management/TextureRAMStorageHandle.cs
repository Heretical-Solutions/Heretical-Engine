using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Persistence.IO;

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureRAMStorageHandle
		: AReadOnlyResourceStorageHandle<Image<Rgba32>>
	{
		private readonly FilePathSettings filePathSettings;

		private readonly IGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer;

		public TextureRAMStorageHandle(
			FilePathSettings filePathSettings,
			IGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IRuntimeResourceManager runtimeResourceManager,
			ILogger logger = null)
			: base (
				runtimeResourceManager,
				logger)
		{
			this.filePathSettings = filePathSettings;

			this.mainThreadCommandBuffer = mainThreadCommandBuffer;
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
				//context.Logger?.Log<TextureRAMStorageHandle>(
				//	$"INITIATING ASYNC TEXTURE LOADING. THREAD ID: {Thread.CurrentThread.ManagedThreadId}");

				texture = await Image
					.LoadAsync<Rgba32>(
						filePathSettings.FullPath)
					.ThrowExceptions<Image<Rgba32>, TextureRAMStorageHandle>(logger);

				//context.Logger?.Log<TextureRAMStorageHandle>(
				//	$"DONE. TEXTURE IS LOADED: {(texture != default).ToString()}");
			};

			await ExecuteOnMainThread(
				loadTextureDelegate,
				mainThreadCommandBuffer)
				.ThrowExceptions<TextureRAMStorageHandle>(logger);

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