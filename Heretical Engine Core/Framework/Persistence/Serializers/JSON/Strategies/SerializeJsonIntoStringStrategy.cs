using HereticalSolutions.Persistence.Arguments;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Represents a strategy for serializing JSON into a string.
    /// </summary>
    public class SerializeJsonIntoStringStrategy : IJsonSerializationStrategy
    {
        /// <summary>
        /// Serializes JSON into a string.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="json">The JSON to be serialized.</param>
        /// <returns>True if the serialization was successful; otherwise, false.</returns>
        public bool Serialize(ISerializationArgument argument, string json)
        {
            ((StringArgument)argument).Value = json;

            return true;
        }

        /// <summary>
        /// Deserializes a string into JSON.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="json">The deserialized JSON.</param>
        /// <returns>True if the deserialization was successful; otherwise, false.</returns>
        public bool Deserialize(ISerializationArgument argument, out string json)
        {
            json = ((StringArgument)argument).Value;
            
            return true;
        }
        
        /// <summary>
        /// Erases the serialized string.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        public void Erase(ISerializationArgument argument)
        {
            ((StringArgument)argument).Value = string.Empty;
        }
    }
}