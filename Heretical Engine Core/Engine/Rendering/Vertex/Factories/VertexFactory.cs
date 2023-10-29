using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class VertexFactory
	{
		public static VertexArrayOpenGL<TVertex, TIndex> BuildVertexArrayOpenGL<TVertex, TIndex>(
			GL gl)
			where TVertex : unmanaged
			where TIndex : unmanaged
		{
			var handle = gl.GenVertexArray();

			var result = new VertexArrayOpenGL<TVertex, TIndex>(
				handle);

			result.Bind(
				gl);

			return result;
		}
	}
}