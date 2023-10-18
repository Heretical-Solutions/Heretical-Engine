using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class GLBuffer<TValue> : IDisposable
		where TValue : unmanaged
	{
		private uint handle;

		private BufferTargetARB bufferType;

		private GL gl;

		public unsafe GLBuffer(
			GL gl,
			Span<TValue> data,
			BufferTargetARB bufferType)
		{
			this.gl = gl;

			this.bufferType = bufferType;

            handle = this.gl.GenBuffer();

			Bind();

			fixed (void* d = data)
			{
				gl.BufferData(
					bufferType,
					(nuint)(data.Length * sizeof(TValue)),
					d,
					BufferUsageARB.StaticDraw);
			}
		}

		public void Bind()
		{
			gl.BindBuffer(bufferType, handle);
		}

		public void Dispose()
		{
			gl.DeleteBuffer(handle);
		}
	}
}