// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public struct GeometryOpenGL
	{
		public VertexArrayObjectOpenGL VertexArrayObject;

		public BufferObjectOpenGL<float> VertexBufferObject;

		public BufferObjectOpenGL<uint> ElementBufferObject;

		public GeometryOpenGL(
			VertexArrayObjectOpenGL vertexArrayObject,
			BufferObjectOpenGL<float> vertexBufferObject,
			BufferObjectOpenGL<uint> elementBufferObject)
		{
			VertexArrayObject = vertexArrayObject;

			VertexBufferObject = vertexBufferObject;

			ElementBufferObject = elementBufferObject;
		}

		public void Bind(
			GL gl)
		{
			VertexArrayObject.Bind(gl);
		}

		public void Dispose(
			GL gl)
		{
			VertexArrayObject.Dispose(gl);

			VertexBufferObject.Dispose(gl);

			ElementBufferObject.Dispose(gl);
		}
	}
}