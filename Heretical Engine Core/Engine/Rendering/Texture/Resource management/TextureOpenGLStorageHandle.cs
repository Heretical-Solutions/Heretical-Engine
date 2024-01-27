/*
using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Application;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureOpenGLStorageHandle
		: AReadOnlyResourceStorageHandle<TextureOpenGL>
	{
		private readonly string glResourcePath;


		private readonly string textureRAMResourcePath;

		private readonly string textureRAMResourceVariantID;


		private readonly string textureDescriptorResourcePath;

		private readonly string textureDescriptorResourceVariantID;


		public TextureOpenGLStorageHandle(
			string glResourcePath,

			string textureRAMResourcePath,
			string textureRAMResourceVariantID,
			
			string textureDescriptorResourcePath,
			string textureDescriptorResourceVariantID,
			
			ApplicationContext context)
			: base (
				context)
		{
			this.glResourcePath = glResourcePath;


			this.textureRAMResourcePath = textureRAMResourcePath;

			this.textureRAMResourceVariantID = textureRAMResourceVariantID;


			this.textureDescriptorResourcePath = textureDescriptorResourcePath;

			this.textureDescriptorResourceVariantID = textureDescriptorResourceVariantID;
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
					glResourcePath,
					string.Empty,
					glLoadProgress)
					.ThrowExceptions<IReadOnlyResourceStorageHandle, TextureOpenGLStorageHandle>(context.Logger);
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
					textureRAMResourcePath,
					textureRAMResourceVariantID,
					textureRAMLoadProgress)
					.ThrowExceptions<IReadOnlyResourceStorageHandle, TextureOpenGLStorageHandle>(context.Logger);
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
					textureDescriptorResourcePath,
					textureDescriptorResourceVariantID,
					textureDescriptorLoadProgress)
					.ThrowExceptions<IReadOnlyResourceStorageHandle, TextureOpenGLStorageHandle>(context.Logger);
			};

			loadDependencyTasks.Add(loadTextureDescriptor());

			#endregion

			await Task
				.WhenAll(loadDependencyTasks)
				.ThrowExceptions<TextureOpenGLStorageHandle>(context.Logger);
#else
			#region Load GL

			IProgress<float> localProgress = progress.CreateLocalProgress(
				0f,
				0.25f);

			glStorageHandle = await LoadDependency(
				glPath,
				string.Empty,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, TextureOpenGLStorageHandle>(context.Logger);

			progress?.Report(0.25f);

			#endregion

			#region Load RAM texture

			localProgress = progress.CreateLocalProgress(
				0.25f,
				0.5f);

			textureRAMStorageHandle = await LoadDependency(
				textureRAMPath,
				textureRAMVariantID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, TextureOpenGLStorageHandle>(context.Logger);

			progress?.Report(0.5f);

			#endregion

			#region Load texture descriptor

			localProgress = progress.CreateLocalProgress(
				0.5f,
				0.75f);

			textureDescriptorStorageHandle = await LoadDependency(
				textureDescriptorPath,
				textureDescriptorVariantID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, TextureOpenGLStorageHandle>(context.Logger);

			progress?.Report(0.75f);

			#endregion
#endif

			GL gl = glStorageHandle.GetResource<GL>();

			Image<Rgba32> textureRAM = textureRAMStorageHandle.GetResource<Image<Rgba32>>();

			TextureAssetDescriptor descriptor = textureDescriptorStorageHandle.GetResource<TextureAssetDescriptor>();

			progress?.Report(0.75f);

			TextureOpenGL textureOpenGL = null;

			Action buildTextureOpenGLDelegate = () =>
			{
				textureOpenGL = TextureFactory.BuildTextureOpenGL(
					gl,
					textureRAM,
					descriptor,
					context.Logger);
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
				glResourcePath,
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
*/