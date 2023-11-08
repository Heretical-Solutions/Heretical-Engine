using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class BufferFactory
	{
		public static BufferOpenGL<TValue> BuildBufferOpenGL<TValue>(
			GL gl,
			//Span<TValue> data,
			BufferTargetARB bufferType)
			where TValue : unmanaged
		{
			var handle = gl.GenBuffer();

			var result = new BufferOpenGL<TValue>(
				handle,
				bufferType);

			/*
			result.Bind(
				gl);

			result.Update(
				gl,
				data);
			*/

			return result;
		}
	}
}