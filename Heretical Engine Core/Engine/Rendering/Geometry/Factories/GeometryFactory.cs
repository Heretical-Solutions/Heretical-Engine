using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class GeometryFactory
	{
		public static GeometryOpenGL BuildGeometryOpenGL(
			GL gl,
			Geometry geometry)
		{
			var vertexArray = VertexFactory.BuildVertexArrayOpenGL<float, uint>(
				gl);

			var verticesBuffer = BufferFactory.BuildBufferOpenGL<float>(
				gl,
				geometry.Vertices,
				BufferTargetARB.ArrayBuffer);

			var indicesBuffer = BufferFactory.BuildBufferOpenGL<uint>(
				gl,
				geometry.Indices,
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

			return new GeometryOpenGL(
				geometry,
				vertexArray,
				verticesBuffer,
				indicesBuffer);
		}

		public static GeometryOpenGLStorageHandle BuildGeometryOpenGLStorageHandle(
			IReadOnlyResourceStorageHandle geometryRAMStorageHandle,
			GL gl,
			IFormatLogger logger)
		{
			return new GeometryOpenGLStorageHandle(
				geometryRAMStorageHandle,
				gl,
				logger);
		}
	}
}