using System.IO;
using System.Xml.Serialization;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Implements the IXmlSerializationStrategy interface to serialize and deserialize XML into a text file.
    /// </summary>
    public class SerializeXmlIntoTextFileStrategy : IXmlSerializationStrategy
    {
        /// <summary>
        /// Serializes the specified value using the provided serializer and writes it into a text file according to the argument settings.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="serializer">The XML serializer.</param>
        /// <param name="value">The value to be serialized.</param>
        /// <returns>True if the serialization succeeded; otherwise, false.</returns>
        public bool Serialize(ISerializationArgument argument, XmlSerializer serializer, object value)
        {
            FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;

            string xml;
            
            using (StringWriter stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, value);
                
                xml = stringWriter.ToString();
            }
            
            return TextFileIO.Write(filePathSettings, xml);
        }

        /// <summary>
        /// Deserializes XML data from a text file according to the argument settings and updates the value.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="serializer">The XML serializer.</param>
        /// <param name="value">When this method returns, contains the deserialized object. If the deserialization fails, contains null.</param>
        /// <returns>True if the deserialization succeeded; otherwise, false.</returns>
        public bool Deserialize(ISerializationArgument argument, XmlSerializer serializer, out object value)
        {
            FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;

            bool result = TextFileIO.Read(filePathSettings, out string xml);

            if (!result)
            {
                value = default(object);
                
                return false;
            }

            using (StringReader stringReader = new StringReader(xml))
            {
                value = serializer.Deserialize(stringReader);
            }

            return true;
        }
        
        /// <summary>
        /// Erases the contents of a text file according to the argument settings.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        public void Erase(ISerializationArgument argument)
        {
            FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;
            
            TextFileIO.Erase(filePathSettings);
        }
    }
}