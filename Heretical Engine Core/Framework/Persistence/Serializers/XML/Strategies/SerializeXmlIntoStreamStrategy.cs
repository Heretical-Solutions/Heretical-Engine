using System.Xml.Serialization;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeXmlIntoStreamStrategy : IXmlSerializationStrategy
    {
        private readonly IFormatLogger logger;

        public SerializeXmlIntoStreamStrategy(
            IFormatLogger logger = null)
        {
            this.logger = logger;
        }

        public bool Serialize(
            ISerializationArgument argument,
            XmlSerializer serializer,
            object value)
        {
            FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;
            
            if (!StreamIO.OpenWriteStream(
                filePathSettings,
                out StreamWriter streamWriter,
                logger))
                return false;
            
            serializer.Serialize(streamWriter, value);
            
            StreamIO.CloseStream(streamWriter);

            return true;
        }

        public bool Deserialize(
            ISerializationArgument argument,
            XmlSerializer serializer,
            out object value)
        {
            FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;

            if (!StreamIO.OpenReadStream(
                filePathSettings,
                out StreamReader streamReader,
                logger))
            {
                value = default;
                
                return false;
            }

            value = serializer.Deserialize(streamReader);
            
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