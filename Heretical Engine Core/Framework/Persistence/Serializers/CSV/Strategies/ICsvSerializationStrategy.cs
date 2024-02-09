using System;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// The interface for CSV serialization strategy
    /// </summary>
    public interface ICsvSerializationStrategy
    {
        /// <summary>
        /// Serializes the specified value using the provided serialization argument
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="valueType">The type of the value to be serialized.</param>
        /// <param name="value">The value to be serialized.</param>
        /// <returns><c>true</c> if the serialization is successful, otherwise <c>false</c>.</returns>
        bool Serialize(ISerializationArgument argument, Type valueType, object value);
        
        /// <summary>
        /// Deserializes the value using the provided serialization argument
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="valueType">The type of the value to be deserialized.</param>
        /// <param name="value">The deserialized value.</param>
        /// <returns><c>true</c> if the deserialization is successful, otherwise <c>false</c>.</returns>
        bool Deserialize(ISerializationArgument argument, Type valueType, out object value);

        /// <summary>
        /// Erases the value using the provided serialization argument
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        void Erase(ISerializationArgument argument);
    }
}