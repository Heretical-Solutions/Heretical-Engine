using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class VertexFactory
	{
		public static VertexArrayOpenGL<TVertex> BuildVertexArrayOpenGL<TVertex>(
			GL gl)
			//where TVertex : unmanaged
		{
			var handle = gl.GenVertexArray();

			var result = new VertexArrayOpenGL<TVertex>(
				handle);

			/*
			result.Bind(
				gl);
			*/

			return result;
		}
	}
}