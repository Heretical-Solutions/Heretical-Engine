using HereticalSolutions.Persistence.Arguments;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Serializers
{
	public class SerializePlainTextIntoStringStrategy : IPlainTextSerializationStrategy
	{
		private readonly ILogger logger;

		public SerializePlainTextIntoStringStrategy(
			ILogger logger = null)
		{
			this.logger = logger;
		}

		public bool Serialize(ISerializationArgument argument, string text)
		{
			((StringArgument)argument).Value = text;

			return true;
		}

		public bool Deserialize(ISerializationArgument argument, out string text)
		{
			text = ((StringArgument)argument).Value;

			return true;
		}

		public void Erase(ISerializationArgument argument)
		{
			((StringArgument)argument).Value = string.Empty;
		}
	}
}