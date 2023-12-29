using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public struct BufferObjectOpenGL<TValue>
		where TValue : unmanaged
	{
		public uint Handle;

		public int Length;

		public BufferTargetARB BufferType;

		public unsafe BufferObjectOpenGL(
			uint handle,
			int length,
			BufferTargetARB bufferType)
		{
			Handle = handle;

			Length = length;

			BufferType = bufferType;
		}

		public void Bind(
			GL gl)
		{
			gl.BindBuffer(
				BufferType,
				Handle);
		}

		public unsafe void Update(
			GL gl,
			Span<TValue> data)
		{
			//Console.WriteLine($"Updating buffer {handle} with {data.Length} elements of type {typeof(TValue).Name}. Element size: {sizeof(TValue)} Total size: {((nuint)(data.Length * sizeof(TValue))).ToString()}");

			fixed (void* dataPointer = data)
			{
				gl.BufferData(
					BufferType,
					(nuint)(data.Length * sizeof(TValue)),
					dataPointer,
					BufferUsageARB.StaticDraw);

				Length = data.Length;
			}
		}

		public unsafe void Update(
			GL gl,
			TValue[] data)
		{
			/*
			Console.WriteLine($"Updating buffer {Handle} with {data.Length} elements of type {typeof(TValue).Name}. Element size: {sizeof(TValue)} Total size: {((nuint)(data.Length * sizeof(TValue))).ToString()}");
			*/

			fixed (void* dataPointer = &data[0])
			{

				gl.BufferData(
					BufferType,
					(nuint)(data.Length * sizeof(TValue)),
					dataPointer,
					BufferUsageARB.StaticDraw);

				Length = data.Length;
			}
		}

		public void Dispose(
			GL gl)
		{
			gl.DeleteBuffer(Handle);
		}
	}
}