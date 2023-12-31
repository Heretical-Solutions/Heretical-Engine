using HereticalSolutions.Logging;

using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.HereticalEngine.Application;

using Antlr4.Runtime;

using Silk.NET.OpenGL;

using Silk.NET.Assimp;

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

		public static void TryFillVertexAttributeValues(
			ref ShaderVertexAttributeOpenGL vertexAttribute,
			IFormatLogger logger)
		{
			#region Keyword parsing

			EVertexAttributeKeywords vertexAttributeKeyword;

			if (Enum.TryParse(
				vertexAttribute.Name,
				out vertexAttributeKeyword))
			{
				vertexAttribute.KeywordVertexAttribute = true;

				switch (vertexAttributeKeyword)
				{
					case EVertexAttributeKeywords.VertexPosition:
						if (vertexAttribute.Type != VEC3)
						{
							logger?.LogWarning(
								typeof(ShaderFactory),
								$"SHADER VARIABLE {vertexAttribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE KEYWORD, BUT IS NOT OF TYPE {VEC3}");

							break;
						}

						vertexAttribute.PointerType = VertexAttribPointerType.Float;
						vertexAttribute.AttributeSize = 3;
						vertexAttribute.ByteSize = sizeof(float) * 3;
						return;

					case EVertexAttributeKeywords.VertexNormal:
						if (vertexAttribute.Type != VEC3)
						{
							logger?.LogWarning(
								typeof(ShaderFactory),
								$"SHADER VARIABLE {vertexAttribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE KEYWORD, BUT IS NOT OF TYPE {VEC3}");

							break;
						}

						vertexAttribute.PointerType = VertexAttribPointerType.Float;
						vertexAttribute.AttributeSize = 3;
						vertexAttribute.ByteSize = sizeof(float) * 3;
						return;

					case EVertexAttributeKeywords.VertexTangent:
						if (vertexAttribute.Type != VEC3)
						{
							logger?.LogWarning(
								typeof(ShaderFactory),
								$"SHADER VARIABLE {vertexAttribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE KEYWORD, BUT IS NOT OF TYPE {VEC3}");

							break;
						}

						vertexAttribute.PointerType = VertexAttribPointerType.Float;
						vertexAttribute.AttributeSize = 3;
						vertexAttribute.ByteSize = sizeof(float) * 3;
						return;

					case EVertexAttributeKeywords.VertexBitangent:
						if (vertexAttribute.Type != VEC3)
						{
							logger?.LogWarning(
								typeof(ShaderFactory),
								$"SHADER VARIABLE {vertexAttribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE KEYWORD, BUT IS NOT OF TYPE {VEC3}");

							break;
						}

						vertexAttribute.PointerType = VertexAttribPointerType.Float;
						vertexAttribute.AttributeSize = 3;
						vertexAttribute.ByteSize = sizeof(float) * 3;
						return;

					case EVertexAttributeKeywords.VertexColor:
						if (vertexAttribute.Type != VEC4)
						{
							logger?.LogWarning(
								typeof(ShaderFactory),
								$"SHADER VARIABLE {vertexAttribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE KEYWORD, BUT IS NOT OF TYPE {VEC4}");

							break;
						}

						vertexAttribute.PointerType = VertexAttribPointerType.Float;
						vertexAttribute.AttributeSize = 4;
						vertexAttribute.ByteSize = sizeof(float) * 4;
						return;

					case EVertexAttributeKeywords.VertexUV0:
						if (vertexAttribute.Type != VEC2)
						{
							logger?.LogWarning(
								typeof(ShaderFactory),
								$"SHADER VARIABLE {vertexAttribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE KEYWORD, BUT IS NOT OF TYPE {VEC2}");

							break;
						}

						vertexAttribute.PointerType = VertexAttribPointerType.Float;
						vertexAttribute.AttributeSize = 2;
						vertexAttribute.ByteSize = sizeof(float) * 2;
						return;

					case EVertexAttributeKeywords.VertexUV1:
						if (vertexAttribute.Type != VEC2)
						{
							logger?.LogWarning(
								typeof(ShaderFactory),
								$"SHADER VARIABLE {vertexAttribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE KEYWORD, BUT IS NOT OF TYPE {VEC2}");

							break;
						}

						vertexAttribute.PointerType = VertexAttribPointerType.Float;
						vertexAttribute.AttributeSize = 2;
						vertexAttribute.ByteSize = sizeof(float) * 2;
						return;

					case EVertexAttributeKeywords.VertexUV2:
						if (vertexAttribute.Type != VEC2)
						{
							logger?.LogWarning(
								typeof(ShaderFactory),
								$"SHADER VARIABLE {vertexAttribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE KEYWORD, BUT IS NOT OF TYPE {VEC2}");

							break;
						}

						vertexAttribute.PointerType = VertexAttribPointerType.Float;
						vertexAttribute.AttributeSize = 2;
						vertexAttribute.ByteSize = sizeof(float) * 2;
						return;

					case EVertexAttributeKeywords.VertexUV3:
						if (vertexAttribute.Type != VEC2)
						{
							logger?.LogWarning(
								typeof(ShaderFactory),
								$"SHADER VARIABLE {vertexAttribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE KEYWORD, BUT IS NOT OF TYPE {VEC2}");

							break;
						}

						vertexAttribute.PointerType = VertexAttribPointerType.Float;
						vertexAttribute.AttributeSize = 2;
						vertexAttribute.ByteSize = sizeof(float) * 2;
						return;

					default:
						logger?.LogWarning(
							typeof(ShaderFactory),
							$"SHADER VARIABLE {vertexAttribute.Name} IS NAMED LIKE A VERTEX ATTRIBUTE KEYWORD, BUT PARSING LOGIC FOR THIS KIND OF ATTRIBUTE IS NOT IMPLEMENTED YET");

						return;
				}
			}

			#endregion

			vertexAttribute.KeywordVertexAttribute = false;

			switch (vertexAttribute.Type)
			{
				case DOUBLE:
					vertexAttribute.PointerType = VertexAttribPointerType.Double;
					vertexAttribute.AttributeSize = 1;
					vertexAttribute.ByteSize = sizeof(double);
					return;

				case FLOAT:
					vertexAttribute.PointerType = VertexAttribPointerType.Float;
					vertexAttribute.AttributeSize = 1;
					vertexAttribute.ByteSize = sizeof(float);
					return;

				case INT:
					vertexAttribute.PointerType = VertexAttribPointerType.Int;
					vertexAttribute.AttributeSize = 1;
					vertexAttribute.ByteSize = sizeof(int);
					return;

				case UINT:
					vertexAttribute.PointerType = VertexAttribPointerType.UnsignedInt;
					vertexAttribute.AttributeSize = 1;
					vertexAttribute.ByteSize = sizeof(uint);
					return;

				case VEC2:
					vertexAttribute.PointerType = VertexAttribPointerType.Float;
					vertexAttribute.AttributeSize = 2;
					vertexAttribute.ByteSize = sizeof(float) * 2;
					return;

				case VEC3:
					vertexAttribute.PointerType = VertexAttribPointerType.Float;
					vertexAttribute.AttributeSize = 3;
					vertexAttribute.ByteSize = sizeof(float) * 3;
					return;

				case VEC4:
					vertexAttribute.PointerType = VertexAttribPointerType.Float;
					vertexAttribute.AttributeSize = 4;
					vertexAttribute.ByteSize = sizeof(float) * 4;
					return;

				default:
					logger?.LogError(
						typeof(ShaderFactory),
						$"SHADER VARIABLE {vertexAttribute.Name} TYPE IS EITHER INVALID OR ITS PARSING IS NOT IMPLEMENTED YET");

					return;
			}
		}

		public static void TryFillSamplerArgumentValues(
			ref ShaderSampler2DArgumentOpenGL samplerArgument,
			IFormatLogger logger)
		{
			ETextureKeywords textureKeyword;

			if (Enum.TryParse(
				samplerArgument.Name,
				out textureKeyword))
			{
				samplerArgument.KeywordTexture = true;

				switch (textureKeyword)
				{
					case ETextureKeywords.TextureDiffuse:

						samplerArgument.Type = TextureType.Diffuse;

						break;

					case ETextureKeywords.TextureNormal:

						samplerArgument.Type = TextureType.Normals;

						break;

					default:
						logger?.LogWarning(
							typeof(ShaderFactory),
							$"TEXTURE SAMPLER {samplerArgument.Name} IS NAMED LIKE A TEXTURE ARGUMENT KEYWORD, BUT PARSING LOGIC FOR THIS KIND OF ARGUMENT IS NOT IMPLEMENTED YET");

						samplerArgument.Type = TextureType.None;

						break;
				}

				return;
			}

			samplerArgument.KeywordTexture = false;

			samplerArgument.Type = TextureType.None;
		}

		public static void ParseGLSL(
			string shaderCode,
			ref ShaderDescriptorOpenGL descriptor,
			bool parseVertexAttributes,
			bool parseUniformSamplers,
			IFormatLogger logger)
		{
			AntlrInputStream inputStream = new AntlrInputStream(shaderCode);

			GLSLLexer glslLexer = new GLSLLexer(inputStream);
			CommonTokenStream commonTokenStream = new CommonTokenStream(glslLexer);
			GLSLParser glslParser = new GLSLParser(commonTokenStream);

			GLSLParser.Translation_unitContext context = glslParser.translation_unit();

			ShaderDescriptorBuilder builder = new ShaderDescriptorBuilder(
				new List<ShaderVertexAttributeOpenGL>(),
				new List<ShaderSampler2DArgumentOpenGL>(),
				logger);

			builder.Result = descriptor;

			builder.ParseVertexAttributes = parseVertexAttributes;

			builder.ParseUniformSamplers = parseUniformSamplers;

			//var result = (ShaderDescriptorOpenGL)builder.Visit(context);

			builder.Visit(context);

			descriptor = builder.Result;

			//return result;
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

			gl.DetachShader(
				handle,
				vertex);

			gl.DetachShader(
				handle,
				fragment);

			gl.DeleteShader(vertex);

			gl.DeleteShader(fragment);

			descriptor = default;

			ParseGLSL(
				vertexShaderSource,
				ref descriptor,
				true,
				false,
				logger);

			ParseGLSL(
				fragmentShaderSource,
				ref descriptor,
				false,
				true,
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

			return handle;
		}

		public static ShaderOpenGLStorageHandle BuildShaderOpenGLStorageHandle(
			string vertexShaderSource,
			string fragmentShaderSource,
			ApplicationContext context)
		{
			return new ShaderOpenGLStorageHandle(
				OpenGLModule.GL_RESOURCE_PATH,
				vertexShaderSource,
				fragmentShaderSource,
				context);
		}

		public static ConcurrentShaderOpenGLStorageHandle BuildConcurrentShaderOpenGLStorageHandle(
			string vertexShaderSource,
			string fragmentShaderSource,
			ApplicationContext context)
		{
			return new ConcurrentShaderOpenGLStorageHandle(
				OpenGLModule.GL_RESOURCE_PATH,
				vertexShaderSource,
				fragmentShaderSource,
				new SemaphoreSlim(1, 1),
				context);
		}
	}
}