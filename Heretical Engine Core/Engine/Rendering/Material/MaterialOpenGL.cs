namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MaterialOpenGL
	{
		public ShaderOpenGL Shader { get; private set; }

		public TextureOpenGL[] Textures { get; private set; }

		public MaterialOpenGL(
			ShaderOpenGL shader,
			TextureOpenGL[] textures)
		{
			Shader = shader;

			Textures = textures;
		}
	}
}