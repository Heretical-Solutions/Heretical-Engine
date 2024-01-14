/*
using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Application;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ShaderOpenGLStorageHandle
		: AReadOnlyResourceStorageHandle<ShaderOpenGL>
	{
		private readonly string glPath;

		private readonly string vertexShaderSource = string.Empty;

		private readonly string fragmentShaderSource = string.Empty;


		private ShaderResourceMetadata vertexShaderMetadata;

		public ShaderResourceMetadata VertexShaderMetadata { get => vertexShaderMetadata; }


		private ShaderResourceMetadata fragmentShaderMetadata;

		public ShaderResourceMetadata FragmentShaderMetadata { get => fragmentShaderMetadata; }


		private ShaderResourceMetadata shaderProgramMetadata;

		public ShaderResourceMetadata ShaderProgramMetadata { get => shaderProgramMetadata; }

		public ShaderOpenGLStorageHandle(
			string glPath,
			string vertexShaderSource,
			string fragmentShaderSource,
			ApplicationContext context)
			: base (
				context)
		{
			this.glPath = glPath;

			this.vertexShaderSource = vertexShaderSource;

			this.fragmentShaderSource = fragmentShaderSource;


			vertexShaderMetadata = new ShaderResourceMetadata
			{
				Compiled = false,

				CompilationLog = string.Empty
			};

			fragmentShaderMetadata = new ShaderResourceMetadata
			{
				Compiled = false,

				CompilationLog = string.Empty
			};

			shaderProgramMetadata = new ShaderResourceMetadata
			{
				Compiled = false,

				CompilationLog = string.Empty
			};
		}

		protected override async Task<ShaderOpenGL> AllocateResource(
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
				.ThrowExceptions<IReadOnlyResourceStorageHandle, ShaderOpenGLStorageHandle>(context.Logger);

			GL gl = glStorageHandle.GetResource<GL>();

			progress?.Report(0.5f);

			ShaderOpenGL shaderOpenGL = null;

			//According to this: https://old.reddit.com/r/opengl/comments/14zul38/how_to_transfer_gl_context_to_different_thread_in/js1pl1v/
			//OpenGL is not thread safe and should be executed on the main thread only
			//That's why I particularly had a 'Silk.NET.Core.Loader.SymbolLoadingException: Native symbol not found (Symbol: glCreateShader)' exception while running the following code (included into the delegate)
			//The async would start the method in a different thread and the OpenGL would not be able to find the symbols
			//Running the entire thing in the Program.cs itself where the window is created worked smoothly, all the metadata
			//showed Compiled == true and all the handles were greater than 0
			//While I hope I won't have such problems with Vulkan storage handle I decided to simply delegate the task of
			//building the shader to the main thread with the help of a simple thread safe circular buffer
			Action buildShaderDelegate = () =>
			{
				if (!ShaderFactory.BuildShaderProgram(
					vertexShaderSource,
					fragmentShaderSource,
					gl,
					context.Logger,
					out uint handle,
					out var descriptor,
					out vertexShaderMetadata,
					out fragmentShaderMetadata,
					out shaderProgramMetadata))
				{
					context.Logger?.LogError<ShaderOpenGLStorageHandle>(
						$"BUILDING SHADER PROGRAM FAILED");

					return;
				}

				shaderOpenGL = new ShaderOpenGL(
					handle,
					descriptor);
			};

			await ExecuteOnMainThread(
				buildShaderDelegate)
				.ThrowExceptions<ShaderOpenGLStorageHandle>(context.Logger);

			progress?.Report(1f);

			return shaderOpenGL;
		}

		protected override async Task FreeResource(
			ShaderOpenGL resource,
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
				.ThrowExceptions<IReadOnlyResourceStorageHandle, ShaderOpenGLStorageHandle>(context.Logger);

			GL gl = glStorageHandle.GetResource<GL>();

			progress?.Report(0.5f);

			//gl.DeleteProgram(resource.Handle);

			Action deleteShaderDelegate = () =>
			{
				gl.DeleteProgram(resource.Handle);
			};

			await ExecuteOnMainThread(
				deleteShaderDelegate)
				.ThrowExceptions<ShaderOpenGLStorageHandle>(context.Logger);

			progress?.Report(1f);
		}
	}
}
*/