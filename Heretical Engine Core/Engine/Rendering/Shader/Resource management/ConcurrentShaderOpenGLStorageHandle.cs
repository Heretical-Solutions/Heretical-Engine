using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ConcurrentShaderOpenGLStorageHandle : IReadOnlyResourceStorageHandle
	{
		private readonly string vertexShaderSource = string.Empty;

		private readonly string fragmentShaderSource = string.Empty;

		private readonly GL cachedGL = default;

		private readonly ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer;

		private readonly SemaphoreSlim semaphore;

		private readonly IFormatLogger logger;


		private bool allocated = false;

		private ShaderOpenGL shader = null;


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
			string vertexShaderSource,
			string fragmentShaderSource,
			GL gl,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			SemaphoreSlim semaphore,
			IFormatLogger logger)
		{
			this.vertexShaderSource = vertexShaderSource;

			this.fragmentShaderSource = fragmentShaderSource;

			cachedGL = gl;

			this.mainThreadCommandBuffer = mainThreadCommandBuffer;

			this.semaphore = semaphore;

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
			get
			{
				semaphore.Wait(); // Acquire the semaphore
				
				try
				{
					return allocated;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		public virtual async Task Allocate(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				if (allocated)
				{
					progress?.Report(1f);

					return;
				}

				logger.Log<ConcurrentShaderOpenGLStorageHandle>(
					$"ALLOCATING");

				// Delegate the shader building task to the main thread
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
						logger.LogError<ConcurrentShaderOpenGLStorageHandle>(
							$"BUILDING SHADER PROGRAM FAILED");

						return;
					}

					shader = new ShaderOpenGL(handle);
				};

				var command = new MainThreadCommand(buildShaderDelegate);

				while (!mainThreadCommandBuffer.TryProduce(command))
				{
					await Task.Yield();
				}

				while (command.Status != ECommandStatus.DONE)
				{
					await Task.Yield();
				}

				allocated = true;

				logger.Log<ConcurrentShaderOpenGLStorageHandle>(
					$"ALLOCATED");
			}
			finally
			{
				semaphore.Release(); // Release the semaphore

				progress?.Report(1f);
			}
		}

		public virtual async Task Free(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				if (!allocated)
				{
					progress?.Report(1f);

					return;
				}

				logger.Log<ConcurrentShaderOpenGLStorageHandle>(
					$"FREEING");

				//cachedGL.DeleteProgram(shader.Handle);

				Action deleteShaderDelegate = () =>
				{
					cachedGL.DeleteProgram(shader.Handle);
				};

				var command = new MainThreadCommand(
					deleteShaderDelegate);

				while (!mainThreadCommandBuffer.TryProduce(
					command))
				{
					await Task.Yield();
				}

				while (command.Status != ECommandStatus.DONE)
				{
					await Task.Yield();
				}

				shader = null;

				vertexShaderMetadata.Compiled = false;
				vertexShaderMetadata.CompilationLog = string.Empty;

				fragmentShaderMetadata.Compiled = false;
				fragmentShaderMetadata.CompilationLog = string.Empty;

				shaderProgramMetadata.Compiled = false;
				shaderProgramMetadata.CompilationLog = string.Empty;

				allocated = false;

				logger.Log<ConcurrentShaderOpenGLStorageHandle>(
					$"FREE");
			}
			finally
			{
				semaphore.Release(); // Release the semaphore

				progress?.Report(1f);
			}
		}

		#endregion

		public object RawResource
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					if (!allocated)
						throw new InvalidOperationException("Resource is not allocated.");

					return shader;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		public TValue GetResource<TValue>()
		{
			semaphore.Wait(); // Acquire the semaphore
			try
			{
				if (!allocated)
					throw new InvalidOperationException("Resource is not allocated.");

				return (TValue)(object)shader;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		#endregion
	}
}