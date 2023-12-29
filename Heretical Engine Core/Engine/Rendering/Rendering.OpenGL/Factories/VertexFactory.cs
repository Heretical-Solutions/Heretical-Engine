using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class VertexFactory
	{
		public static VertexArrayObjectOpenGL BuildVertexArrayOpenGL(
			GL gl)
		{
			var handle = gl.GenVertexArray();

			var result = new VertexArrayObjectOpenGL(
				handle);

			return result;
		}
	}
}