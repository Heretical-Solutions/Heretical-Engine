/*
using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Application;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ConcurrentShaderOpenGLStorageHandle
		: AConcurrentReadOnlyResourceStorageHandle<ShaderOpenGL>
	{
		private readonly string glPath;

		private readonly string vertexShaderSource = string.Empty;

		private readonly string fragmentShaderSource = string.Empty;


		private ShaderResourceMetadata vertexShaderMetadata;

		public ShaderResourceMetadata VertexShaderMetadata
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					return vertexShaderMetadata;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		private ShaderResourceMetadata fragmentShaderMetadata;

		public ShaderResourceMetadata FragmentShaderMetadata
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					return fragmentShaderMetadata;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		private ShaderResourceMetadata shaderProgramMetadata;

		public ShaderResourceMetadata ShaderProgramMetadata
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					return shaderProgramMetadata;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		public ConcurrentShaderOpenGLStorageHandle(
			string glPath,
			string vertexShaderSource,
			string fragmentShaderSource,
			SemaphoreSlim semaphore,
			ApplicationContext context)
			: base(
				semaphore,
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
				.ThrowExceptions<IReadOnlyResourceStorageHandle, ConcurrentShaderOpenGLStorageHandle>(context.Logger);

			GL gl = glStorageHandle.GetResource<GL>();

			progress?.Report(0.5f);

			ShaderOpenGL shaderOpenGL = null;

			// Delegate the shader building task to the main thread
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
					context.Logger?.LogError<ConcurrentShaderOpenGLStorageHandle>(
						$"BUILDING SHADER PROGRAM FAILED. LOGS:\nVERTEX: {vertexShaderMetadata.CompilationLog}\nFRAGMENT: {fragmentShaderMetadata.CompilationLog}\nSHADER PROGRAM:{shaderProgramMetadata.CompilationLog}");

					return;
				}

				shaderOpenGL = new ShaderOpenGL(
					handle,
					descriptor);
			};

			await ExecuteOnMainThread(
				buildShaderDelegate)
				.ThrowExceptions<ConcurrentShaderOpenGLStorageHandle>(context.Logger);

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
				.ThrowExceptions<IReadOnlyResourceStorageHandle, ConcurrentShaderOpenGLStorageHandle>(context.Logger);

			GL gl = glStorageHandle.GetResource<GL>();

			progress?.Report(0.5f);

			//gl.DeleteProgram(resource.Handle);

			Action deleteShaderDelegate = () =>
			{
				gl.DeleteProgram(resource.Handle);
			};

			await ExecuteOnMainThread(
				deleteShaderDelegate)
				.ThrowExceptions<ConcurrentShaderOpenGLStorageHandle>(context.Logger);

			vertexShaderMetadata.Compiled = false;
			vertexShaderMetadata.CompilationLog = string.Empty;

			fragmentShaderMetadata.Compiled = false;
			fragmentShaderMetadata.CompilationLog = string.Empty;

			shaderProgramMetadata.Compiled = false;
			shaderProgramMetadata.CompilationLog = string.Empty;

			progress?.Report(1f);
		}
	}
}
*/