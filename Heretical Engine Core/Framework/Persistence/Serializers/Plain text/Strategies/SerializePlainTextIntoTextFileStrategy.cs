using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Serializers
{
	public class SerializePlainTextIntoTextFileStrategy : IPlainTextSerializationStrategy
	{
		private readonly ILogger logger;

		public SerializePlainTextIntoTextFileStrategy(
			ILogger logger = null)
		{
			this.logger = logger;
		}

		public bool Serialize(ISerializationArgument argument, string text)
		{
			FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;

			return TextFileIO.Write(
				filePathSettings,
				text,
				logger);
		}

		public bool Deserialize(ISerializationArgument argument, out string text)
		{
			FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;

			return TextFileIO.Read(
				filePathSettings,
				out text,
				logger);
		}

		public void Erase(ISerializationArgument argument)
		{
			FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;

			TextFileIO.Erase(filePathSettings);
		}
	}
}