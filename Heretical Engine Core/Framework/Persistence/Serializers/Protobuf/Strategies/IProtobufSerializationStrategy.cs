using System;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Interface for Protobuf serialization strategy
    /// </summary>
    public interface IProtobufSerializationStrategy
    {
        /// <summary>
        /// Serializes the specified value using protobuf
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="valueType">The type of the value being serialized.</param>
        /// <param name="value">The value to serialize.</param>
        /// <returns><c>true</c> if the serialization is successful; otherwise, <c>false</c>.</returns>
        bool Serialize(ISerializationArgument argument, Type valueType, object value);

        /// <summary>
        /// Deserializes the value using protobuf
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="valueType">The type of the value being deserialized.</param>
        /// <param name="value">The deserialized value.</param>
        /// <returns><c>true</c> if the deserialization is successful; otherwise, <c>false</c>.</returns>
        bool Deserialize(ISerializationArgument argument, Type valueType, out object value);

        /// <summary>
        /// Erases the serialized data
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        void Erase(ISerializationArgument argument);
    }
}