using System.IO;
using System.Xml.Serialization;

using HereticalSolutions.Persistence.Arguments;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Represents a strategy for serializing objects to XML and storing the XML as a string.
    /// </summary>
    public class SerializeXmlIntoStringStrategy : IXmlSerializationStrategy
    {
        /// <summary>
        /// Serializes an object into XML and stores the XML as a string.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="serializer">The XML serializer.</param>
        /// <param name="value">The object to serialize.</param>
        /// <returns>True if the serialization was successful, otherwise false.</returns>
        public bool Serialize(ISerializationArgument argument, XmlSerializer serializer, object value)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, value);
                
                ((StringArgument)argument).Value = stringWriter.ToString();
            }
            
            return true;
        }

        /// <summary>
        /// Deserializes an object from the stored XML string.
        /// </summary>
        /// <param name="argument">The deserialization argument.</param>
        /// <param name="serializer">The XML serializer.</param>
        /// <param name="value">The deserialized object.</param>
        /// <returns>True if the deserialization was successful, otherwise false.</returns>
        public bool Deserialize(ISerializationArgument argument, XmlSerializer serializer, out object value)
        {
            using (StringReader stringReader = new StringReader(((StringArgument)argument).Value))
            {
                value = serializer.Deserialize(stringReader);
            }
            
            return true;
        }
        
        /// <summary>
        /// Erases the stored XML string.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        public void Erase(ISerializationArgument argument)
        {
            ((StringArgument)argument).Value = string.Empty;
        }
    }
}