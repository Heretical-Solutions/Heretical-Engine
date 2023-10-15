namespace HereticalSolutions.Persistence.Serializers
{
	public interface IPlainTextSerializationStrategy
	{
		bool Serialize(ISerializationArgument argument, string text);

		bool Deserialize(ISerializationArgument argument, out string text);

		void Erase(ISerializationArgument argument);
	}
}