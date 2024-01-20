using System.Xml.Serialization;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeXmlIntoTextFileStrategy : IXmlSerializationStrategy
    {
        private readonly ILogger logger;

        public SerializeXmlIntoTextFileStrategy(
            ILogger logger = null)
        {
            this.logger = logger;
        }

        public bool Serialize(
            ISerializationArgument argument,
            XmlSerializer serializer,
            object value)
        {
            FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;

            string xml;
            
            using (StringWriter stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, value);
                
                xml = stringWriter.ToString();
            }
            
            return TextFileIO.Write(
                filePathSettings,
                xml,
                logger);
        }

        public bool Deserialize(
            ISerializationArgument argument,
            XmlSerializer serializer,
            out object value)
        {
            FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;

            bool result = TextFileIO.Read(
                filePathSettings,
                out string xml,
                logger);

            if (!result)
            {
                value = default;
                
                return false;
            }

            using (StringReader stringReader = new StringReader(xml))
            {
                value = serializer.Deserialize(stringReader);
            }

            return true;
        }
        
        public void Erase(ISerializationArgument argument)
        {
            FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;
            
            TextFileIO.Erase(filePathSettings);
        }
    }
}