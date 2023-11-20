#define PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Application;

using Silk.NET.OpenGL;

using Silk.NET.Assimp;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureOpenGLStorageHandle
		: AReadOnlyResourceStorageHandle<TextureOpenGL>
	{
		private readonly string glPath;

		private readonly string textureRAMPath;

		private readonly string textureRAMVariantID;

		private readonly TextureType textureType;

		public TextureOpenGLStorageHandle(
			string glPath,
			string textureRAMPath,
			string textureRAMVariantID,
			TextureType textureType,
			ApplicationContext context)
			: base (
				context)
		{
			this.glPath = glPath;

			this.textureRAMPath = textureRAMPath;

			this.textureType = textureType;

			this.textureRAMVariantID = textureRAMVariantID;
		}

		protected override async Task<TextureOpenGL> AllocateResource(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			IReadOnlyResourceStorageHandle glStorageHandle = null;

			IReadOnlyResourceStorageHandle textureRAMStorageHandle = null;

#if PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES
			List<Task> loadDependencyTasks = new List<Task>();

			List<float> loadDependencyProgresses = new List<float>();

			IProgress<float> glLoadProgress = progress.CreateLocalProgress(
				0f,
				0.666f,
				loadDependencyProgresses,
				0);

			loadDependencyTasks.Add(
				Task.Run(async () => 
					{
						glStorageHandle = await LoadDependency(
							glPath,
							string.Empty,
							glLoadProgress)
							.ThrowExceptions<IReadOnlyResourceStorageHandle, TextureOpenGLStorageHandle>(context.Logger);
					}));

			IProgress<float> textureRAMLoadProgress = progress.CreateLocalProgress(
				0f,
				0.666f,
				loadDependencyProgresses,
				1);

			loadDependencyTasks.Add(
				Task.Run(async () =>
					{
						textureRAMStorageHandle = await LoadDependency(
							textureRAMPath,
							textureRAMVariantID,
							textureRAMLoadProgress)
							.ThrowExceptions<IReadOnlyResourceStorageHandle, TextureOpenGLStorageHandle>(context.Logger);
					}));


			await Task
				.WhenAll(loadDependencyTasks)
				.ThrowExceptions<TextureOpenGLStorageHandle>(context.Logger);
#else
			IProgress<float> localProgress = progress.CreateLocalProgress(
				0f,
				0.333f);

			glStorageHandle = await LoadDependency(
				glPath,
				string.Empty,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, TextureOpenGLStorageHandle>(context.Logger);

			progress?.Report(0.333f);

			localProgress = progress.CreateLocalProgress(
				0.333f,
				0.666f);

			textureRAMStorageHandle = await LoadDependency(
				textureRamPath,
				TextureRAMAssetImporter.TEXTURE_RAM_VARIANT_ID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, TextureOpenGLStorageHandle>(context.Logger);
#endif

			GL gl = glStorageHandle.GetResource<GL>();

			Image<Rgba32> textureRAM = textureRAMStorageHandle.GetResource<Image<Rgba32>>();

			progress?.Report(0.666f);

			TextureOpenGL textureOpenGL = null;

			Action buildTextureOpenGLDelegate = () =>
			{
				textureOpenGL = TextureFactory.BuildTextureOpenGL(
					gl,
					textureRAM,
					textureType);
			};

			await ExecuteOnMainThread(
				buildTextureOpenGLDelegate)
				.ThrowExceptions<TextureOpenGLStorageHandle>(context.Logger);

			progress?.Report(1f);

			return textureOpenGL;
		}

		protected override async Task FreeResource(
			TextureOpenGL resource,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			IProgress<float> localProgress = progress.CreateLocalProgress(
				0f,
				0.5f);

			var glStorageHandle = await LoadDependency(
				glPath,
				string.Empty,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, TextureOpenGLStorageHandle>(context.Logger);

			GL gl = glStorageHandle.GetResource<GL>();

			progress?.Report(0.5f);

			Action deleteShaderDelegate = () =>
			{
				gl.DeleteTexture(resource.Handle);
			};

			await ExecuteOnMainThread(
				deleteShaderDelegate)
				.ThrowExceptions<TextureOpenGLStorageHandle>(context.Logger);

			progress?.Report(1f);
		}
	}
}