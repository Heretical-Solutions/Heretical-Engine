using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
	public class SerializePlainTextIntoTextFileStrategy : IPlainTextSerializationStrategy
	{
		public bool Serialize(ISerializationArgument argument, string text)
		{
			FileSystemSettings fileSystemSettings = ((TextFileArgument)argument).Settings;

			return TextFileIO.Write(fileSystemSettings, text);
		}

		public bool Deserialize(ISerializationArgument argument, out string text)
		{
			FileSystemSettings fileSystemSettings = ((TextFileArgument)argument).Settings;

			return TextFileIO.Read(fileSystemSettings, out text);
		}

		public void Erase(ISerializationArgument argument)
		{
			FileSystemSettings fileSystemSettings = ((TextFileArgument)argument).Settings;

			TextFileIO.Erase(fileSystemSettings);
		}
	}
}