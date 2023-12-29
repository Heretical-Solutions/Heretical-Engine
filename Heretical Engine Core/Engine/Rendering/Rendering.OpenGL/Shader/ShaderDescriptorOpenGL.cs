namespace HereticalSolutions.HereticalEngine.Rendering
{
	public struct ShaderDescriptorOpenGL
	{
		public string Name;

		public int Stride;

		public ShaderVertexAttributeOpenGL[] VertexAttributes;

		public ShaderSampler2DArgumentOpenGL[] Sampler2DArguments;
	}
}