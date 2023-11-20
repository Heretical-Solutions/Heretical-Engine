using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Persistence.IO;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureRAMStorageHandle
		: AReadOnlyResourceStorageHandle<Image<Rgba32>>
	{
		private readonly FilePathSettings filePathSettings;

		public TextureRAMStorageHandle(
			FilePathSettings filePathSettings,
			ApplicationContext context)
			: base (context)
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