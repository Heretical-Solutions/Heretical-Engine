using System.Xml.Serialization;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Interface for XML serialization strategy
    /// </summary>
    public interface IXmlSerializationStrategy
    {
        /// <summary>
        /// Serializes the specified value using the provided XmlSerializer
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="serializer">The XmlSerializer to use.</param>
        /// <param name="value">The value to serialize.</param>
        /// <returns>True if serialization is successful, otherwise false.</returns>
        bool Serialize(ISerializationArgument argument, XmlSerializer serializer, object value);
        
        /// <summary>
        /// Deserializes the value using the provided XmlSerializer
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="serializer">The XmlSerializer to use.</param>
        /// <param name="value">The deserialized value.</param>
        /// <returns>True if deserialization is successful, otherwise false.</returns>
        bool Deserialize(ISerializationArgument argument, XmlSerializer serializer, out object value);

        /// <summary>
        /// Erases the serialized data from the provided serialization argument
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        void Erase(ISerializationArgument argument);
    }
}