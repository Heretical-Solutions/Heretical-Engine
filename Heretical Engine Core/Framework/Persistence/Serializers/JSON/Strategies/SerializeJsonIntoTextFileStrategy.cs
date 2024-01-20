using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeJsonIntoTextFileStrategy : IJsonSerializationStrategy
    {
        private readonly ILogger logger;

        public SerializeJsonIntoTextFileStrategy(
            ILogger logger = null)
        {
            this.logger = logger;
        }

        public bool Serialize(
            ISerializationArgument argument,
            string json)
        {
            FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;
            
            return TextFileIO.Write(
                filePathSettings,
                json,
                logger);
        }

        public bool Deserialize(
            ISerializationArgument argument,
            out string json)
        {
            FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;
            
            return TextFileIO.Read(
                filePathSettings,
                out json,
                logger);
        }
        
        public void Erase(ISerializationArgument argument)
        {
            FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;
            
            TextFileIO.Erase(filePathSettings);
        }
    }
}