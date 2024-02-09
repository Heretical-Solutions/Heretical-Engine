using System;

namespace HereticalSolutions.Persistence
{
    /// <summary>
    /// Interface for serializing and deserializing objects
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes the provided data transfer object (DTO) using the specified argument
        /// </summary>
        /// <typeparam name="TValue">The type of the data transfer object.</typeparam>
        /// <param name="argument">The serialization argument to use.</param>
        /// <param name="DTO">The data transfer object to serialize.</param>
        /// <returns>`true` if the serialization was successful; otherwise, `false`.</returns>
        bool Serialize<TValue>(ISerializationArgument argument, TValue DTO);
        
        /// <summary>
        /// Serializes the provided data transfer object (DTO) of the specified type using the specified argument
        /// </summary>
        /// <param name="argument">The serialization argument to use.</param>
        /// <param name="DTOType">The type of the data transfer object.</param>
        /// <param name="DTO">The data transfer object to serialize.</param>
        /// <returns>`true` if the serialization was successful; otherwise, `false`.</returns>
        bool Serialize(ISerializationArgument argument, Type DTOType, object DTO);
        
        /// <summary>
        /// Deserializes a data transfer object (DTO) using the specified argument
        /// </summary>
        /// <typeparam name="TValue">The type of the data transfer object.</typeparam>
        /// <param name="argument">The serialization argument to use.</param>
        /// <param name="DTO">When this method returns, contains the deserialized data transfer object if the deserialization was successful; otherwise, the default value for the type.</param>
        /// <returns>`true` if the deserialization was successful; otherwise, `false`.</returns>
        bool Deserialize<TValue>(ISerializationArgument argument, out TValue DTO);

        /// <summary>
        /// Deserializes a data transfer object (DTO) of the specified type using the specified argument
        /// </summary>
        /// <param name="argument">The serialization argument to use.</param>
        /// <param name="DTOType">The type of the data transfer object.</param>
        /// <param name="DTO">When this method returns, contains the deserialized data transfer object if the deserialization was successful; otherwise, `null`.</param>
        /// <returns>`true` if the deserialization was successful; otherwise, `false`.</returns>
        bool Deserialize(ISerializationArgument argument, Type DTOType, out object DTO);
        
        /// <summary>
        /// Erases the serialized data using the specified argument
        /// </summary>
        /// <param name="argument">The serialization argument to use.</param>
        void Erase(ISerializationArgument argument);
    }
}