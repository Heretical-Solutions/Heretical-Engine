using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
	public class SerializePlainTextIntoStreamStrategy : IPlainTextSerializationStrategy
	{
		public bool Serialize(ISerializationArgument argument, string text)
		{
			FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;

			if (!StreamIO.OpenWriteStream(filePathSettings, out StreamWriter streamWriter))
				return false;

			streamWriter.Write(text);

			StreamIO.CloseStream(streamWriter);

			return true;
		}

		public bool Deserialize(ISerializationArgument argument, out string text)
		{
			FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;

			text = string.Empty;

			if (!StreamIO.OpenReadStream(filePathSettings, out StreamReader streamReader))
				return false;

			text = streamReader.ReadToEnd();

			StreamIO.CloseStream(streamReader);

			return true;
		}

		public void Erase(ISerializationArgument argument)
		{
			FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;

			StreamIO.Erase(filePathSettings);
		}
	}
}