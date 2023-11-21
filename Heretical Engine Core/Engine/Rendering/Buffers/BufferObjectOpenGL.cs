using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class BufferObjectOpenGL<TValue>
		where TValue : unmanaged
	{
		private uint handle;

		public uint Handle => handle;

		private int length;

		public int Length => length;

		private BufferTargetARB bufferType;

		public BufferTargetARB BufferType => bufferType;

		public unsafe BufferObjectOpenGL(
			uint handle,
			BufferTargetARB bufferType)
		{
			this.handle = handle;

			this.bufferType = bufferType;

			length = 0;
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
			//Console.WriteLine($"Updating buffer {handle} with {data.Length} elements of type {typeof(TValue).Name}. Element size: {sizeof(TValue)} Total size: {((nuint)(data.Length * sizeof(TValue))).ToString()}");

			fixed (void* dataPointer = data)
			{
				gl.BufferData(
					bufferType,
					(nuint)(data.Length * sizeof(TValue)),
					dataPointer,
					BufferUsageARB.StaticDraw);

				length = data.Length;
			}
		}

		public unsafe void Update(
			GL gl,
			TValue[] data)
		{
			Console.WriteLine($"Updating buffer {handle} with {data.Length} elements of type {typeof(TValue).Name}. Element size: {sizeof(TValue)} Total size: {((nuint)(data.Length * sizeof(TValue))).ToString()}");

			fixed (void* dataPointer = &data[0])
			{

				gl.BufferData(
					bufferType,
					(nuint)(data.Length * sizeof(TValue)),
					dataPointer,
					BufferUsageARB.StaticDraw);

				length = data.Length;
			}
		}

		public void Dispose(
			GL gl)
		{
			gl.DeleteBuffer(handle);
		}
	}
}