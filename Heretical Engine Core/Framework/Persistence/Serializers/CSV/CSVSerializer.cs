using System;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Represents a serializer that serializes and deserializes objects to and from CSV format.
    /// </summary>
    public class CSVSerializer : ISerializer
    {
        private readonly IReadOnlyObjectRepository strategyRepository;

        /// <summary>
        /// Initializes a new instance of the CSVSerializer class.
        /// </summary>
        /// <param name="strategyRepository">The repository that contains the serialization strategies.</param>
        public CSVSerializer(IReadOnlyObjectRepository strategyRepository)
        {
            this.strategyRepository = strategyRepository;
        }

        #region ISerializer

        /// <summary>
        /// Serializes an object of type TValue to CSV format.
        /// </summary>
        /// <typeparam name="TValue">The type of the object to serialize.</typeparam>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="DTO">The object to serialize.</param>
        /// <returns>True if the serialization was successful, otherwise false.</returns>
        public bool Serialize<TValue>(ISerializationArgument argument, TValue DTO)
        {
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[CSVSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (ICsvSerializationStrategy)strategyObject;

            return concreteStrategy.Serialize(argument, typeof(TValue), DTO);
        }

        /// <summary>
        /// Serializes an object of type DTOType to CSV format.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="DTOType">The type of the object to serialize.</param>
        /// <param name="DTO">The object to serialize.</param>
        /// <returns>True if the serialization was successful, otherwise false.</returns>
        public bool Serialize(ISerializationArgument argument, Type DTOType, object DTO)
        {
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[CSVSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (ICsvSerializationStrategy)strategyObject;

            return concreteStrategy.Serialize(argument, DTOType, DTO);
        }

        /// <summary>
        /// Deserializes an object of type TValue from CSV format.
        /// </summary>
        /// <typeparam name="TValue">The type of the object to deserialize.</typeparam>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="DTO">The deserialized object.</param>
        /// <returns>True if the deserialization was successful, otherwise false.</returns>
        public bool Deserialize<TValue>(ISerializationArgument argument, out TValue DTO)
        {
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[CSVSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (ICsvSerializationStrategy)strategyObject;

            var result = concreteStrategy.Deserialize(argument, typeof(TValue), out object dtoObject);

            DTO = (TValue)dtoObject;

            return result;
        }

        /// <summary>
        /// Deserializes an object of type DTOType from CSV format.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="DTOType">The type of the object to deserialize.</param>
        /// <param name="DTO">The deserialized object.</param>
        /// <returns>True if the deserialization was successful, otherwise false.</returns>
        public bool Deserialize(ISerializationArgument argument, Type DTOType, out object DTO)
        {
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[CSVSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (ICsvSerializationStrategy)strategyObject;

            return concreteStrategy.Deserialize(argument, DTOType, out DTO);
        }

        /// <summary>
        /// Erases the serialized data specified by the serialization argument.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        public void Erase(ISerializationArgument argument)
        {
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[CSVSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (ICsvSerializationStrategy)strategyObject;

            concreteStrategy.Erase(argument);
        }

        #endregion
    }
}