using Silk.NET.Assimp;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureOpenGL
	{
		private uint handle;

		public uint Handle { get => handle; }

		public TextureType Type { get; private set; }

		public TextureOpenGL(
			uint handle,
			TextureType type = TextureType.None)
		{
			this.handle = handle;

			Type = type;
		}
	}
}