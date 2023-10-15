using System;
using HereticalSolutions.Repositories;
using YamlDotNet.Serialization;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Represents a YAML serializer that implements the ISerializer interface.
    /// </summary>
    public class YAMLSerializer : ISerializer
    {
        /// <summary>
        /// The YAML serializer object.
        /// </summary>
        private readonly YamlDotNet.Serialization.ISerializer yamlSerializer;

        /// <summary>
        /// The YAML deserializer object.
        /// </summary>
        private readonly YamlDotNet.Serialization.IDeserializer yamlDeserializer;

        /// <summary>
        /// The repository of strategies for object serialization.
        /// </summary>
        private readonly IReadOnlyObjectRepository strategyRepository;

        /// <summary>
        /// Initializes a new instance of the YAMLSerializer class with the specified strategy repository.
        /// </summary>
        /// <param name="strategyRepository">The strategy repository to use for object serialization.</param>
        public YAMLSerializer(IReadOnlyObjectRepository strategyRepository)
        {
            yamlSerializer = new SerializerBuilder().Build();
            yamlDeserializer = new DeserializerBuilder().Build();
            this.strategyRepository = strategyRepository;
        }

        #region ISerializer
        
        /// <summary>
        /// Serializes the specified DTO using the provided serialization argument.
        /// </summary>
        /// <typeparam name="TValue">The type of the DTO.</typeparam>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="DTO">The DTO to serialize.</param>
        /// <returns>True if the serialization is successful; otherwise, false.</returns>
        public bool Serialize<TValue>(ISerializationArgument argument, TValue DTO)
        {
            string yaml = yamlSerializer.Serialize(DTO);
            
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[YAMLSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IYamlSerializationStrategy)strategyObject;

            return concreteStrategy.Serialize(argument, yaml);
        }

        /// <summary>
        /// Serializes the specified DTO object using the provided serialization argument.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="DTOType">The type of the DTO object.</param>
        /// <param name="DTO">The DTO object to serialize.</param>
        /// <returns>True if the serialization is successful; otherwise, false.</returns>
        public bool Serialize(ISerializationArgument argument, Type DTOType, object DTO)
        {
            string yaml = yamlSerializer.Serialize(DTO);
            
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[YAMLSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IYamlSerializationStrategy)strategyObject;

            return concreteStrategy.Serialize(argument, yaml);
        }

        /// <summary>
        /// Deserializes the specified DTO using the provided serialization argument.
        /// </summary>
        /// <typeparam name="TValue">The type of the DTO.</typeparam>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="DTO">When this method returns, contains the deserialized DTO if the deserialization is successful; otherwise, the default value for the type of the DTO.</param>
        /// <returns>True if the deserialization is successful; otherwise, false.</returns>
        public bool Deserialize<TValue>(ISerializationArgument argument, out TValue DTO)
        {
            DTO = (TValue)Activator.CreateInstance(typeof(TValue));
            
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[YAMLSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IYamlSerializationStrategy)strategyObject;

            if (!concreteStrategy.Deserialize(argument, out var yaml))
                return false;

            DTO = yamlDeserializer.Deserialize<TValue>(yaml);
            
            return true;
        }

        /// <summary>
        /// Deserializes the specified DTO object using the provided serialization argument.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="DTOType">The type of the DTO object.</param>
        /// <param name="DTO">When this method returns, contains the deserialized DTO object if the deserialization is successful; otherwise, null.</param>
        /// <returns>True if the deserialization is successful; otherwise, false.</returns>
        public bool Deserialize(ISerializationArgument argument, Type DTOType, out object DTO)
        {
            DTO = Activator.CreateInstance(DTOType);
            
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[YAMLSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IYamlSerializationStrategy)strategyObject;

            if (!concreteStrategy.Deserialize(argument, out var yaml))
                return false;

            DTO = yamlDeserializer.Deserialize(yaml, DTOType);

            return true;
        }

        /// <summary>
        /// Erases the serialized data identified by the provided serialization argument.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        public void Erase(ISerializationArgument argument)
        {
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[YAMLSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IYamlSerializationStrategy)strategyObject;
            
            concreteStrategy.Erase(argument);
        }
        
        #endregion
    }
}