using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeYamlIntoStreamStrategy : IYamlSerializationStrategy
    {
        private readonly IFormatLogger logger;

        public SerializeYamlIntoStreamStrategy(IFormatLogger logger)
        {
            this.logger = logger;
        }

        public bool Serialize(ISerializationArgument argument, string yaml)
        {
            FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;
            
            if (!StreamIO.OpenWriteStream(
                filePathSettings,
                out StreamWriter streamWriter,
                logger))
                return false;
            
            streamWriter.Write(yaml);
            
            StreamIO.CloseStream(streamWriter);

            return true;
        }

        public bool Deserialize(
            ISerializationArgument argument,
            out string yaml)
        {
            FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;
            
            yaml = string.Empty;
            
            if (!StreamIO.OpenReadStream(
                filePathSettings,
                out StreamReader streamReader,
                logger))
                return false;
            
            yaml = streamReader.ReadToEnd();
            
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