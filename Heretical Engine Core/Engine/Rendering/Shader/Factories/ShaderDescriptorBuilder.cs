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

		private IFormatLogger logger;

		public ShaderDescriptorBuilder(
			IFormatLogger logger)
		{
			this.logger = logger;
		}

		public override object VisitDeclaration(
			GLSLParser.DeclarationContext context)
		{
			Clear();

			var declaration = context.GetText();

			logger.Log<ShaderDescriptorBuilder>(
				$"DECLARATION: {declaration}");

			base.VisitDeclaration(context);

			if (inAttributeFound)
			{
				logger.Log<ShaderDescriptorBuilder>(
					$"PARSED ATTRIBUTE. NAME: {attributeName} TYPE: {attributeType} INDEX: {locationIndex}");
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
				logger.Log<ShaderDescriptorBuilder>(
					"FOUND AN 'IN' ATTRIBUTE");

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
