using System.Runtime.Serialization.Formatters.Binary;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Interface for binary serialization strategy
    /// </summary>
    public interface IBinarySerializationStrategy
    {
        /// <summary>
        /// Serializes the specified value using the provided serializer
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="formatter">The binary formatter used for serialization.</param>
        /// <param name="value">The value to be serialized.</param>
        /// <returns>A boolean indicating whether the serialization was successful or not.</returns>
        bool Serialize(ISerializationArgument argument, BinaryFormatter formatter, object value);

        /// <summary>
        /// Deserializes the value using the provided serializer
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="formatter">The binary formatter used for deserialization.</param>
        /// <param name="value">The deserialized value.</param>
        /// <returns>A boolean indicating whether the deserialization was successful or not.</returns>
        bool Deserialize(ISerializationArgument argument, BinaryFormatter formatter, out object value);

        /// <summary>
        /// Erases the serialized data related to the provided serialization argument
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        void Erase(ISerializationArgument argument);
    }
}