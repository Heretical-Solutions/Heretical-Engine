using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class BufferOpenGL<TValue>
		where TValue : unmanaged
	{
		private uint handle;

		public uint Handle => handle;

		private BufferTargetARB bufferType;

		public BufferTargetARB BufferType => bufferType;

		public unsafe BufferOpenGL(
			uint handle,
			BufferTargetARB bufferType)
		{
			this.handle = handle;

			this.bufferType = bufferType;
		}

		public void Bind(
			GL gl)
		{
			gl.BindBuffer(
				bufferType,
				handle);
		}

		public unsafe void Update(
			GL gl,
			Span<TValue> data)
		{
			fixed (void* dataPointer = data)
			{
				gl.BufferSubData(
					bufferType,
					0,
					(nuint)(data.Length * sizeof(TValue)),
					dataPointer);
			}
		}

		public void Dispose(
			GL gl)
		{
			gl.DeleteBuffer(handle);
		}
	}
}