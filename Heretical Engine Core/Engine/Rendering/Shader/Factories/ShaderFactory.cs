using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class ShaderFactory
	{
		public static bool BuildShaderProgram(
			string vertexShaderSource,
			string fragmentShaderSource,
			GL gl,
			out uint handle,
			out ShaderResourceMetadata VertexShaderMetadata,
			out ShaderResourceMetadata FragmentShaderMetadata,
			out ShaderResourceMetadata ShaderProgramMetadata)
		{
			handle = 0;

			uint vertex = CompileShader(
				gl,
				vertexShaderSource,
				ShaderType.VertexShader,
				out VertexShaderMetadata);

			if (!VertexShaderMetadata.Compiled)
			{
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

				return false;
			}

			uint fragment = CompileShader(
				gl,
				fragmentShaderSource,
				ShaderType.FragmentShader,
				out FragmentShaderMetadata);

			if (!FragmentShaderMetadata.Compiled)
			{
				ShaderProgramMetadata = new ShaderResourceMetadata
				{
					Compiled = false,

					CompilationLog = string.Empty
				};

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

			ShaderProgramMetadata = default;

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

		private static uint CompileShader(
			GL gl,
			string source,
			ShaderType type,
			out ShaderResourceMetadata metadata)
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

		public static ShaderOpenGLStorageHandle BuildShaderOpenGLStorageHandle(
			string vertexShaderSource,
			string fragmentShaderSource,
			GL gl,
			IFormatLogger logger)
		{
			return new ShaderOpenGLStorageHandle(
				vertexShaderSource,
				fragmentShaderSource,
				gl,
				logger);
		}
	}
}