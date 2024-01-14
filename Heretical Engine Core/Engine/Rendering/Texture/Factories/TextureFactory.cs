using HereticalSolutions.Persistence.IO;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

using Silk.NET.Assimp;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class TextureFactory
	{
		public static TextureAssetDescriptor BuildTextureAssetDescriptor(
			string name,
			TextureType type)
		{
			string wrapS = GLEnum.ClampToEdge.ToString();

			string wrapT = GLEnum.ClampToEdge.ToString();

			string minFilter = GLEnum.LinearMipmapLinear.ToString();

			string magFilter = GLEnum.Linear.ToString();

			int baseLevel = 0;

			int maxLevel = 8;

			return new TextureAssetDescriptor
			{
				Name = name,
				Type = type.ToString(),
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
			TextureAssetDescriptor descriptor,
			IFormatLogger logger)
		{
			if (!Enum.TryParse(
				descriptor.Type,
				out TextureType textureType))
			{
				logger?.ThrowException(
					typeof(TextureFactory),
					$"COULD NOT PARSE TEXTURE TYPE: {descriptor.Type}");
			}

			var handle = gl.GenTexture();

			var result = new TextureOpenGL(
				handle,
				textureType);

			result.Bind(
				gl);

			result.Update(
				gl,
				ramTexture,
				descriptor,
				logger);

			gl.BindTexture(
				TextureTarget.Texture2D, 0);

			return result;
		}

		public static unsafe TextureOpenGL BuildTextureOpenGL(
			GL gl,
			Span<byte> data,
			uint width,
			uint height,
			TextureAssetDescriptor descriptor,
			IFormatLogger logger)
		{
			if (!Enum.TryParse(
				descriptor.Type,
				out TextureType textureType))
			{
				logger?.ThrowException(
					typeof(TextureFactory),
					$"COULD NOT PARSE TEXTURE TYPE: {descriptor.Type}");
			}

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
				height,
				descriptor,
				logger);

			gl.BindTexture(
				TextureTarget.Texture2D, 0);

			return result;
		}

		public static TextureRAMStorageHandle BuildTextureRAMStorageHandle(
			FilePathSettings filePathSettings,
			IGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IRuntimeResourceManager runtimeResourceManager,
			IFormatLogger logger = null)
		{
			return new TextureRAMStorageHandle(
				filePathSettings,
				mainThreadCommandBuffer,
				runtimeResourceManager,
				logger);
		}

		public static ConcurrentTextureRAMStorageHandle BuildConcurrentTextureRAMStorageHandle(
			FilePathSettings filePathSettings,
			IGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IRuntimeResourceManager runtimeResourceManager,
			IFormatLogger logger = null)
		{
			return new ConcurrentTextureRAMStorageHandle(
				filePathSettings,
				new SemaphoreSlim(1, 1),
				mainThreadCommandBuffer,
				runtimeResourceManager,
				logger);
		}

		/*
		public static TextureOpenGLStorageHandle BuildTextureOpenGLStorageHandle(
			string textureRAMResourcePath,
			string textureRAMResourceVariantID,
			string textureDescriptorResourcePath,
			string textureDescriptorResourceVariantID,
			ApplicationContext context)
		{
			return new TextureOpenGLStorageHandle(
				OpenGLModule.GL_RESOURCE_PATH,
				textureRAMResourcePath,
				textureRAMResourceVariantID,
				textureDescriptorResourcePath,
				textureDescriptorResourceVariantID,
				context);
		}

		public static ConcurrentTextureOpenGLStorageHandle BuildConcurrentTextureOpenGLStorageHandle(
			string textureRAMResourcePath,
			string textureRAMResourceVariantID,
			string textureDescriptorResourcePath,
			string textureDescriptorResourceVariantID,
			ApplicationContext context)
		{
			return new ConcurrentTextureOpenGLStorageHandle(
				OpenGLModule.GL_RESOURCE_PATH,
				textureRAMResourcePath,
				textureRAMResourceVariantID,
				textureDescriptorResourcePath,
				textureDescriptorResourceVariantID,
				new SemaphoreSlim(1, 1),
				context);
		}
		*/
	}
}