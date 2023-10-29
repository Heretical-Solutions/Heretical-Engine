namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MaterialOpenGL
	{
		public ShaderOpenGL Shader { get; private set; }

		public IReadOnlyList<TextureOpenGL> Textures { get; private set; }

		public MaterialOpenGL(
			ShaderOpenGL shader,
			IReadOnlyList<TextureOpenGL> textures)
		{
			Shader = shader;

			Textures = textures;
		}
	}
}