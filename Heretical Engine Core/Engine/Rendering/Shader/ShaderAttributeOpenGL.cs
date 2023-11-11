using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public struct ShaderAttributeOpenGL
	{
		public string Name;

		public string Type;

		public VertexAttribPointerType PointerType;

		public int Location;

		public int AttributeSize;

		public int ByteSize;

		public int Offset;

		public bool CommonVertexAttribute;
	}
}