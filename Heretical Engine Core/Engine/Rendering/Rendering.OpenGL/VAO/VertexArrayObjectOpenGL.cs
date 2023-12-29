using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public struct VertexArrayObjectOpenGL
	{
		public uint Handle;

		public VertexArrayObjectOpenGL(
			uint handle)
		{
            Handle = handle;
		}

		public unsafe void VertexAttributePointer(
			GL gl,
			uint location,
			int size,
			VertexAttribPointerType type,
			uint stride,
			int offSet)
		{
			//Console.WriteLine($"VertexAttributePointer: location: {location}, size: {size}, stride: {stride}, offset: {offSet}");

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
			gl.BindVertexArray(Handle);
		}

		public void Dispose(
			GL gl)
		{
			gl.DeleteVertexArray(Handle);
		}
	}
}