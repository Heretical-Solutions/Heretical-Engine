using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeJsonIntoStreamStrategy : IJsonSerializationStrategy
    {
        private readonly IFormatLogger logger;

        public SerializeJsonIntoStreamStrategy(IFormatLogger logger)
        {
            this.logger = logger;
        }

        public bool Serialize(
            ISerializationArgument argument,
            string json)
        {
            FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;

            if (!StreamIO.OpenWriteStream(
                filePathSettings,
                out StreamWriter streamWriter,
                logger))
                return false;

            streamWriter.Write(json);

            StreamIO.CloseStream(streamWriter);

            return true;
        }

        public bool Deserialize(
            ISerializationArgument argument,
            out string json)
        {
            FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;

            json = string.Empty;

            if (!StreamIO.OpenReadStream(
                filePathSettings,
                out StreamReader streamReader,
                logger))
                return false;

            json = streamReader.ReadToEnd();

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