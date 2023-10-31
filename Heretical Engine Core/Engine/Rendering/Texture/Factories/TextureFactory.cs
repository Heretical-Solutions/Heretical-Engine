using HereticalSolutions.Collections.Managed;

using HereticalSolutions.Persistence.IO;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

using Silk.NET.Assimp;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class TextureFactory
	{
		public static unsafe TextureOpenGL BuildTextureOpenGL(
			GL gl,
			Image<Rgba32> ramTexture,
			TextureType textureType)
		{
			var handle = gl.GenTexture();

			var result = new TextureOpenGL(
				handle,
				textureType);

			result.Bind(
				gl);

			result.Update(
				gl,
				ramTexture);

			return result;
		}

		public static unsafe TextureOpenGL BuildTextureOpenGL(
			GL gl,
			Span<byte> data,
			uint width,
			uint height,
			TextureType textureType)
		{
			var handle = gl.GenTexture();

			var result = new TextureOpenGL(
				handle,
				textureType);

			result.Bind(
				gl);

			result.Update(
				gl,
				data,
				width,
				height);

			return result;
		}

		public static TextureRAMStorageHandle BuildTextureRAMStorageHandle(
			FilePathSettings filePathSettings,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
		{
			return new TextureRAMStorageHandle(
				filePathSettings,
				mainThreadCommandBuffer,
				logger);
		}

		public static TextureOpenGLStorageHandle BuildTextureOpenGLStorageHandle(
			TextureRAMStorageHandle textureRAMStorageHandle,
			TextureType textureType,
			GL gl,
			IFormatLogger logger)
		{
			return new TextureOpenGLStorageHandle(
				textureRAMStorageHandle,
				textureType,
				gl,
				logger);
		}
	}
}