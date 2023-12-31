using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Serializers
{
	public class SerializePlainTextIntoStreamStrategy : IPlainTextSerializationStrategy
	{
		private readonly IFormatLogger logger;

		public SerializePlainTextIntoStreamStrategy(IFormatLogger logger)
		{
			this.logger = logger;
		}

		public bool Serialize(
			ISerializationArgument argument,
			string text)
		{
			FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;

			if (!StreamIO.OpenWriteStream(
				filePathSettings,
				out StreamWriter streamWriter,
				logger))
				return false;

			streamWriter.Write(text);

			StreamIO.CloseStream(streamWriter);

			return true;
		}

		public bool Deserialize(
			ISerializationArgument argument,
			out string text)
		{
			FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;

			text = string.Empty;

			if (!StreamIO.OpenReadStream(
				filePathSettings,
				out StreamReader streamReader,
				logger))
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