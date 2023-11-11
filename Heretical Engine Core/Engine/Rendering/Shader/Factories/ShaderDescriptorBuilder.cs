using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public class ShaderDescriptorBuilder
		: GLSLParserBaseVisitor<object>
	{
		private bool inAttributeFound = false;

		private bool layoutSpecified = false;

		private bool locationSpecified = false;

		private int locationIndex = -1;

		private string attributeName = string.Empty;

		private string attributeType = string.Empty;

		private List<ShaderAttributeOpenGL> attributes;

		private IFormatLogger logger;

		public ShaderDescriptorBuilder(
			List<ShaderAttributeOpenGL> attributes,
			IFormatLogger logger)
		{
			this.attributes = attributes;

			this.logger = logger;


			attributes.Clear();
		}

		public override object VisitTranslation_unit(
			GLSLParser.Translation_unitContext context)
		{
			logger.Log<ShaderDescriptorBuilder>(
				$"VISITING TRANSLATION UNIT");

			attributes.Clear();

			base.VisitTranslation_unit(context);

			ArrangeAttributesByLocation();

			int stride = CalculateStrideAndOffsets();

			var result = new ShaderDescriptorOpenGL
			{
				VertexAttributes = attributes.ToArray(),

				Stride = stride
			};

			attributes.Clear();

			return result;
		}

		private void ArrangeAttributesByLocation()
		{
			int currentLocation = 0;

			for (int i = 0; i < attributes.Count; i++)
			{
				if (attributes[i].Location != -1)
				{
					currentLocation = attributes[i].Location;
				}
				else
				{
					var attribute = attributes[i];

					attribute.Location = currentLocation;

					attributes[i] = attribute;

					currentLocation++;
				}
			}

			attributes.Sort((a, b) => a.Location.CompareTo(b.Location));
		}

		private int CalculateStrideAndOffsets()
		{
			int stride = 0;

			for (int i = 0; i < attributes.Count; i++)
			{
				var attribute = attributes[i];

				attribute.Offset = stride;

				attributes[i] = attribute;


				stride += attributes[i].ByteSize;
			}

			return stride;
		}

		public override object VisitDeclaration(
			GLSLParser.DeclarationContext context)
		{
			Clear();

			var declaration = context.GetText();

			base.VisitDeclaration(context);

			if (inAttributeFound)
			{
				var attribute = new ShaderAttributeOpenGL
				{
					Name = attributeName,
					Type = attributeType,
					Location = locationIndex
				};

				ShaderFactory.TryFillAttributeValues(
					ref attribute,
					logger);

				logger.Log<ShaderDescriptorBuilder>(
					$"PARSED ATTRIBUTE. NAME: {attribute.Name} TYPE: {attribute.Type} POINTER TYPE: {attribute.PointerType} INDEX: {attribute.Location} ATTRIBUTE SIZE: {attribute.AttributeSize} BYTE SIZE: {attribute.ByteSize} OFFSET: {attribute.Offset}");

				attributes.Add(attribute);
			}

			Clear();

			return null;
		}

		public override object VisitStorage_qualifier(
			GLSLParser.Storage_qualifierContext context)
		{
			var storageQualifier = context.GetText().ToLower();

			if (storageQualifier == "in")
			{
				inAttributeFound = true;
			}

			base.VisitStorage_qualifier(context);

			return null;
		}

		public override object VisitLayout_qualifier(
			GLSLParser.Layout_qualifierContext context)
		{
			layoutSpecified = true;

			base.VisitLayout_qualifier(context);

			return null;
		}

		public override object VisitLayout_qualifier_id(
			GLSLParser.Layout_qualifier_idContext context)
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
					logger.LogError<ShaderDescriptorBuilder>(
						$"COULD NOT PARSE INDEX: {index}");
				}
			}

			base.VisitLayout_qualifier_id(context);

			return null;
		}

		public override object VisitType_specifier(
			GLSLParser.Type_specifierContext context)
		{
			attributeType = context.type_specifier_nonarray().GetText();

			base.VisitType_specifier(context);

			return null;
		}

		public override object VisitSingle_declaration(
			GLSLParser.Single_declarationContext context)
		{
			var typelessDeclaration = context.typeless_declaration();

			if (typelessDeclaration != null)
			{
				attributeName = typelessDeclaration.IDENTIFIER().GetText();
			}

			base.VisitSingle_declaration(context);

			return null;
		}

		private void Clear()
		{
			inAttributeFound = false;

			layoutSpecified = false;

			locationSpecified = false;

			locationIndex = -1;

			attributeName = string.Empty;

			attributeType = string.Empty;
		}
	}
}
