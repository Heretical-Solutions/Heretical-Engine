using System;
using System.Runtime.Serialization.Formatters.Binary;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Class for serializing and deserializing objects using binary format.
    /// </summary>
    public class BinarySerializer : ISerializer
    {
        private readonly IReadOnlyObjectRepository strategyRepository;

        private readonly BinaryFormatter formatter = new BinaryFormatter();

        /// <summary>
        /// Initializes a new instance of the BinarySerializer class.
        /// </summary>
        /// <param name="strategyRepository">The repository containing the serialization strategies.</param>
        public BinarySerializer(IReadOnlyObjectRepository strategyRepository)
        {
            this.strategyRepository = strategyRepository;
        }

        #region ISerializer

        /// <summary>
        /// Serializes the given value using the specified serialization argument.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to serialize.</typeparam>
        /// <param name="argument">The serialization argument to use.</param>
        /// <param name="DTO">The value to serialize.</param>
        /// <returns>Returns true if the serialization was successful; otherwise, false.</returns>
        public bool Serialize<TValue>(ISerializationArgument argument, TValue DTO)
        {
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[BinarySerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IBinarySerializationStrategy)strategyObject;

            return concreteStrategy.Serialize(argument, formatter, DTO);
        }

        /// <summary>
        /// Serializes the given object using the specified serialization argument.
        /// </summary>
        /// <param name="argument">The serialization argument to use.</param>
        /// <param name="DTOType">The type of the object to serialize.</param>
        /// <param name="DTO">The object to serialize.</param>
        /// <returns>Returns true if the serialization was successful; otherwise, false.</returns>
        public bool Serialize(ISerializationArgument argument, Type DTOType, object DTO)
        {
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[BinarySerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IBinarySerializationStrategy)strategyObject;

            return concreteStrategy.Serialize(argument, formatter, DTO);
        }

        /// <summary>
        /// Deserializes the value of the specified type using the specified serialization argument.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to deserialize.</typeparam>
        /// <param name="argument">The serialization argument to use.</param>
        /// <param name="DTO">When this method returns, contains the deserialized value.</param>
        /// <returns>Returns true if the deserialization was successful; otherwise, false.</returns>
        public bool Deserialize<TValue>(ISerializationArgument argument, out TValue DTO)
        {
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[BinarySerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IBinarySerializationStrategy)strategyObject;

            var result = concreteStrategy.Deserialize(argument, formatter, out object dtoObject);

            DTO = (TValue)dtoObject;

            return result;
        }

        /// <summary>
        /// Deserializes an object of the specified type using the specified serialization argument.
        /// </summary>
        /// <param name="argument">The serialization argument to use.</param>
        /// <param name="DTOType">The type of the object to deserialize.</param>
        /// <param name="DTO">When this method returns, contains the deserialized object.</param>
        /// <returns>Returns true if the deserialization was successful; otherwise, false.</returns>
        public bool Deserialize(ISerializationArgument argument, Type DTOType, out object DTO)
        {
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[BinarySerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IBinarySerializationStrategy)strategyObject;

            return concreteStrategy.Deserialize(argument, formatter, out DTO);
        }

        /// <summary>
        /// Erases the serialized data associated with the specified serialization argument.
        /// </summary>
        /// <param name="argument">The serialization argument to use.</param>
        public void Erase(ISerializationArgument argument)
        {
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[BinarySerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IBinarySerializationStrategy)strategyObject;

            concreteStrategy.Erase(argument);
        }

        #endregion
    }
}