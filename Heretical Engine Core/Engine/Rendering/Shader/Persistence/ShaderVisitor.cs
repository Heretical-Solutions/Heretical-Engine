using HereticalSolutions.Persistence;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ShaderVisitor
		: ILoadVisitorGeneric<Shader, VertexFragmentShaderDTO>
	{
		private GL gl;

		public ShaderVisitor(GL gl)
		{
			this.gl = gl;
		}

		#region ILoadVisitorGeneric

		public bool Load(VertexFragmentShaderDTO DTO, out Shader value)
		{
			value = new Shader(
				gl,
				DTO.Vertex.Text,
				DTO.Fragment.Text);

			return true;
		}

		public bool Load(VertexFragmentShaderDTO DTO, Shader valueToPopulate)
		{
			throw new Exception("[ShaderVisitor] POPULATING AN EXISTING SHADER IS A BAD IDEA");
		}

		#endregion
	}
}