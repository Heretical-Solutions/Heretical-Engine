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

		private readonly ReaderWriterLockSlim readWriteLock;

		private readonly IFormatLogger logger;


		private bool allocated = false;

		private ShaderOpenGL shader = null;


		private ShaderResourceMetadata vertexShaderMetadata;

		public ShaderResourceMetadata VertexShaderMetadata
		{
			get
			{
				readWriteLock.EnterReadLock();

				try
				{
					return vertexShaderMetadata;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
		}

		private ShaderResourceMetadata fragmentShaderMetadata;

		public ShaderResourceMetadata FragmentShaderMetadata
		{
			get
			{
				readWriteLock.EnterReadLock();

				try
				{
					return fragmentShaderMetadata;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
		}

		private ShaderResourceMetadata shaderProgramMetadata;

		public ShaderResourceMetadata ShaderProgramMetadata
		{
			get
			{
				readWriteLock.EnterReadLock();

				try
				{
					return shaderProgramMetadata;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
		}

		public ConcurrentShaderOpenGLStorageHandle(
			string vertexShaderSource,
			string fragmentShaderSource,
			GL gl,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			ReaderWriterLockSlim readWriteLock,
			IFormatLogger logger)
		{
			this.vertexShaderSource = vertexShaderSource;

			this.fragmentShaderSource = fragmentShaderSource;

			cachedGL = gl;

			this.mainThreadCommandBuffer = mainThreadCommandBuffer;

			this.readWriteLock = readWriteLock;

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
				readWriteLock.EnterReadLock();
				
				try
				{
					return allocated;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
		}

		public virtual async Task Allocate(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			readWriteLock.EnterWriteLock();

			try
			{
				if (allocated)
				{
					progress?.Report(1f);

					return;
				}

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
			}
			finally
			{
				readWriteLock.ExitWriteLock();

				progress?.Report(1f);
			}
		}

		public virtual async Task Free(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			readWriteLock.EnterWriteLock();

			try
			{
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
			}
			finally
			{
				readWriteLock.ExitWriteLock();

				progress?.Report(1f);
			}
		}

		#endregion

		public object RawResource
		{
			get
			{
				readWriteLock.EnterReadLock();

				try
				{
					if (!allocated)
						throw new InvalidOperationException("Resource is not allocated.");

					return shader;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
		}

		public TValue GetResource<TValue>()
		{
			readWriteLock.EnterReadLock();
			try
			{
				if (!allocated)
					throw new InvalidOperationException("Resource is not allocated.");

				return (TValue)(object)shader;
			}
			finally
			{
				readWriteLock.ExitReadLock();
			}
		}

		#endregion
	}
}