using System;
using HereticalSolutions.Repositories;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Represents a Protobuf serializer implementation of the <see cref="ISerializer"/> interface.
    /// </summary>
    public class ProtobufSerializer : ISerializer
    {
        private readonly IReadOnlyObjectRepository strategyRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtobufSerializer"/> class with the specified strategy repository.
        /// </summary>
        /// <param name="strategyRepository">The strategy repository to use.</param>
        public ProtobufSerializer(IReadOnlyObjectRepository strategyRepository)
        {
            this.strategyRepository = strategyRepository;
        }

        #region ISerializer

        /// <summary>
        /// Serializes the specified value using the provided serialization argument.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to serialize.</typeparam>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="DTO">The value to serialize.</param>
        /// <returns><c>true</c> if the serialization is successful; otherwise, <c>false</c>.</returns>
        /// <exception cref="Exception">Thrown when the strategy for the specified argument type cannot be resolved.</exception>
        public bool Serialize<TValue>(ISerializationArgument argument, TValue DTO)
        {
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[ProtobufSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IProtobufSerializationStrategy)strategyObject;

            return concreteStrategy.Serialize(argument, typeof(TValue), DTO);
        }

        /// <summary>
        /// Serializes the specified value of the specified type using the provided serialization argument.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="DTOType">The type of the value to serialize.</param>
        /// <param name="DTO">The value to serialize.</param>
        /// <returns><c>true</c> if the serialization is successful; otherwise, <c>false</c>.</returns>
        /// <exception cref="Exception">Thrown when the strategy for the specified argument type cannot be resolved.</exception>
        public bool Serialize(ISerializationArgument argument, Type DTOType, object DTO)
        {
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[ProtobufSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IProtobufSerializationStrategy)strategyObject;

            return concreteStrategy.Serialize(argument, DTOType, DTO);
        }

        /// <summary>
        /// Deserializes a value of the specified type using the provided serialization argument.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to deserialize.</typeparam>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="DTO">The deserialized value.</param>
        /// <returns><c>true</c> if the deserialization is successful; otherwise, <c>false</c>.</returns>
        /// <exception cref="Exception">Thrown when the strategy for the specified argument type cannot be resolved.</exception>
        public bool Deserialize<TValue>(ISerializationArgument argument, out TValue DTO)
        {
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[ProtobufSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IProtobufSerializationStrategy)strategyObject;

            var result = concreteStrategy.Deserialize(argument, typeof(TValue), out object dtoObject);

            DTO = (TValue)dtoObject;

            return result;
        }

        /// <summary>
        /// Deserializes a value of the specified type using the provided serialization argument.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="DTOType">The type of the value to deserialize.</param>
        /// <param name="DTO">The deserialized value.</param>
        /// <returns><c>true</c> if the deserialization is successful; otherwise, <c>false</c>.</returns>
        /// <exception cref="Exception">Thrown when the strategy for the specified argument type cannot be resolved.</exception>
        public bool Deserialize(ISerializationArgument argument, Type DTOType, out object DTO)
        {
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[ProtobufSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IProtobufSerializationStrategy)strategyObject;

            return concreteStrategy.Deserialize(argument, DTOType, out DTO);
        }

        /// <summary>
        /// Erases the serialized data specified by the serialization argument.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <exception cref="Exception">Thrown when the strategy for the specified argument type cannot be resolved.</exception>
        public void Erase(ISerializationArgument argument)
        {
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[ProtobufSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IProtobufSerializationStrategy)strategyObject;

            concreteStrategy.Erase(argument);
        }

        #endregion
    }
}