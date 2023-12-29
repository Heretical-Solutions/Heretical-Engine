using Silk.NET.Assimp;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public struct ShaderSampler2DArgumentOpenGL
	{
		public string Name;

		public TextureUnit TextureSlot;

		public TextureType Type;

		public bool KeywordTexture;
	}
}