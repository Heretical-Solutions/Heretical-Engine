using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
	public class SerializePlainTextIntoTextFileStrategy : IPlainTextSerializationStrategy
	{
		public bool Serialize(ISerializationArgument argument, string text)
		{
			FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;

			return TextFileIO.Write(filePathSettings, text);
		}

		public bool Deserialize(ISerializationArgument argument, out string text)
		{
			FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;

			return TextFileIO.Read(filePathSettings, out text);
		}

		public void Erase(ISerializationArgument argument)
		{
			FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;

			TextFileIO.Erase(filePathSettings);
		}
	}
}