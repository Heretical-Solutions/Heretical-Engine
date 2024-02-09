using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Represents a Protobuf serializer implementation of the <see cref="ISerializer"/> interface
    /// </summary>
    public class ProtobufSerializer : ISerializer
    {
        private readonly IReadOnlyObjectRepository strategyRepository;

        private readonly ILogger logger;

        public ProtobufSerializer(
            IReadOnlyObjectRepository strategyRepository,
            ILogger logger = null)
        {
            this.strategyRepository = strategyRepository;

            this.logger = logger;
        }

        #region ISerializer

        public bool Serialize<TValue>(
            ISerializationArgument argument,
            TValue DTO)
        {
            if (!strategyRepository.TryGet(
                argument.GetType(),
                out var strategyObject))
                throw new Exception(
                    logger.TryFormat<ProtobufSerializer>(
                        $"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}"));

            var concreteStrategy = (IProtobufSerializationStrategy)strategyObject;

            return concreteStrategy.Serialize(
                argument,
                typeof(TValue),
                DTO);
        }

        public bool Serialize(
            ISerializationArgument argument,
            Type DTOType,
            object DTO)
        {
            if (!strategyRepository.TryGet(
                argument.GetType(),
                out var strategyObject))
                throw new Exception(
                    logger.TryFormat<ProtobufSerializer>(
                        $"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}"));

            var concreteStrategy = (IProtobufSerializationStrategy)strategyObject;

            return concreteStrategy.Serialize(
                argument,
                DTOType,
                DTO);
        }

        public bool Deserialize<TValue>(
            ISerializationArgument argument,
            out TValue DTO)
        {
            if (!strategyRepository.TryGet(
                argument.GetType(),
                out var strategyObject))
                throw new Exception(
                    logger.TryFormat<ProtobufSerializer>(
                        $"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}"));

            var concreteStrategy = (IProtobufSerializationStrategy)strategyObject;

            var result = concreteStrategy.Deserialize(
                argument,
                typeof(TValue),
                out object dtoObject);

            DTO = (TValue)dtoObject;

            return result;
        }

        public bool Deserialize(
            ISerializationArgument argument,
            Type DTOType,
            out object DTO)
        {
            if (!strategyRepository.TryGet(
                argument.GetType(),
                out var strategyObject))
                throw new Exception(
                    logger.TryFormat<ProtobufSerializer>(
                        $"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}"));

            var concreteStrategy = (IProtobufSerializationStrategy)strategyObject;

            return concreteStrategy.Deserialize(
                argument,
                DTOType,
                out DTO);
        }

        public void Erase(ISerializationArgument argument)
        {
            if (!strategyRepository.TryGet(
                argument.GetType(),
                out var strategyObject))
                throw new Exception(
                    logger.TryFormat<ProtobufSerializer>(
                        $"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}"));

            var concreteStrategy = (IProtobufSerializationStrategy)strategyObject;

            concreteStrategy.Erase(argument);
        }

        #endregion
    }
}