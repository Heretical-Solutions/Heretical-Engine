using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Serializers
{
    public class CSVSerializer : ISerializer
    {
        private readonly IReadOnlyObjectRepository strategyRepository;

        private readonly ILogger logger;

        public CSVSerializer(
            IReadOnlyObjectRepository strategyRepository,
            ILogger logger = null)
        {
            this.strategyRepository = strategyRepository;

            this.logger = logger;
        }

        #region ISerializer

        public bool Serialize<TValue>(ISerializationArgument argument, TValue DTO)
        {
            if (!strategyRepository.TryGet(
                argument.GetType(),
                out var strategyObject))
                throw new Exception(
                    logger.TryFormat<CSVSerializer>(
                        $"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}"));

            var concreteStrategy = (ICsvSerializationStrategy)strategyObject;

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
            if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
                throw new Exception(
                    logger.TryFormat<CSVSerializer>(
                        $"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}"));

            var concreteStrategy = (ICsvSerializationStrategy)strategyObject;

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
                    logger.TryFormat<CSVSerializer>(
                        $"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}"));

            var concreteStrategy = (ICsvSerializationStrategy)strategyObject;

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
                    logger.TryFormat<CSVSerializer>(
                        $"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}"));

            var concreteStrategy = (ICsvSerializationStrategy)strategyObject;

            return concreteStrategy.Deserialize(argument, DTOType, out DTO);
        }

        public void Erase(ISerializationArgument argument)
        {
            if (!strategyRepository.TryGet(
                argument.GetType(),
                out var strategyObject))
                throw new Exception(
                    logger.TryFormat<CSVSerializer>(
                        $"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}"));

            var concreteStrategy = (ICsvSerializationStrategy)strategyObject;

            concreteStrategy.Erase(argument);
        }

        #endregion
    }
}