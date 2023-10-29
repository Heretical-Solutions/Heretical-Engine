using HereticalSolutions.Persistence.IO;

using Silk.NET.OpenGL;

using Silk.NET.Assimp;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class TextureFactory
	{
		public static TextureRAMStorageHandle BuildTextureRAMStorageHandle(
			FileSystemSettings fsSettings)
		{
			return new TextureRAMStorageHandle(
				fsSettings);
		}

		public static TextureOpenGLStorageHandle BuildTextureOpenGLStorageHandle(
			TextureRAMStorageHandle textureRAMStorageHandle,
			TextureType textureType,
			GL gl)
		{
			return new TextureOpenGLStorageHandle(
				textureRAMStorageHandle,
				textureType,
				gl);
		}
	}
}