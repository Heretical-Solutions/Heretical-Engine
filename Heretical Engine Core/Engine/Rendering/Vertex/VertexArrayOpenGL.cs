using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class VertexArrayOpenGL<TVertex>
		//where TVertex : unmanaged
	{
		private uint handle;

		public uint Handle => handle;

		public VertexArrayOpenGL(
			uint handle)
		{
            this.handle = handle;
		}

		public unsafe void VertexAttributePointer(
			GL gl,
			uint index,
			int count,
			VertexAttribPointerType type,
			//uint vertexSize,
			int offSet)
		{
			Console.WriteLine($"VertexAttributePointer: index: {index}, count: {count}, stride: {sizeof(TVertex)}, offset: {offSet}");

			gl.VertexAttribPointer(
				index,
				count,
				type,
				false,
				(uint)sizeof(TVertex),
				(void*)offSet);

			gl.EnableVertexAttribArray(index);

			/*
			gl.EnableVertexAttribArray(index);

			Console.WriteLine($"VertexAttributePointer: index: {index}, count: {count}, stride: {sizeof(TVertex)}, offset: {offSet}");

			gl.VertexAttribPointer(
				index,
				count,
				type,
				false,
				(uint)sizeof(TVertex),
				(void*)offSet);
			*/

			/*
			gl.VertexAttribPointer(
				index,
				count,
				type,
				false,
				vertexSize * (uint)sizeof(TVertex),
				(void*)(offSet * sizeof(TVertex)));

			gl.EnableVertexAttribArray(index);
			*/
		}

		public void Bind(
			GL gl)
		{
			gl.BindVertexArray(handle);
		}

		public void Dispose(
			GL gl)
		{
			gl.DeleteVertexArray(handle);
		}
	}
}