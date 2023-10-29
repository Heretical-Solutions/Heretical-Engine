// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MeshOpenGL
	{
		public Mesh Mesh { get; private set; }

		public VertexArrayOpenGL<float, uint> VertexArray { get; set; }

		public BufferOpenGL<float> VerticesBuffer { get; set; }

		public BufferOpenGL<uint> IndicesBuffer { get; set; }

		public MeshOpenGL(
			Mesh mesh,
			VertexArrayOpenGL<float, uint> vertexArray,
			BufferOpenGL<float> verticesBuffer,
			BufferOpenGL<uint> indicesBuffer)
		{
			Mesh = mesh;

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
			Mesh mesh)
		{
			Mesh = mesh;

			VerticesBuffer.Update(
				gl,
				mesh.Vertices);

			IndicesBuffer.Update(
				gl,
				mesh.Indices);
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