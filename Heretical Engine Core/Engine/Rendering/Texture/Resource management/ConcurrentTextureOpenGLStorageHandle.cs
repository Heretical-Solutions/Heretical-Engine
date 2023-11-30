#define PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Application;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ConcurrentTextureOpenGLStorageHandle
		: AConcurrentReadOnlyResourceStorageHandle<TextureOpenGL>
	{
		private readonly string glPath;


		private readonly string textureRAMPath;

		private readonly string textureRAMVariantID;


		private readonly string textureDescriptorPath;

		private readonly string textureDescriptorVariantID;

		public ConcurrentTextureOpenGLStorageHandle(
			string glPath,
			string textureRAMPath,
			string textureRAMVariantID,
			string textureDescriptorPath,
			string textureDescriptorVariantID,
			SemaphoreSlim semaphore,
			ApplicationContext context)
			: base(
				semaphore,
				context)
		{
			this.glPath = glPath;


			this.textureRAMPath = textureRAMPath;

			this.textureRAMVariantID = textureRAMVariantID;


			this.textureDescriptorPath = textureDescriptorPath;

			this.textureDescriptorVariantID = textureDescriptorVariantID;
		}

		protected override async Task<TextureOpenGL> AllocateResource(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			IReadOnlyResourceStorageHandle glStorageHandle = null;

			IReadOnlyResourceStorageHandle textureRAMStorageHandle = null;

			IReadOnlyResourceStorageHandle textureDescriptorStorageHandle = null;

#if PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES
			List<Task> loadDependencyTasks = new List<Task>();

			List<float> loadDependencyProgresses = new List<float>();

			#region Load GL

			IProgress<float> glLoadProgress = progress.CreateLocalProgress(
				0f,
				0.75f,
				loadDependencyProgresses,
				0);

			Func<Task> loadGL = async () =>
			{
				glStorageHandle = await LoadDependency(
					glPath,
					string.Empty,
					glLoadProgress)
					.ThrowExceptions<IReadOnlyResourceStorageHandle, ConcurrentTextureOpenGLStorageHandle>(context.Logger);
			};

			loadDependencyTasks.Add(loadGL());

			#endregion

			#region Load RAM texture

			IProgress<float> textureRAMLoadProgress = progress.CreateLocalProgress(
				0f,
				0.75f,
				loadDependencyProgresses,
				1);

			Func<Task> loadTextureRAM = async () =>
			{
				textureRAMStorageHandle = await LoadDependency(
					textureRAMPath,
					textureRAMVariantID,
					textureRAMLoadProgress)
					.ThrowExceptions<IReadOnlyResourceStorageHandle, ConcurrentTextureOpenGLStorageHandle>(context.Logger);
			};

			loadDependencyTasks.Add(loadTextureRAM());

			#endregion

			#region Load texture descriptor

			IProgress<float> textureDescriptorLoadProgress = progress.CreateLocalProgress(
				0f,
				0.75f,
				loadDependencyProgresses,
				2);

			Func<Task> loadTextureDescriptor = async () =>
			{
				textureDescriptorStorageHandle = await LoadDependency(
					textureDescriptorPath,
					textureDescriptorVariantID,
					textureDescriptorLoadProgress)
					.ThrowExceptions<IReadOnlyResourceStorageHandle, ConcurrentTextureOpenGLStorageHandle>(context.Logger);
			};

			loadDependencyTasks.Add(loadTextureDescriptor());

			#endregion

			await Task
				.WhenAll(loadDependencyTasks)
				.ThrowExceptions<ConcurrentTextureOpenGLStorageHandle>(context.Logger);
#else
			#region Load GL

			IProgress<float> localProgress = progress.CreateLocalProgress(
				0f,
				0.25f);

			glStorageHandle = await LoadDependency(
				glPath,
				string.Empty,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, ConcurrentTextureOpenGLStorageHandle>(context.Logger);

			progress?.Report(0.25f);

			#endregion

			#region Load RAM texture

			localProgress = progress.CreateLocalProgress(
				0.25f,
				0.5f);

			textureRAMStorageHandle = await LoadDependency(
				textureRAMPath,
				TextureRAMAssetImporter.TEXTURE_RAM_VARIANT_ID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, ConcurrentTextureOpenGLStorageHandle>(context.Logger);

			progress?.Report(0.5f);

			#endregion

			#region Load texture descriptor

			localProgress = progress.CreateLocalProgress(
				0.5f,
				0.75f);

			textureDescriptorStorageHandle = await LoadDependency(
				textureDescriptorPath,
				TextureDescriptorAssetImporter.TEXTURE_DESCRIPTOR_VARIANT_ID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, ConcurrentTextureOpenGLStorageHandle>(context.Logger);

			progress?.Report(0.75f);

			#endregion
#endif

			GL gl = glStorageHandle.GetResource<GL>();

			Image<Rgba32> textureRAM = textureRAMStorageHandle.GetResource<Image<Rgba32>>();

			TextureDescriptorDTO descriptor = textureDescriptorStorageHandle.GetResource<TextureDescriptorDTO>();

			progress?.Report(0.75f);

			TextureOpenGL textureOpenGL = null;

			Action buildTextureOpenGLDelegate = () =>
			{
				textureOpenGL = TextureFactory.BuildTextureOpenGL(
					gl,
					textureRAM,
					descriptor);
			};

			await ExecuteOnMainThread(
				buildTextureOpenGLDelegate)
				.ThrowExceptions<ConcurrentTextureOpenGLStorageHandle>(context.Logger);

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
				.ThrowExceptions<IReadOnlyResourceStorageHandle, ConcurrentTextureOpenGLStorageHandle>(context.Logger);

			GL gl = glStorageHandle.GetResource<GL>();

			progress?.Report(0.5f);

			Action deleteShaderDelegate = () =>
			{
				gl.DeleteTexture(resource.Handle);
			};

			await ExecuteOnMainThread(
				deleteShaderDelegate)
				.ThrowExceptions<ConcurrentTextureOpenGLStorageHandle>(context.Logger);

			progress?.Report(1f);
		}
	}
}
