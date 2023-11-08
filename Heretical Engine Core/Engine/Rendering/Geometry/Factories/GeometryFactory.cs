using HereticalSolutions.Collections.Managed;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Messaging;

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
			//https://learnopengl.com/Model-Loading/Mesh
			/*
			void setupMesh()
			{
				glGenVertexArrays(1, &VAO);
				glGenBuffers(1, &VBO);
				glGenBuffers(1, &EBO);

				glBindVertexArray(VAO);
				glBindBuffer(GL_ARRAY_BUFFER, VBO);

				glBufferData(GL_ARRAY_BUFFER, vertices.size() * sizeof(Vertex), &vertices[0], GL_STATIC_DRAW);

				glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
				glBufferData(GL_ELEMENT_ARRAY_BUFFER, indices.size() * sizeof(unsigned int), 
                 &indices[0], GL_STATIC_DRAW);

				// vertex positions
				glEnableVertexAttribArray(0);
				glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)0);
				// vertex normals
				glEnableVertexAttribArray(1);
				glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, Normal));
				// vertex texture coords
				glEnableVertexAttribArray(2);
				glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, TexCoords));

				glBindVertexArray(0);
			}
			*/

			//https://dotnet.github.io/Silk.NET/docs/opengl/c1/2-hello-quad.html

			var vertexArray = VertexFactory.BuildVertexArrayOpenGL<Vertex>(
				gl);

			vertexArray.Bind(
				gl);


			var verticesBuffer = BufferFactory.BuildBufferOpenGL<float>(
				gl,
				//geometry.VertexAttributes,
				BufferTargetARB.ArrayBuffer);

			verticesBuffer.Bind(
				gl);

			verticesBuffer.Update(
				gl,
				geometry.VertexAttributes);

			// vertex positions
			vertexArray.VertexAttributePointer(
				gl,
				0, //Position of the attribute in the shader. For instance, "layout (location = 0) in vec3 vPos;" -> location = 0 shows that this variable should be 0
				3, //"Since we're using a vec3, we tell it that the size is 3"
				VertexAttribPointerType.Float, //"Next up, we tell it that we're using floats"
				0);

			// uv0
			vertexArray.VertexAttributePointer(
				gl,
				1,
				2,
				VertexAttribPointerType.Float,
				48); //12 * 4

			var indicesBuffer = BufferFactory.BuildBufferOpenGL<uint>(
				gl,
				//geometry.Indices,
				BufferTargetARB.ElementArrayBuffer);

			indicesBuffer.Bind(
				gl);

			indicesBuffer.Update(
				gl,
				geometry.Indices);


			gl.BindVertexArray(0);

			//gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

			//gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

			return new GeometryOpenGL(
				geometry,
				vertexArray,
				verticesBuffer,
				indicesBuffer);
		}

		public static GeometryOpenGLStorageHandle BuildGeometryOpenGLStorageHandle(
			IReadOnlyResourceStorageHandle geometryRAMStorageHandle,
			GL gl,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
		{
			return new GeometryOpenGLStorageHandle(
				geometryRAMStorageHandle,
				gl,
				mainThreadCommandBuffer,
				logger);
		}

		public static ConcurrentGeometryOpenGLStorageHandle BuildConcurrentGeometryOpenGLStorageHandle(
			IReadOnlyResourceStorageHandle geometryRAMStorageHandle,
			GL gl,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
		{
			return new ConcurrentGeometryOpenGLStorageHandle(
				geometryRAMStorageHandle,
				new SemaphoreSlim(1, 1),
				gl,
				mainThreadCommandBuffer,
				logger);
		}
	}
}