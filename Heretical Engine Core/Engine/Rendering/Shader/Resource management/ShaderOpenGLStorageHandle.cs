using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ShaderOpenGLStorageHandle
		: IResourceStorageHandle
	{
		private readonly string vertexShaderSource = string.Empty;

		private readonly string fragmentShaderSource = string.Empty;

		private readonly GL cachedGL = default;


		private bool allocated = false;

		private ShaderOpenGL shader = null;


		public ShaderResourceMetadata VertexShaderMetadata { get; private set; }

		public ShaderResourceMetadata FragmentShaderMetadata { get; private set; }

		public ShaderResourceMetadata ShaderProgramMetadata { get; private set; }

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

		#region IResourceStorageHandle

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

			if (!LoadShader(
				vertexShaderSource,
				fragmentShaderSource,
				cachedGL,
				out uint handle))
			{
				progress?.Report(1f);

				return;
			}

			shader = new ShaderOpenGL(
				handle);

			allocated = true;

			progress?.Report(1f);
		}

		private bool LoadShader(
			string vertexShaderSource,
			string fragmentShaderSource,
			GL gl,
			out uint handle)
		{
			handle = 0;

			uint vertex = CompileShader(
				gl,
				vertexShaderSource,
				ShaderType.VertexShader,
				VertexShaderMetadata);

			if (!VertexShaderMetadata.Compiled)
			{
				return false;
			}

			uint fragment = CompileShader(
				gl,
				fragmentShaderSource,
				ShaderType.FragmentShader,
				FragmentShaderMetadata);

			if (!FragmentShaderMetadata.Compiled)
			{
				return false;
			}

			handle = gl.CreateProgram();

			gl.AttachShader(
				handle,
				vertex);

			gl.AttachShader(
				handle,
				fragment);

			gl.LinkProgram(handle);

			gl.GetProgram(
				handle,
				GLEnum.LinkStatus,
				out var status);

			ShaderProgramMetadata.Compiled = status != 0;

			ShaderProgramMetadata.CompilationLog = gl.GetProgramInfoLog(handle);

			if (!ShaderProgramMetadata.Compiled)
			{
				return false;
			}

			/*
			if (status == 0)
			{
				throw new Exception($"Program failed to link with error: {gl.GetProgramInfoLog(handle)}");
			}
			*/

			gl.DetachShader(
				handle,
				vertex);

			gl.DetachShader(
				handle,
				fragment);

			gl.DeleteShader(vertex);

			gl.DeleteShader(fragment);

			return true;
		}

		private uint CompileShader(
			GL gl,
			string source,
			ShaderType type,
			ShaderResourceMetadata metadata)
		{
			uint handle = gl.CreateShader(type);

			gl.ShaderSource(
				handle,
				source);

			gl.CompileShader(handle);

			string infoLog = gl.GetShaderInfoLog(handle);

			metadata.Compiled = string.IsNullOrWhiteSpace(infoLog);

			metadata.CompilationLog = infoLog;

			/*
			if (!string.IsNullOrWhiteSpace(infoLog))
			{
				throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
			}
			*/

			return handle;
		}

		public object RawResource
		{
			get => shader;
		}

		public TValue GetResource<TValue>()
		{
			return (TValue)(object)shader;
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
	}
}