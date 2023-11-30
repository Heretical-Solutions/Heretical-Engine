using HereticalSolutions.Persistence.IO;

using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.HereticalEngine.Modules;

using Silk.NET.OpenGL;

using Silk.NET.Assimp;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class TextureFactory
	{
		public static TextureDescriptorDTO BuildTextureDescriptorDTO(
			string name,
			TextureType type)
		{
			int wrapS = (int)GLEnum.ClampToEdge;

			int wrapT = (int)GLEnum.ClampToEdge;

			int minFilter = (int)GLEnum.LinearMipmapLinear;

			int magFilter = (int)GLEnum.Linear;

			int baseLevel = 0;

			int maxLevel = 8;

			return new TextureDescriptorDTO
			{
				Name = name,
				Type = type,
				WrapS = wrapS,
				WrapT = wrapT,
				MinFilter = minFilter,
				MagFilter = magFilter,
				BaseLevel = baseLevel,
				MaxLevel = maxLevel
			};
		}

		public static unsafe TextureOpenGL BuildTextureOpenGL(
			GL gl,
			Image<Rgba32> ramTexture,
			TextureDescriptorDTO descriptor)
		{
			var handle = gl.GenTexture();

			var result = new TextureOpenGL(
				handle,
				descriptor.Type);

			result.Bind(
				gl);

			result.Update(
				gl,
				ramTexture,
				descriptor);

			gl.BindTexture(
				TextureTarget.Texture2D, 0);

			return result;
		}

		public static unsafe TextureOpenGL BuildTextureOpenGL(
			GL gl,
			Span<byte> data,
			uint width,
			uint height,
			TextureDescriptorDTO descriptor)
		{
			var handle = gl.GenTexture();

			var result = new TextureOpenGL(
				handle,
				descriptor.Type);

			result.Bind(
				gl);

			result.Update(
				gl,
				data,
				width,
				height,
				descriptor);

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
			string textureDescriptorPath,
			string textureDescriptorVariantID,
			ApplicationContext context)
		{
			return new TextureOpenGLStorageHandle(
				OpenGLModule.GL_RESOURCE_PATH,
				textureRAMPath,
				textureRAMVariantID,
				textureDescriptorPath,
				textureDescriptorVariantID,
				context);
		}

		public static ConcurrentTextureOpenGLStorageHandle BuildConcurrentTextureOpenGLStorageHandle(
			string textureRAMPath,
			string textureRAMVariantID,
			string textureDescriptorPath,
			string textureDescriptorVariantID,
			ApplicationContext context)
		{
			return new ConcurrentTextureOpenGLStorageHandle(
				OpenGLModule.GL_RESOURCE_PATH,
				textureRAMPath,
				textureRAMVariantID,
				textureDescriptorPath,
				textureDescriptorVariantID,
				new SemaphoreSlim(1, 1),
				context);
		}
	}
}