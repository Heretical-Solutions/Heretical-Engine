namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Interface for a YAML serialization strategy
    /// </summary>
    public interface IYamlSerializationStrategy
    {
        /// <summary>
        /// Serializes the specified YAML string using the given serialization argument
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="yaml">The YAML string to serialize.</param>
        /// <returns><c>true</c> if the serialization was successful; otherwise, <c>false</c>.</returns>
        bool Serialize(ISerializationArgument argument, string yaml);
        
        /// <summary>
        /// Deserializes the YAML string using the given serialization argument
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="yaml">The deserialized YAML string.</param>
        /// <returns><c>true</c> if the deserialization was successful; otherwise, <c>false</c>.</returns>
        bool Deserialize(ISerializationArgument argument, out string yaml);

        /// <summary>
        /// Erases any serialized data associated with the given serialization argument
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        void Erase(ISerializationArgument argument);
    }
}