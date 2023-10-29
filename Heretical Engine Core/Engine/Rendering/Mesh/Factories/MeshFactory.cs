using HereticalSolutions.ResourceManagement;
using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class MeshFactory
	{
		public static MeshOpenGL BuildMeshOpenGL(
			GL gl,
			Mesh mesh)
		{
			var vertexArray = VertexFactory.BuildVertexArrayOpenGL<float, uint>(
				gl);

			var verticesBuffer = BufferFactory.BuildBufferOpenGL<float>(
				gl,
				mesh.Vertices,
				BufferTargetARB.ArrayBuffer);

			var indicesBuffer = BufferFactory.BuildBufferOpenGL<uint>(
				gl,
				mesh.Indices,
				BufferTargetARB.ElementArrayBuffer);

			vertexArray.VertexAttributePointer(
				gl,
				0,
				3,
				VertexAttribPointerType.Float,
				5,
				0);

			vertexArray.VertexAttributePointer(
				gl,
				1,
				2,
				VertexAttribPointerType.Float,
				5,
				3);

			return new MeshOpenGL(
				mesh,
				vertexArray,
				verticesBuffer,
				indicesBuffer);
		}

		public static MeshOpenGLStorageHandle BuildMeshOpenGLStorageHandle(
			IReadOnlyResourceStorageHandle meshRAMStorageHandle,
			GL gl)
		{
			return new MeshOpenGLStorageHandle(
				meshRAMStorageHandle,
				gl);
		}
	}
}