using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.ResourceManagement;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ShaderOpenGLStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		private readonly string vertexShaderSource = string.Empty;

		private readonly string fragmentShaderSource = string.Empty;

		private readonly GL cachedGL = default;


		private bool allocated = false;

		private ShaderOpenGL shader = null;


		public ShaderResourceMetadata VertexShaderMetadata;

		public ShaderResourceMetadata FragmentShaderMetadata;

		public ShaderResourceMetadata ShaderProgramMetadata;

		public ShaderOpenGLStorageHandle(
			string vertexShaderSource,
			string fragmentShaderSource,
			GL gl)
		{
			this.vertexShaderSource = vertexShaderSource;

			this.fragmentShaderSource = fragmentShaderSource;

			cachedGL = gl;


			shader = null;

			allocated = false;


			VertexShaderMetadata = new ShaderResourceMetadata
			{
				Compiled = false,

				CompilationLog = string.Empty
			};

			FragmentShaderMetadata = new ShaderResourceMetadata
			{
				Compiled = false,

				CompilationLog = string.Empty
			};

			ShaderProgramMetadata = new ShaderResourceMetadata
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

			if (!ShaderFactory.BuildShaderProgram(
				vertexShaderSource,
				fragmentShaderSource,
				cachedGL,
				out uint handle,
				out VertexShaderMetadata,
				out FragmentShaderMetadata,
				out ShaderProgramMetadata))
			{
				progress?.Report(1f);

				return;
			}

			shader = new ShaderOpenGL(
				handle);

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


			VertexShaderMetadata.Compiled = false;

			VertexShaderMetadata.CompilationLog = string.Empty;


			FragmentShaderMetadata.Compiled = false;

			FragmentShaderMetadata.CompilationLog = string.Empty;


			ShaderProgramMetadata.Compiled = false;

			ShaderProgramMetadata.CompilationLog = string.Empty;


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