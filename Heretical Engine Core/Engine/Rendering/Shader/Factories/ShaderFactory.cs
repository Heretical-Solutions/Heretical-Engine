using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class ShaderFactory
	{
		public static ShaderOpenGLStorageHandle BuildShaderOpenGLStorageHandle(
			string vertexShaderSource,
			string fragmentShaderSource,
			GL gl)
		{
			return new ShaderOpenGLStorageHandle(
				vertexShaderSource,
				fragmentShaderSource,
				gl);
		}
	}
}