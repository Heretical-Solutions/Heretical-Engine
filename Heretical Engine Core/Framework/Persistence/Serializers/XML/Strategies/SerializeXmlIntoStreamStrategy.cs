using System.IO;
using System.Xml.Serialization;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Strategy for serializing XML into a stream.
    /// </summary>
    public class SerializeXmlIntoStreamStrategy : IXmlSerializationStrategy
    {
        /// <summary>
        /// Serializes the specified value into a stream.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="serializer">The XML serializer.</param>
        /// <param name="value">The value to be serialized.</param>
        /// <returns>True if the serialization is successful, otherwise false.</returns>
        public bool Serialize(ISerializationArgument argument, XmlSerializer serializer, object value)
        {
            FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;
            
            if (!StreamIO.OpenWriteStream(fileSystemSettings, out StreamWriter streamWriter))
                return false;
            
            serializer.Serialize(streamWriter, value);
            
            StreamIO.CloseStream(streamWriter);

            return true;
        }

        /// <summary>
        /// Deserializes the object from a stream.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="serializer">The XML serializer.</param>
        /// <param name="value">The deserialized object.</param>
        /// <returns>True if the deserialization is successful, otherwise false.</returns>
        public bool Deserialize(ISerializationArgument argument, XmlSerializer serializer, out object value)
        {
            FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;

            if (!StreamIO.OpenReadStream(fileSystemSettings, out StreamReader streamReader))
            {
                value = default(object);
                
                return false;
            }

            value = serializer.Deserialize(streamReader);
            
            StreamIO.CloseStream(streamReader);

            return true;
        }

        /// <summary>
        /// Erases the serialized object from a stream.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        public void Erase(ISerializationArgument argument)
        {
            FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;
            
            StreamIO.Erase(fileSystemSettings);
        }
    }
}