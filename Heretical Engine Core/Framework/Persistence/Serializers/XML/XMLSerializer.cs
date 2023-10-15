using System;
using System.Xml.Serialization;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Represents a serializer that uses XML format for serialization and deserialization.
    /// </summary>
    public class XMLSerializer : ISerializer
    {
        private readonly IReadOnlyObjectRepository strategyRepository;
        
        /// <summary>
        /// Initializes a new instance of the XMLSerializer class with the specified strategy repository.
        /// </summary>
        /// <param name="strategyRepository">The repository that contains the serialization strategies.</param>
        public XMLSerializer(IReadOnlyObjectRepository strategyRepository)
        {
            this.strategyRepository = strategyRepository;
        }
        
        #region ISerializer
        
        /// <summary>
        /// Serializes the specified data transfer object (DTO) to XML format using the provided serialization argument.
        /// </summary>
        /// <typeparam name="TValue">The type of the DTO.</typeparam>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="DTO">The DTO to serialize.</param>
        /// <returns>True if the serialization is successful, otherwise false.</returns>
        public bool Serialize<TValue>(ISerializationArgument argument, TValue DTO)
        {
            var serializer = new XmlSerializer(typeof(TValue));
            
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[XMLSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IXmlSerializationStrategy)strategyObject;

            return concreteStrategy.Serialize(argument, serializer, DTO);
        }

        /// <summary>
        /// Serializes the specified data transfer object (DTO) to XML format using the provided serialization argument and data transfer object type.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="DTOType">The type of the DTO.</param>
        /// <param name="DTO">The DTO to serialize.</param>
        /// <returns>True if the serialization is successful, otherwise false.</returns>
        public bool Serialize(ISerializationArgument argument, Type DTOType, object DTO)
        {
            var serializer = new XmlSerializer(DTOType);
            
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[XMLSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IXmlSerializationStrategy)strategyObject;

            return concreteStrategy.Serialize(argument, serializer, DTO);
        }

        /// <summary>
        /// Deserializes the XML string contained in the serialization argument to the specified DTO type using the provided serialization argument.
        /// </summary>
        /// <typeparam name="TValue">The type of the DTO.</typeparam>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="DTO">The deserialized DTO.</param>
        /// <returns>True if the deserialization is successful, otherwise false.</returns>
        public bool Deserialize<TValue>(ISerializationArgument argument, out TValue DTO)
        {
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[XMLSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IXmlSerializationStrategy)strategyObject;

            var serializer = new XmlSerializer(typeof(TValue));
            
            var result = concreteStrategy.Deserialize(argument, serializer, out object dtoObject);

            DTO = (TValue)dtoObject;

            return result;
        }

        /// <summary>
        /// Deserializes the XML string contained in the serialization argument to the specified DTO type using the provided serialization argument and DTO type.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="DTOType">The type of the DTO.</param>
        /// <param name="DTO">The deserialized DTO.</param>
        /// <returns>True if the deserialization is successful, otherwise false.</returns>
        public bool Deserialize(ISerializationArgument argument, Type DTOType, out object DTO)
        {
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[XMLSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IXmlSerializationStrategy)strategyObject;

            var serializer = new XmlSerializer(DTOType);
            
            return concreteStrategy.Deserialize(argument, serializer, out DTO);
        }

        /// <summary>
        /// Erases any serialized data associated with the serialization argument.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        public void Erase(ISerializationArgument argument)
        {
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception($"[XMLSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

            var concreteStrategy = (IXmlSerializationStrategy)strategyObject;
			
            concreteStrategy.Erase(argument);
        }
        
        #endregion
    }
}