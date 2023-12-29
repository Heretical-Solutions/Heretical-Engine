using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class BufferFactory
	{
		public static BufferObjectOpenGL<TValue> BuildBufferOpenGL<TValue>(
			GL gl,
			BufferTargetARB bufferType)
			where TValue : unmanaged
		{
			var handle = gl.GenBuffer();

			var result = new BufferObjectOpenGL<TValue>(
				handle,
				0,
				bufferType);

			return result;
		}
	}
}