using System.IO;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Strategy for serializing YAML into a stream.
    /// </summary>
    public class SerializeYamlIntoStreamStrategy : IYamlSerializationStrategy
    {
        /// <summary>
        /// Serializes the YAML into a stream.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="yaml">The YAML to serialize.</param>
        /// <returns>True if the serialization is successful, otherwise false.</returns>
        public bool Serialize(ISerializationArgument argument, string yaml)
        {
            FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;
            
            if (!StreamIO.OpenWriteStream(filePathSettings, out StreamWriter streamWriter))
                return false;
            
            streamWriter.Write(yaml);
            
            StreamIO.CloseStream(streamWriter);

            return true;
        }

        /// <summary>
        /// Deserializes YAML from a stream.
        /// </summary>
        /// <param name="argument">The deserialization argument.</param>
        /// <param name="yaml">The deserialized YAML.</param>
        /// <returns>True if the deserialization is successful, otherwise false.</returns>
        public bool Deserialize(ISerializationArgument argument, out string yaml)
        {
            FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;
            
            yaml = string.Empty;
            
            if (!StreamIO.OpenReadStream(filePathSettings, out StreamReader streamReader))
                return false;
            
            yaml = streamReader.ReadToEnd();
            
            StreamIO.CloseStream(streamReader);

            return true;
        }

        /// <summary>
        /// Erases the serialized data.
        /// </summary>
        /// <param name="argument">The argument containing the data to erase.</param>
        public void Erase(ISerializationArgument argument)
        {
            FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;
            
            StreamIO.Erase(filePathSettings);
        }
    }
}