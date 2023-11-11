// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class GeometryOpenGL
	{
		public Geometry Geometry { get; private set; }

		public VertexArrayObjectOpenGL VertexArrayObject { get; set; }

		public BufferObjectOpenGL<float> VertexBufferObject { get; set; }

		public BufferObjectOpenGL<uint> ElementBufferObject { get; set; }

		public GeometryOpenGL(
			Geometry geometry,
			VertexArrayObjectOpenGL vertexArrayObject,
			BufferObjectOpenGL<float> vertexBufferObject,
			BufferObjectOpenGL<uint> elementBufferObject)
		{
			Geometry = geometry;

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