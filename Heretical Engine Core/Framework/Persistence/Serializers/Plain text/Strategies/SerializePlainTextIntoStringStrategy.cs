using HereticalSolutions.Persistence.Arguments;

namespace HereticalSolutions.Persistence.Serializers
{
	public class SerializePlainTextIntoStringStrategy : IPlainTextSerializationStrategy
	{
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