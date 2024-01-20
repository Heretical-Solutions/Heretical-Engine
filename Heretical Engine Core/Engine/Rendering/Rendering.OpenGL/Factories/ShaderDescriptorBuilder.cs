using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public class ShaderDescriptorBuilder
		: GLSLParserBaseVisitor<object>
	{
		#region Parsing settings

		private ShaderDescriptorOpenGL result = default;

		public ShaderDescriptorOpenGL Result
		{
			get => result;
			set => result = value;
		}

		public bool ParseVertexAttributes { get; set; } = false;

		public bool ParseUniformSamplers { get; set; } = false;

		#endregion

		#region Local variables used in translation unit visit

		private bool insideFunctionDefinition = false;

		private int currentTextureIndex = 0;

		private List<ShaderVertexAttributeOpenGL> vertexAttributes;

		private List<ShaderSampler2DArgumentOpenGL> sampler2DArguments;

		#endregion

		#region Local variables used in declaration visit

		//Vertex attributes
		private bool inAttributeFound = false;

		private bool layoutSpecified = false;

		private bool locationSpecified = false;

		private int locationIndex = -1;

		private string attributeName = string.Empty;

		private string attributeType = string.Empty;


		//Uniform samplers
		private bool uniformFound = false;

		private bool sampler2DFound = false;

		private string samplerName = string.Empty;

		#endregion

		private readonly ILogger logger;

		public ShaderDescriptorBuilder(
			List<ShaderVertexAttributeOpenGL> vertexAttributes,
			List<ShaderSampler2DArgumentOpenGL> sampler2DArguments,
			ILogger logger = null)
		{
			this.vertexAttributes = vertexAttributes;

			this.sampler2DArguments = sampler2DArguments;

			this.logger = logger;


			vertexAttributes.Clear();
		}

		public override object VisitTranslation_unit(
			GLSLParser.Translation_unitContext context)
		{
			ClearTranslationUnitVisitVariables();

			logger?.Log<ShaderDescriptorBuilder>(
				$"VISITING TRANSLATION UNIT");

			base.VisitTranslation_unit(context);

			logger?.Log<ShaderDescriptorBuilder>(
				$"VISITING FINISHED");

			logger?.Log<ShaderDescriptorBuilder>(
				$"BUILDING STARTED");

			if (ParseVertexAttributes)
			{
				ArrangeAttributesByLocation();

				int stride = CalculateStrideAndOffsets();

				/*
				var result = new ShaderDescriptorOpenGL
				{
					VertexAttributes = vertexAttributes.ToArray(),

					Stride = stride
				};
				*/

				result.VertexAttributes = vertexAttributes.ToArray();

				result.Stride = stride;
			}

			if (ParseUniformSamplers)
			{
				result.Sampler2DArguments = sampler2DArguments.ToArray();
			}

			ClearTranslationUnitVisitVariables();

			logger?.Log<ShaderDescriptorBuilder>(
				$"BUILDING FINISHED");

			return result;
		}

		private void ArrangeAttributesByLocation()
		{
			int currentLocation = 0;

			for (int i = 0; i < vertexAttributes.Count; i++)
			{
				if (vertexAttributes[i].Location != -1)
				{
					currentLocation = vertexAttributes[i].Location;
				}
				else
				{
					var attribute = vertexAttributes[i];

					attribute.Location = currentLocation;

					vertexAttributes[i] = attribute;

					currentLocation++;
				}
			}

			vertexAttributes.Sort((a, b) => a.Location.CompareTo(b.Location));
		}

		private int CalculateStrideAndOffsets()
		{
			int stride = 0;

			for (int i = 0; i < vertexAttributes.Count; i++)
			{
				var attribute = vertexAttributes[i];

				attribute.Offset = stride;

				vertexAttributes[i] = attribute;


				stride += vertexAttributes[i].ByteSize;
			}

			return stride;
		}

		public override object VisitFunction_definition(
			GLSLParser.Function_definitionContext context)
		{
			insideFunctionDefinition = true;

			base.VisitFunction_definition(context);

			insideFunctionDefinition = false;

			return null;
		}

		public override object VisitDeclaration(
			GLSLParser.DeclarationContext context)
		{
			ClearDeclarationVisitVariables();

			var declaration = context.GetText();

			//logger?.Log<ShaderDescriptorBuilder>(
			//	$"DECLARATION: {declaration} INSIDE FUNCTION DEFINITION: {insideFunctionDefinition}");

			base.VisitDeclaration(context);

			if (ParseVertexAttributes)
			{
				if (insideFunctionDefinition)
				{
					ClearDeclarationVisitVariables();

					return null;
				}

				if (inAttributeFound)
				{
					var vertexAttribute = new ShaderVertexAttributeOpenGL
					{
						Name = attributeName,
						Type = attributeType,
						Location = locationIndex
					};

					ShaderFactory.TryFillVertexAttributeValues(
						ref vertexAttribute,
						logger);

					logger?.Log<ShaderDescriptorBuilder>(
						$"PARSED VERTEX ATTRIBUTE. NAME: {vertexAttribute.Name} TYPE: {vertexAttribute.Type} POINTER TYPE: {vertexAttribute.PointerType} INDEX: {vertexAttribute.Location} ATTRIBUTE SIZE: {vertexAttribute.AttributeSize} BYTE SIZE: {vertexAttribute.ByteSize} OFFSET: {vertexAttribute.Offset} KEYWORD: {vertexAttribute.KeywordVertexAttribute}");

					vertexAttributes.Add(vertexAttribute);
				}
			}

			if (ParseUniformSamplers)
			{
				if (insideFunctionDefinition)
				{
					ClearDeclarationVisitVariables();

					return null;
				}

				if (uniformFound && sampler2DFound)
				{
					string slotName = $"Texture{currentTextureIndex}";

					if (!Enum.TryParse(
						slotName,
						out TextureUnit textureSlot))
					{
						throw new Exception(
							logger.TryFormat<ShaderDescriptorBuilder>(
								$"COULD NOT PARSE TextureUnit: {slotName}"));
					}

					var sampler2DArgument = new ShaderSampler2DArgumentOpenGL
					{
						Name = samplerName,

						TextureSlot = textureSlot
					};

					ShaderFactory.TryFillSamplerArgumentValues(
						ref sampler2DArgument,
						logger);

					currentTextureIndex++;

					logger?.Log<ShaderDescriptorBuilder>(
						$"PARSED UNIFORM SAMPLER. NAME: {sampler2DArgument.Name} TEXTURE SLOT: {sampler2DArgument.TextureSlot} TEXTURE TYPE: {sampler2DArgument.Type} KEYWORD: {sampler2DArgument.KeywordTexture}");

					sampler2DArguments.Add(sampler2DArgument);
				}
			}

			ClearDeclarationVisitVariables();

			return null;
		}

		public override object VisitStorage_qualifier(
			GLSLParser.Storage_qualifierContext context)
		{
			if (ParseVertexAttributes)
			{
				var storageQualifier = context.GetText().ToLower();

				if (storageQualifier == "in")
				{
					inAttributeFound = true;
				}
			}

			if (ParseUniformSamplers)
			{
				var storageQualifier = context.GetText().ToLower();

				if (storageQualifier == "uniform")
				{
					uniformFound = true;
				}
			}

			base.VisitStorage_qualifier(context);

			return null;
		}

		public override object VisitLayout_qualifier(
			GLSLParser.Layout_qualifierContext context)
		{
			if (ParseVertexAttributes)
			{
				layoutSpecified = true;
			}

			base.VisitLayout_qualifier(context);

			return null;
		}

		public override object VisitLayout_qualifier_id(
			GLSLParser.Layout_qualifier_idContext context)
		{
			if (ParseVertexAttributes)
			{
				var identifier = context.IDENTIFIER().GetText().ToLower();

				if (identifier == "location")
				{
					locationSpecified = true;
				}

				var constantExpression = context.constant_expression();

				if (constantExpression != null)
				{
					var index = constantExpression.GetText();

					if (!int.TryParse(
						index,
						out locationIndex))
					{
						logger?.LogError<ShaderDescriptorBuilder>(
							$"COULD NOT PARSE INDEX: {index}");
					}
				}
			}

			base.VisitLayout_qualifier_id(context);

			return null;
		}

		public override object VisitType_specifier(
			GLSLParser.Type_specifierContext context)
		{
			if (ParseVertexAttributes)
			{
				attributeType = context.type_specifier_nonarray().GetText();
			}

			if (ParseUniformSamplers)
			{
				var typeSpecifier = context.type_specifier_nonarray().GetText().ToLower();

				if (typeSpecifier == "sampler2d")
				{
					sampler2DFound = true;
				}
			}

			base.VisitType_specifier(context);

			return null;
		}

		public override object VisitSingle_declaration(
			GLSLParser.Single_declarationContext context)
		{
			if (ParseVertexAttributes)
			{
				var typelessDeclaration = context.typeless_declaration();

				if (typelessDeclaration != null)
				{
					attributeName = typelessDeclaration.IDENTIFIER().GetText();
				}
			}

			if (ParseUniformSamplers)
			{
				var typelessDeclaration = context.typeless_declaration();

				if (typelessDeclaration != null)
				{
					samplerName = typelessDeclaration.IDENTIFIER().GetText();
				}
			}

			base.VisitSingle_declaration(context);

			return null;
		}

		private void ClearDeclarationVisitVariables()
		{
			//Vertex attributes
			inAttributeFound = false;

			layoutSpecified = false;

			locationSpecified = false;

			locationIndex = -1;

			attributeName = string.Empty;

			attributeType = string.Empty;


			//Uniform samplers
			uniformFound = false;

			sampler2DFound = false;

			samplerName = string.Empty;
		}

		private void ClearTranslationUnitVisitVariables()
		{
			insideFunctionDefinition = false;

			currentTextureIndex = 0;

			vertexAttributes.Clear();

			sampler2DArguments.Clear();
		}
	}
}
