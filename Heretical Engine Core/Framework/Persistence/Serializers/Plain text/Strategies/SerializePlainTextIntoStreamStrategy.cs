using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
	public class SerializePlainTextIntoStreamStrategy : IPlainTextSerializationStrategy
	{
		public bool Serialize(ISerializationArgument argument, string text)
		{
			FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;

			if (!StreamIO.OpenWriteStream(fileSystemSettings, out StreamWriter streamWriter))
				return false;

			streamWriter.Write(text);

			StreamIO.CloseStream(streamWriter);

			return true;
		}

		public bool Deserialize(ISerializationArgument argument, out string text)
		{
			FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;

			text = string.Empty;

			if (!StreamIO.OpenReadStream(fileSystemSettings, out StreamReader streamReader))
				return false;

			text = streamReader.ReadToEnd();

			StreamIO.CloseStream(streamReader);

			return true;
		}

		public void Erase(ISerializationArgument argument)
		{
			FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;

			StreamIO.Erase(fileSystemSettings);
		}
	}
}