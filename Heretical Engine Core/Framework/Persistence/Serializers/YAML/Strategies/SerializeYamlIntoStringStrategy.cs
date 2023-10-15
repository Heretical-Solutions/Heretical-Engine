using HereticalSolutions.Persistence.Arguments;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Implements the <see cref="IYamlSerializationStrategy"/> interface and provides functionality to serialize and deserialize YAML into a string.
    /// </summary>
    public class SerializeYamlIntoStringStrategy : IYamlSerializationStrategy
    {
        /// <summary>
        /// Serializes the specified YAML into a string.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="yaml">The YAML to be serialized.</param>
        /// <returns>true if serialization is successful; otherwise, false.</returns>
        public bool Serialize(ISerializationArgument argument, string yaml)
        {
            ((StringArgument)argument).Value = yaml;

            return true;
        }

        /// <summary>
        /// Deserializes the YAML from the specified serialization argument into a string.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="yaml">The deserialized YAML.</param>
        /// <returns>true if deserialization is successful; otherwise, false.</returns>
        public bool Deserialize(ISerializationArgument argument, out string yaml)
        {
            yaml = ((StringArgument)argument).Value;
            
            return true;
        }
        
        /// <summary>
        /// Erases the value of the specified serialization argument by setting it to an empty string.
        /// </summary>
        /// <param name="argument">The serialization argument to be erased.</param>
        public void Erase(ISerializationArgument argument)
        {
            ((StringArgument)argument).Value = string.Empty;
        }
    }
}