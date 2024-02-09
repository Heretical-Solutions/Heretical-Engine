namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Represents a strategy for serializing JSON data
    /// </summary>
    public interface IJsonSerializationStrategy
    {
        /// <summary>
        /// Serializes the JSON data to the specified argument
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="json">The JSON string to be serialized.</param>
        /// <returns><see langword="true"/> if the serialization was successful; otherwise, <see langword="false"/>.</returns>
        bool Serialize(ISerializationArgument argument, string json);
        
        /// <summary>
        /// Deserializes the JSON data from the specified argument
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="json">The deserialized JSON string.</param>
        /// <returns><see langword="true"/> if the deserialization was successful; otherwise, <see langword="false"/>.</returns>
        bool Deserialize(ISerializationArgument argument, out string json);

        /// <summary>
        /// Erases the serialized data from the specified argument
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        void Erase(ISerializationArgument argument);
    }
}