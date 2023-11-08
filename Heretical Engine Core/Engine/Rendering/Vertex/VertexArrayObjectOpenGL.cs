using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class VertexArrayObjectOpenGL<TVertex>
		//where TVertex : unmanaged
	{
		private uint handle;

		public uint Handle => handle;

		public VertexArrayObjectOpenGL(
			uint handle)
		{
            this.handle = handle;
		}

		public unsafe void VertexAttributePointer(
			GL gl,
			uint index,
			int size,
			VertexAttribPointerType type,
			int offSet)
		{
			Console.WriteLine($"VertexAttributePointer: index: {index}, size: {size}, stride: {sizeof(TVertex)}, offset: {offSet}");

			/*
			gl.VertexAttribPointer(
				index,
				count,
				type,
				false,
				(uint)sizeof(TVertex),
				(void*)offSet);

			gl.EnableVertexAttribArray(index);
			*/

			///*
			gl.EnableVertexAttribArray(index);

			gl.VertexAttribPointer(
				index,
				size,
				type,
				false,
				(uint)sizeof(TVertex),
				(void*)offSet);
			//*/
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