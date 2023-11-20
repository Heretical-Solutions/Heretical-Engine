using HereticalSolutions.Persistence.IO;

using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.HereticalEngine.Modules;

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
			ApplicationContext context)
		{
			return new TextureRAMStorageHandle(
				filePathSettings,
				context);
		}

		public static ConcurrentTextureRAMStorageHandle BuildConcurrentTextureRAMStorageHandle(
			FilePathSettings filePathSettings,
			ApplicationContext context)
		{
			return new ConcurrentTextureRAMStorageHandle(
				filePathSettings,
				new SemaphoreSlim(1, 1),
				context);
		}

		public static TextureOpenGLStorageHandle BuildTextureOpenGLStorageHandle(
			string textureRAMPath,
			string textureRAMVariantID,
			TextureType textureType,
			ApplicationContext context)
		{
			return new TextureOpenGLStorageHandle(
				OpenGLModule.GL_RESOURCE_PATH,
				textureRAMPath,
				textureRAMVariantID,
				textureType,
				context);
		}

		public static ConcurrentTextureOpenGLStorageHandle BuildConcurrentTextureOpenGLStorageHandle(
			string textureRAMPath,
			string textureRAMVariantID,
			TextureType textureType,
			ApplicationContext context)
		{
			return new ConcurrentTextureOpenGLStorageHandle(
				OpenGLModule.GL_RESOURCE_PATH,
				textureRAMPath,
				textureRAMVariantID,
				textureType,
				new SemaphoreSlim(1, 1),
				context);
		}
	}
}