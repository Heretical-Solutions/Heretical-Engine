using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

using Newtonsoft.Json;

namespace HereticalSolutions.Persistence.Serializers
{
    public class JSONSerializer : ISerializer
    {
        private readonly JsonSerializerSettings writeSerializerSettings;

        private readonly JsonSerializerSettings readSerializerSettings;

        private readonly IReadOnlyObjectRepository strategyRepository;

        private readonly ILogger logger;

        public JSONSerializer(
            IReadOnlyObjectRepository strategyRepository,
            ILogger logger = null)
        {
            writeSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                //TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple, //COMMENTED OUT BECAUSE THIS OPTION IS NOT PRESENT IN JSON.NET.AOT
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                //NullValueHandling = NullValueHandling.Ignore,
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
            };

            writeSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            readSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                //NullValueHandling = NullValueHandling.Ignore,
                //TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple, //COMMENTED OUT BECAUSE THIS OPTION IS NOT PRESENT IN JSON.NET.AOT
                MaxDepth = 10
            };

            this.strategyRepository = strategyRepository;

            this.logger = logger;
        }

        #region ISerializer

        public bool Serialize<TValue>(ISerializationArgument argument, TValue DTO)
        {
            string json = JsonConvert.SerializeObject(
                DTO,
                Formatting.Indented,
                writeSerializerSettings);

            if (!strategyRepository.TryGet(
                argument.GetType(),
                out var strategyObject))
                throw new Exception(
                    logger.TryFormat<JSONSerializer>(
                        $"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}"));

            var concreteStrategy = (IJsonSerializationStrategy)strategyObject;

            return concreteStrategy.Serialize(argument, json);
        }

        public bool Serialize(
            ISerializationArgument argument,
            Type DTOType,
            object DTO)
        {
            string json = JsonConvert.SerializeObject(
                DTO,
                Formatting.Indented,
                writeSerializerSettings);

            if (!strategyRepository.TryGet(
                argument.GetType(),
                out var strategyObject))
                throw new Exception(
                    logger.TryFormat<JSONSerializer>(
                        $"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}"));

            var concreteStrategy = (IJsonSerializationStrategy)strategyObject;

            return concreteStrategy.Serialize(argument, json);
        }

        //TODO: fix deserialize as <GENERIC> is NOT working at all - the value does NOT get populated properly
        //WARNING: THIS ONE MAY NOT PROPERLY POPULATE. I HAVE NO IDEA WHY
        /// <summary>
        /// Deserializes a DTO using the given argument and returns the result
        /// </summary>
        /// <typeparam name="TValue">The type of the DTO.</typeparam>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="DTO">When this method returns, contains the deserialized DTO.</param>
        /// <returns>true if deserialization succeeds, false otherwise.</returns>
        public bool Deserialize<TValue>(ISerializationArgument argument, out TValue DTO)
        {
            DTO = (TValue)Activator.CreateInstance(typeof(TValue));

            if (!strategyRepository.TryGet(
                argument.GetType(),
                out var strategyObject))
                throw new Exception(
                    logger.TryFormat<JSONSerializer>(
                        $"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}"));

            var concreteStrategy = (IJsonSerializationStrategy)strategyObject;

            if (!concreteStrategy.Deserialize(
                argument,
                out var json))
                return false;

            JsonConvert.PopulateObject(
                json,
                DTO,
                readSerializerSettings);

            return true;
        }

        //TODO: fix deserialize as <GENERIC> is NOT working at all - the value does NOT get populated properly
        /// <summary>
        /// Deserializes a DTO object using the given argument and returns the result
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="DTOType">The type of the DTO.</param>
        /// <param name="DTO">When this method returns, contains the deserialized DTO object.</param>
        /// <returns>true if deserialization succeeds, false otherwise.</returns>
        public bool Deserialize(ISerializationArgument argument, Type DTOType, out object DTO)
        {
            DTO = Activator.CreateInstance(DTOType);

            if (!strategyRepository.TryGet(
                argument.GetType(),
                out var strategyObject))
                throw new Exception(
                    logger.TryFormat<JSONSerializer>(
                        $"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}"));

            var concreteStrategy = (IJsonSerializationStrategy)strategyObject;

            if (!concreteStrategy.Deserialize(
                argument,
                out var json))
                return false;

            JsonConvert.PopulateObject(
                json,
                DTO,
                readSerializerSettings);

            return true;
        }

        public void Erase(ISerializationArgument argument)
        {
            if (!strategyRepository.TryGet(
                argument.GetType(),
                out var strategyObject))
                throw new Exception(
                    logger.TryFormat<JSONSerializer>(
                        $"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}"));

            var concreteStrategy = (IJsonSerializationStrategy)strategyObject;

            concreteStrategy.Erase(argument);
        }

        #endregion
    }
}