using HereticalSolutions.Collections.Managed;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

using Antlr4.Runtime;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class ShaderFactory
	{
		//public const string BOOL = "bool";

		/*
		public const string BVEC2 = "bvec2";

		public const string BVEC3 = "bvec3";
		
		public const string BVEC4 = "bvec4";
		*/

		public const string DOUBLE = "double";
		
		/*
		public const string DVEC2 = "dvec2";
		
		public const string DVEC3 = "dvec3";
		
		public const string DVEC4 = "dvec4";
		*/

		public const string FLOAT = "float";
		
		public const string INT = "int";
		
		/*
		public const string IVEC2 = "ivec2";
		
		public const string IVEC3 = "ivec3";
		
		public const string IVEC4 = "ivec4";
		*/

		public const string UINT = "uint";
		
		/*
		public const string UVEC2 = "uvec2";
		
		public const string UVEC3 = "uvec3";
		
		public const string UVEC4 = "uvec4";
		*/
		
		public const string VEC2 = "vec2";
		
		public const string VEC3 = "vec3";
		
		public const string VEC4 = "vec4";

		public static void TryFillAttributeValues(
			ref ShaderAttributeOpenGL attribute,
			IFormatLogger logger)
		{
			ECommonVertexAttributes vertexAttribute;

			if (Enum.TryParse(
				attribute.Name,
				out vertexAttribute))
			{
				attribute.CommonVertexAttribute = true;

				switch (vertexAttribute)
				{
					case ECommonVertexAttributes.VertexPosition:
						if (attribute.Type != VEC3)
						{
							logger?.LogWarning(
								$"[ShaderFactory] SHADER VARIABLE {attribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE, BUT IS NOT OF TYPE {VEC3}");

							break;
						}

						attribute.PointerType = VertexAttribPointerType.Float;
						attribute.AttributeSize = 3;
						attribute.ByteSize = sizeof(float) * 3;
						return;

					case ECommonVertexAttributes.VertexNormal:
						if (attribute.Type != VEC3)
						{
							logger?.LogWarning(
								$"[ShaderFactory] SHADER VARIABLE {attribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE, BUT IS NOT OF TYPE {VEC3}");

							break;
						}

						attribute.PointerType = VertexAttribPointerType.Float;
						attribute.AttributeSize = 3;
						attribute.ByteSize = sizeof(float) * 3;
						return;

					case ECommonVertexAttributes.VertexTangent:
						if (attribute.Type != VEC3)
						{
							logger?.LogWarning(
								$"[ShaderFactory] SHADER VARIABLE {attribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE, BUT IS NOT OF TYPE {VEC3}");

							break;
						}

						attribute.PointerType = VertexAttribPointerType.Float;
						attribute.AttributeSize = 3;
						attribute.ByteSize = sizeof(float) * 3;
						return;

					case ECommonVertexAttributes.VertexBitangent:
						if (attribute.Type != VEC3)
						{
							logger?.LogWarning(
								$"[ShaderFactory] SHADER VARIABLE {attribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE, BUT IS NOT OF TYPE {VEC3}");

							break;
						}

						attribute.PointerType = VertexAttribPointerType.Float;
						attribute.AttributeSize = 3;
						attribute.ByteSize = sizeof(float) * 3;
						return;

					case ECommonVertexAttributes.VertexColor:
						if (attribute.Type != VEC4)
						{
							logger?.LogWarning(
								$"[ShaderFactory] SHADER VARIABLE {attribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE, BUT IS NOT OF TYPE {VEC4}");

							break;
						}

						attribute.PointerType = VertexAttribPointerType.Float;
						attribute.AttributeSize = 4;
						attribute.ByteSize = sizeof(float) * 4;
						return;

					case ECommonVertexAttributes.VertexUV0:
						if (attribute.Type != VEC2)
						{
							logger?.LogWarning(
								$"[ShaderFactory] SHADER VARIABLE {attribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE, BUT IS NOT OF TYPE {VEC2}");

							break;
						}

						attribute.PointerType = VertexAttribPointerType.Float;
						attribute.AttributeSize = 2;
						attribute.ByteSize = sizeof(float) * 2;
						return;

					case ECommonVertexAttributes.VertexUV1:
						if (attribute.Type != VEC2)
						{
							logger?.LogWarning(
								$"[ShaderFactory] SHADER VARIABLE {attribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE, BUT IS NOT OF TYPE {VEC2}");

							break;
						}

						attribute.PointerType = VertexAttribPointerType.Float;
						attribute.AttributeSize = 2;
						attribute.ByteSize = sizeof(float) * 2;
						return;

					case ECommonVertexAttributes.VertexUV2:
						if (attribute.Type != VEC2)
						{
							logger?.LogWarning(
								$"[ShaderFactory] SHADER VARIABLE {attribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE, BUT IS NOT OF TYPE {VEC2}");

							break;
						}

						attribute.PointerType = VertexAttribPointerType.Float;
						attribute.AttributeSize = 2;
						attribute.ByteSize = sizeof(float) * 2;
						return;

					case ECommonVertexAttributes.VertexUV3:
						if (attribute.Type != VEC2)
						{
							logger?.LogWarning(
								$"[ShaderFactory] SHADER VARIABLE {attribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE, BUT IS NOT OF TYPE {VEC2}");

							break;
						}

						attribute.PointerType = VertexAttribPointerType.Float;
						attribute.AttributeSize = 2;
						attribute.ByteSize = sizeof(float) * 2;
						return;

					default:
						logger?.LogWarning(
							$"[ShaderFactory] SHADER VARIABLE {attribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE, BUT PARSING LOGIC FOR THIS KIND OF ATTRIBUTE IS NOT IMPLEMENTED YET");

						return;
				}
			}

			attribute.CommonVertexAttribute = false;

			switch (attribute.Type)
			{
				case DOUBLE:
					attribute.PointerType = VertexAttribPointerType.Double;
					attribute.AttributeSize = 1;
					attribute.ByteSize = sizeof(double);
					return;

				case FLOAT:
					attribute.PointerType = VertexAttribPointerType.Float;
					attribute.AttributeSize = 1;
					attribute.ByteSize = sizeof(float);
					return;

				case INT:
					attribute.PointerType = VertexAttribPointerType.Int;
					attribute.AttributeSize = 1;
					attribute.ByteSize = sizeof(int);
					return;

				case UINT:
					attribute.PointerType = VertexAttribPointerType.UnsignedInt;
					attribute.AttributeSize = 1;
					attribute.ByteSize = sizeof(uint);
					return;

				case VEC2:
					attribute.PointerType = VertexAttribPointerType.Float;
					attribute.AttributeSize = 2;
					attribute.ByteSize = sizeof(float) * 2;
					return;

				case VEC3:
					attribute.PointerType = VertexAttribPointerType.Float;
					attribute.AttributeSize = 3;
					attribute.ByteSize = sizeof(float) * 3;
					return;

				case VEC4:
					attribute.PointerType = VertexAttribPointerType.Float;
					attribute.AttributeSize = 4;
					attribute.ByteSize = sizeof(float) * 4;
					return;

				default:
					logger?.LogError(
						$"[ShaderFactory] SHADER VARIABLE {attribute.Name} TYPE IS EITHER INVALID OR ITS PARSING IS NOT IMPLEMENTED YET");

					return;
			}
		}

		public static ShaderDescriptorOpenGL ParseGLSL(
			string shaderCode,
			IFormatLogger logger)
		{
			AntlrInputStream inputStream = new AntlrInputStream(shaderCode);

			GLSLLexer glslLexer = new GLSLLexer(inputStream);
			CommonTokenStream commonTokenStream = new CommonTokenStream(glslLexer);
			GLSLParser glslParser = new GLSLParser(commonTokenStream);

			GLSLParser.Translation_unitContext context = glslParser.translation_unit();

			//GLSLParserBaseVisitor<object> visitor = new GLSLParserBaseVisitor<object>();
			//visitor.Visit(context);

			ShaderDescriptorBuilder builder = new ShaderDescriptorBuilder(
				new List<ShaderAttributeOpenGL>(),
				logger);

			var result = (ShaderDescriptorOpenGL)builder.Visit(context);

			return result;
		}

		public static bool BuildShaderProgram(
			string vertexShaderSource,
			string fragmentShaderSource,
			GL gl,
			IFormatLogger logger,
			out uint handle,
			out ShaderDescriptorOpenGL descriptor,
			out ShaderResourceMetadata VertexShaderMetadata,
			out ShaderResourceMetadata FragmentShaderMetadata,
			out ShaderResourceMetadata ShaderProgramMetadata)
		{
			handle = 0;

			uint vertex = CompileShader(
				gl,
				vertexShaderSource,
				ShaderType.VertexShader,
				out VertexShaderMetadata);

			if (!VertexShaderMetadata.Compiled)
			{
				FragmentShaderMetadata = new ShaderResourceMetadata
				{
					Compiled = false,

					CompilationLog = string.Empty
				};

				ShaderProgramMetadata = new ShaderResourceMetadata
				{
					Compiled = false,

					CompilationLog = string.Empty
				};

				descriptor = default;

				return false;
			}

			uint fragment = CompileShader(
				gl,
				fragmentShaderSource,
				ShaderType.FragmentShader,
				out FragmentShaderMetadata);

			if (!FragmentShaderMetadata.Compiled)
			{
				ShaderProgramMetadata = new ShaderResourceMetadata
				{
					Compiled = false,

					CompilationLog = string.Empty
				};

				descriptor = default;

				return false;
			}

			handle = gl.CreateProgram();

			gl.AttachShader(
				handle,
				vertex);

			gl.AttachShader(
				handle,
				fragment);

			gl.LinkProgram(handle);

			gl.GetProgram(
				handle,
				GLEnum.LinkStatus,
				out var status);

			ShaderProgramMetadata = default;

			ShaderProgramMetadata.Compiled = status != 0;

			ShaderProgramMetadata.CompilationLog = gl.GetProgramInfoLog(handle);

			if (!ShaderProgramMetadata.Compiled)
			{
				descriptor = default;

				return false;
			}

			/*
			if (status == 0)
			{
				throw new Exception($"Program failed to link with error: {gl.GetProgramInfoLog(handle)}");
			}
			*/

			gl.DetachShader(
				handle,
				vertex);

			gl.DetachShader(
				handle,
				fragment);

			gl.DeleteShader(vertex);

			gl.DeleteShader(fragment);

			descriptor = ParseGLSL(
				vertexShaderSource,
				logger);

			return true;
		}

		private static uint CompileShader(
			GL gl,
			string source,
			ShaderType type,
			out ShaderResourceMetadata metadata)
		{
			uint handle = gl.CreateShader(type);

			gl.ShaderSource(
				handle,
				source);

			gl.CompileShader(handle);

			string infoLog = gl.GetShaderInfoLog(handle);

			metadata.Compiled = string.IsNullOrWhiteSpace(infoLog);

			metadata.CompilationLog = infoLog;

			/*
			if (!string.IsNullOrWhiteSpace(infoLog))
			{
				throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
			}
			*/

			return handle;
		}

		public static ShaderOpenGLStorageHandle BuildShaderOpenGLStorageHandle(
			string vertexShaderSource,
			string fragmentShaderSource,
			GL gl,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
		{
			return new ShaderOpenGLStorageHandle(
				vertexShaderSource,
				fragmentShaderSource,
				gl,
				mainThreadCommandBuffer,
				logger);
		}

		public static ConcurrentShaderOpenGLStorageHandle BuildConcurrentShaderOpenGLStorageHandle(
			string vertexShaderSource,
			string fragmentShaderSource,
			GL gl,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
		{
			return new ConcurrentShaderOpenGLStorageHandle(
				vertexShaderSource,
				fragmentShaderSource,
				gl,
				mainThreadCommandBuffer,
				new SemaphoreSlim(1, 1),
				logger);
		}
	}
}