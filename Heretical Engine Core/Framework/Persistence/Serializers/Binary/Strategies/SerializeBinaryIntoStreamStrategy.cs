using System.Runtime.Serialization.Formatters.Binary;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

using HereticalSolutions.Logging;

//BinaryFormatter.Serialize(Stream, object)' is obsolete: 'BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.
// Disable the warning.
#pragma warning disable SYSLIB0011

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeBinaryIntoStreamStrategy : IBinarySerializationStrategy
    {
        private readonly IFormatLogger logger;

        public SerializeBinaryIntoStreamStrategy(
            IFormatLogger logger = null)
        {
            this.logger = logger;
        }

        public bool Serialize(
            ISerializationArgument argument,
            BinaryFormatter formatter,
            object value)
        {
            FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;
            
            if (!StreamIO.OpenWriteStream(
                filePathSettings,
                out FileStream fileStream,
                logger))
                return false;
            
            formatter.Serialize(fileStream, value);
            
            StreamIO.CloseStream(fileStream);

            return true;
        }

        public bool Deserialize(
            ISerializationArgument argument,
            BinaryFormatter formatter,
            out object value)
        {
            FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;
            
            if (!StreamIO.OpenReadStream(
                filePathSettings,
                out FileStream fileStream,
                logger))
            {
                value = default;
                
                return false;
            }

            value = formatter.Deserialize(fileStream);
            
            StreamIO.CloseStream(fileStream);

            return true;
        }

        public void Erase(ISerializationArgument argument)
        {
            FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;
            
            StreamIO.Erase(filePathSettings);
        }
    }
}