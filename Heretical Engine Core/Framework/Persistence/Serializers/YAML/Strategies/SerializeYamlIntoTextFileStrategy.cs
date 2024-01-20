using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeYamlIntoTextFileStrategy : IYamlSerializationStrategy
    {
        private readonly ILogger logger;

        public SerializeYamlIntoTextFileStrategy(
            ILogger logger = null)
        {
            this.logger = logger;
        }

        public bool Serialize(
            ISerializationArgument argument,
            string yaml)
        {
            FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;
            
            return TextFileIO.Write(
                filePathSettings,
                yaml,
                logger);
        }

        public bool Deserialize(
            ISerializationArgument argument,
            out string yaml)
        {
            FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;
            
            return TextFileIO.Read(
                filePathSettings,
                out yaml,
                logger);
        }
        
        public void Erase(ISerializationArgument argument)
        {
            FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;
            
            TextFileIO.Erase(filePathSettings);
        }
    }
}