using System;
using System.Threading.Tasks;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ShaderOpenGLStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		private readonly string vertexShaderSource = string.Empty;

		private readonly string fragmentShaderSource = string.Empty;

		private readonly GL cachedGL = default;

		private readonly ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer;

		private readonly IFormatLogger logger;


		private bool allocated = false;

		private ShaderOpenGL shader = null;


		private ShaderResourceMetadata vertexShaderMetadata;

		public ShaderResourceMetadata VertexShaderMetadata { get => vertexShaderMetadata; }


		private ShaderResourceMetadata fragmentShaderMetadata;

		public ShaderResourceMetadata FragmentShaderMetadata { get => fragmentShaderMetadata; }


		private ShaderResourceMetadata shaderProgramMetadata;

		public ShaderResourceMetadata ShaderProgramMetadata { get => shaderProgramMetadata; }

		public ShaderOpenGLStorageHandle(
			string vertexShaderSource,
			string fragmentShaderSource,
			GL gl,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
		{
			this.vertexShaderSource = vertexShaderSource;

			this.fragmentShaderSource = fragmentShaderSource;

			cachedGL = gl;

			this.mainThreadCommandBuffer = mainThreadCommandBuffer;

			this.logger = logger;


			shader = null;

			allocated = false;


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
					cachedGL,
					out uint handle,
					out vertexShaderMetadata,
					out fragmentShaderMetadata,
					out shaderProgramMetadata))
				{
					//progress?.Report(1f);

					logger.LogError<ShaderOpenGLStorageHandle>(
						$"BUILDING SHADER PROGRAM FAILED");

					return;
				}

				shader = new ShaderOpenGL(
					handle);
			};

			var command = new MainThreadCommand(
				buildShaderDelegate);

			while (!mainThreadCommandBuffer.TryProduce(
				command))
			{
				await Task.Yield();
			}

			while (command.Status != ECommandStatus.DONE)
			{
				await Task.Yield();
			}

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

			cachedGL.DeleteProgram(shader.Handle);

			shader = null;


			vertexShaderMetadata.Compiled = false;

			vertexShaderMetadata.CompilationLog = string.Empty;


			fragmentShaderMetadata.Compiled = false;

			fragmentShaderMetadata.CompilationLog = string.Empty;


			shaderProgramMetadata.Compiled = false;

			shaderProgramMetadata.CompilationLog = string.Empty;


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

				return shader;
			}
		}

		public TValue GetResource<TValue>()
		{
			if (!allocated)
				throw new InvalidOperationException("Resource is not allocated.");

			return (TValue)(object)shader; //DO NOT REPEAT
		}

		#endregion
	}
}