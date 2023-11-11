using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class VertexArrayObjectOpenGL
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
			uint location,
			int size,
			VertexAttribPointerType type,
			uint stride,
			int offSet)
		{
			Console.WriteLine($"VertexAttributePointer: location: {location}, size: {size}, stride: {stride}, offset: {offSet}");

			gl.EnableVertexAttribArray(location);

			gl.VertexAttribPointer(
				location,
				size,
				type,
				false,
				stride,
				(void*)offSet);
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