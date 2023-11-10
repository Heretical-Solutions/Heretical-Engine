using HereticalSolutions.Collections.Managed;

using HereticalSolutions.Persistence.IO;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

using Silk.NET.Assimp;
using HereticalSolutions.ResourceManagement;

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

			gl.BindTexture(
				TextureTarget.Texture2D, 0);

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

			gl.BindTexture(
				TextureTarget.Texture2D, 0);

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

		public static ConcurrentTextureRAMStorageHandle BuildConcurrentTextureRAMStorageHandle(
			FilePathSettings filePathSettings,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
		{
			return new ConcurrentTextureRAMStorageHandle(
				filePathSettings,
				mainThreadCommandBuffer,
				new SemaphoreSlim(1, 1),
				logger);
		}

		public static TextureOpenGLStorageHandle BuildTextureOpenGLStorageHandle(
			IReadOnlyResourceStorageHandle textureRAMStorageHandle,
			TextureType textureType,
			GL gl,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
		{
			return new TextureOpenGLStorageHandle(
				textureRAMStorageHandle,
				textureType,
				gl,
				mainThreadCommandBuffer,
				logger);
		}

		public static ConcurrentTextureOpenGLStorageHandle BuildConcurrentTextureOpenGLStorageHandle(
			IReadOnlyResourceStorageHandle textureRAMStorageHandle,
			TextureType textureType,
			GL gl,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
		{
			return new ConcurrentTextureOpenGLStorageHandle(
				textureRAMStorageHandle,
				textureType,
				gl,
				mainThreadCommandBuffer,
				new SemaphoreSlim(1, 1),
				logger);
		}
	}
}