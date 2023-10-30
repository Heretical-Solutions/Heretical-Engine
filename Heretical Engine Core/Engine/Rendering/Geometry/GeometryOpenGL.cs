// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class GeometryOpenGL
	{
		public Geometry Geometry { get; private set; }

		public VertexArrayOpenGL<float, uint> VertexArray { get; set; }

		public BufferOpenGL<float> VerticesBuffer { get; set; }

		public BufferOpenGL<uint> IndicesBuffer { get; set; }

		public GeometryOpenGL(
			Geometry geometry,
			VertexArrayOpenGL<float, uint> vertexArray,
			BufferOpenGL<float> verticesBuffer,
			BufferOpenGL<uint> indicesBuffer)
		{
			Geometry = geometry;

			VertexArray = vertexArray;

			VerticesBuffer = verticesBuffer;

			IndicesBuffer = indicesBuffer;
		}

		public void Bind(
			GL gl)
		{
			VertexArray.Bind(gl);
		}

		public void Update(
			GL gl,
			Geometry geometry)
		{
			Geometry = geometry;

			VerticesBuffer.Update(
				gl,
				geometry.Vertices);

			IndicesBuffer.Update(
				gl,
				geometry.Indices);
		}

		public void Dispose(
			GL gl)
		{
			VertexArray.Dispose(gl);

			VerticesBuffer.Dispose(gl);

			IndicesBuffer.Dispose(gl);
		}
	}
}