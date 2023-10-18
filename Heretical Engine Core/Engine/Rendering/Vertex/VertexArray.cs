using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class VertexArrayObject<TVertex, TIndex> : IDisposable
		where TVertex : unmanaged
		where TIndex : unmanaged
	{
		private uint handle;

		private GL gl;

		public VertexArrayObject(
			GL gl,
			GLBuffer<TVertex> vbo,
			GLBuffer<TIndex> ebo)
		{
			this.gl = gl;

            handle = this.gl.GenVertexArray();

			Bind();

			vbo.Bind();

			ebo.Bind();
		}

		public unsafe void VertexAttributePointer(
			uint index,
			int count,
			VertexAttribPointerType type,
			uint vertexSize,
			int offSet)
		{
			gl.VertexAttribPointer(
				index,
				count,
				type,
				false,
				vertexSize * (uint)sizeof(TVertex),
				(void*)(offSet * sizeof(TVertex)));

			gl.EnableVertexAttribArray(index);
		}

		public void Bind()
		{
			gl.BindVertexArray(handle);
		}

		public void Dispose()
		{
			gl.DeleteVertexArray(handle);
		}
	}
}