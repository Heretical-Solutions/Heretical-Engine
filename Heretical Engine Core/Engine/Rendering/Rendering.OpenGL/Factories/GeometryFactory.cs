using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class GeometryFactory
	{
		public static GeometryOpenGL BuildGeometryOpenGL(
			GL gl,
			GeometryRAM ramGeometry,
			ShaderDescriptorOpenGL shaderDescriptor,
			IFormatLogger logger)
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

			var vao = VertexFactory.BuildVertexArrayOpenGL(
				gl);

			vao.Bind(
				gl);


			var vbo = BufferFactory.BuildBufferOpenGL<float>(
				gl,
				BufferTargetARB.ArrayBuffer);

			vbo.Bind(
				gl);

			vbo.Update(
				gl,
				BuildVertexBufferObject(
					ramGeometry.Vertices,
					shaderDescriptor,
					logger));


			BindAttributePointers(
				shaderDescriptor,
				vao,
				gl);


			var ebo = BufferFactory.BuildBufferOpenGL<uint>(
				gl,
				BufferTargetARB.ElementArrayBuffer);

			ebo.Bind(
				gl);

			ebo.Update(
				gl,
				ramGeometry.Indices);


			gl.BindVertexArray(0);

			gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

			gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

			return new GeometryOpenGL(
				//ramGeometry,
				vao,
				vbo,
				ebo);
		}

		private static float[] BuildVertexBufferObject(
			Vertex[] vertices,
			ShaderDescriptorOpenGL shaderDescriptor,
			IFormatLogger logger)
		{
			float[] result = new float[vertices.Length * shaderDescriptor.Stride];

			for (int i = 0; i < shaderDescriptor.VertexAttributes.Length; i++)
			{
				if (shaderDescriptor.VertexAttributes[i].KeywordVertexAttribute)
				{
					FillVBOWithKeywordedAttribute(
						vertices,
						shaderDescriptor.VertexAttributes[i],
						result,
						shaderDescriptor.Stride,
						logger);
				}
			}

			return result;
		}

		private static void FillVBOWithKeywordedAttribute(
			Vertex[] vertices,
			ShaderVertexAttributeOpenGL vertexAttribute,
			float[] vbo,
			int stride,
			IFormatLogger logger)
		{
			int strideInFloat = (int)(stride / sizeof(float));

			int offsetInFloat = (int)(vertexAttribute.Offset / sizeof(float));

			EVertexAttributeKeywords vertexAttributeKeyword;

			if (Enum.TryParse(
				vertexAttribute.Name,
				out vertexAttributeKeyword))
			{
				vertexAttribute.KeywordVertexAttribute = true;

				switch (vertexAttributeKeyword)
				{
					case EVertexAttributeKeywords.VertexPosition:

						for (int i = 0; i < vertices.Length; i++)
						{
							vbo[i * strideInFloat + offsetInFloat] = vertices[i].Position.X;

							vbo[i * strideInFloat + offsetInFloat + 1] = vertices[i].Position.Y;

							vbo[i * strideInFloat + offsetInFloat + 2] = vertices[i].Position.Z;
						}

						return;

					case EVertexAttributeKeywords.VertexNormal:

						for (int i = 0; i < vertices.Length; i++)
						{
							vbo[i * strideInFloat + offsetInFloat] = vertices[i].Normal.X;

							vbo[i * strideInFloat + offsetInFloat + 1] = vertices[i].Normal.Y;

							vbo[i * strideInFloat + offsetInFloat + 2] = vertices[i].Normal.Z;
						}

						return;

					case EVertexAttributeKeywords.VertexTangent:
						
						for (int i = 0; i < vertices.Length; i++)
						{
							vbo[i * strideInFloat + offsetInFloat] = vertices[i].Tangent.X;

							vbo[i * strideInFloat + offsetInFloat + 1] = vertices[i].Tangent.Y;

							vbo[i * strideInFloat + offsetInFloat + 2] = vertices[i].Tangent.Z;
						}

						return;

					case EVertexAttributeKeywords.VertexBitangent:
						
						for (int i = 0; i < vertices.Length; i++)
						{
							vbo[i * strideInFloat + offsetInFloat] = vertices[i].Bitangent.X;

							vbo[i * strideInFloat + offsetInFloat + 1] = vertices[i].Bitangent.Y;

							vbo[i * strideInFloat + offsetInFloat + 2] = vertices[i].Bitangent.Z;
						}

						return;

					case EVertexAttributeKeywords.VertexColor:
						
						for (int i = 0; i < vertices.Length; i++)
						{
							vbo[i * strideInFloat + offsetInFloat] = vertices[i].Color.X;

							vbo[i * strideInFloat + offsetInFloat + 1] = vertices[i].Color.Y;

							vbo[i * strideInFloat + offsetInFloat + 2] = vertices[i].Color.Z;

							vbo[i * strideInFloat + offsetInFloat + sizeof(float) * 3] = vertices[i].Color.W;
						}

						return;

					case EVertexAttributeKeywords.VertexUV0:
						
						for (int i = 0; i < vertices.Length; i++)
						{
							vbo[i * strideInFloat + offsetInFloat] = vertices[i].UV0.X;

							vbo[i * strideInFloat + offsetInFloat + 1] = vertices[i].UV0.Y;
						}

						return;

					case EVertexAttributeKeywords.VertexUV1:

						for (int i = 0; i < vertices.Length; i++)
						{
							vbo[i * strideInFloat + offsetInFloat] = vertices[i].UV1.X;

							vbo[i * strideInFloat + offsetInFloat + 1] = vertices[i].UV1.Y;
						}

						return;

					case EVertexAttributeKeywords.VertexUV2:

						for (int i = 0; i < vertices.Length; i++)
						{
							vbo[i * strideInFloat + offsetInFloat] = vertices[i].UV2.X;

							vbo[i * strideInFloat + offsetInFloat + 1] = vertices[i].UV2.Y;
						}
						
						return;

					case EVertexAttributeKeywords.VertexUV3:

						for (int i = 0; i < vertices.Length; i++)
						{
							vbo[i * strideInFloat + offsetInFloat] = vertices[i].UV3.X;

							vbo[i * strideInFloat + offsetInFloat + 1] = vertices[i].UV3.Y;
						}

						return;

					default:
						logger?.LogWarning(
							typeof(GeometryFactory),
							$"CANNOT FILL VBO WITH VALUES OF ATTRIBUTE {vertexAttribute.Name}");

						return;
				}
			}
		}

		private static void BindAttributePointers(
			ShaderDescriptorOpenGL descriptor,
			VertexArrayObjectOpenGL vao,
			GL gl)
		{
			for (int i = 0; i < descriptor.VertexAttributes.Length; i++)
				vao.VertexAttributePointer(
					gl,
					(uint)descriptor.VertexAttributes[i].Location,
					descriptor.VertexAttributes[i].AttributeSize,
					descriptor.VertexAttributes[i].PointerType,
					(uint)descriptor.Stride,
					descriptor.VertexAttributes[i].Offset);
		}

		#if FIXME

		public static GeometryOpenGLStorageHandle BuildGeometryOpenGLStorageHandle(
			string shaderOpenGLPath,
			string shaderOpenGLVariantID,
			string geometryRAMPath,
			string geometryRAMVariantID,
			ApplicationContext context)
		{
			return new GeometryOpenGLStorageHandle(
				OpenGLModule.GL_RESOURCE_PATH,
				shaderOpenGLPath,
				shaderOpenGLVariantID,
				geometryRAMPath,
				geometryRAMVariantID,
				context);
		}

		public static ConcurrentGeometryOpenGLStorageHandle BuildConcurrentGeometryOpenGLStorageHandle(
			string shaderOpenGLResourcePath,
			string shaderOpenGLResourceVariantID,
			string geometryRAMResourcePath,
			string geometryRAMResourceVariantID,
			ApplicationContext context)
		{
			return new ConcurrentGeometryOpenGLStorageHandle(
				OpenGLModule.GL_RESOURCE_PATH,
				shaderOpenGLResourcePath,
				shaderOpenGLResourceVariantID,
				geometryRAMResourcePath,
				geometryRAMResourceVariantID,
				new SemaphoreSlim(1, 1),
				context);
		}

		#endif
	}
}