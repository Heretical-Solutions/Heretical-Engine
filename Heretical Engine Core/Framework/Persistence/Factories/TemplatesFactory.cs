using HereticalSolutions.Persistence.Serializers;
using HereticalSolutions.Persistence.Arguments;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Factories
{
    public static partial class PersistenceFactory
    {
        public static BinarySerializer BuildSimpleBinarySerializer(
            ILoggerResolver loggerResolver = null)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();

            ILogger argumentLogger =
                loggerResolver?.GetLogger<SerializeBinaryIntoStreamStrategy>()
                ?? null;

            database.Add(
                typeof(StreamArgument),
                new SerializeBinaryIntoStreamStrategy(argumentLogger));
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            ILogger logger =
                loggerResolver?.GetLogger<BinarySerializer>()
                ?? null;

            return new BinarySerializer(
                strategyRepository,
                logger);
        }
        
        public static ProtobufSerializer BuildSimpleProtobufSerializer(
            ILoggerResolver loggerResolver = null)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            ILogger argumentLogger =
                loggerResolver?.GetLogger<SerializeProtobufIntoStreamStrategy>()
                ?? null;

            database.Add(
                typeof(StreamArgument),
                new SerializeProtobufIntoStreamStrategy(argumentLogger));
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            ILogger logger =
                loggerResolver?.GetLogger<ProtobufSerializer>()
                ?? null;

            return new ProtobufSerializer(
                strategyRepository,
                logger);
        }
        
        public static JSONSerializer BuildSimpleJSONSerializer(
            ILoggerResolver loggerResolver = null)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            ILogger stringArgumentLogger =
                loggerResolver?.GetLogger<SerializeJsonIntoStringStrategy>()
                ?? null;

            database.Add(
                typeof(StringArgument),
                new SerializeJsonIntoStringStrategy(stringArgumentLogger));
            
            ILogger streamArgumentLogger =
                loggerResolver?.GetLogger<SerializeJsonIntoStreamStrategy>()
                ?? null;

            database.Add(
                typeof(StreamArgument),
                new SerializeJsonIntoStreamStrategy(streamArgumentLogger));

            ILogger textFileArgumentLogger =
                loggerResolver?.GetLogger<SerializeJsonIntoTextFileStrategy>()
                ?? null;

            database.Add(
                typeof(TextFileArgument),
                new SerializeJsonIntoTextFileStrategy(textFileArgumentLogger));
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            ILogger logger =
                loggerResolver?.GetLogger<JSONSerializer>()
                ?? null;

            return new JSONSerializer(
                strategyRepository,
                logger);
        }

        public static XMLSerializer BuildSimpleXMLSerializer(
            ILoggerResolver loggerResolver = null)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            ILogger stringArgumentLogger =
                loggerResolver?.GetLogger<SerializeXmlIntoStringStrategy>()
                ?? null;

            database.Add(
                typeof(StringArgument),
                new SerializeXmlIntoStringStrategy(stringArgumentLogger));
            
            ILogger streamArgumentLogger =
                loggerResolver?.GetLogger<SerializeXmlIntoStreamStrategy>()
                ?? null;

            database.Add(
                typeof(StreamArgument),
                new SerializeXmlIntoStreamStrategy(streamArgumentLogger));

            ILogger textFileArgumentLogger =
                loggerResolver?.GetLogger<SerializeXmlIntoTextFileStrategy>()
                ?? null;

            database.Add(
                typeof(TextFileArgument),
                new SerializeXmlIntoTextFileStrategy(textFileArgumentLogger));
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            ILogger logger =
                loggerResolver?.GetLogger<XMLSerializer>()
                ?? null;

            return new XMLSerializer(
                strategyRepository,
                logger);
        }
        
        public static YAMLSerializer BuildSimpleYAMLSerializer(
            ILoggerResolver loggerResolver = null)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            ILogger stringArgumentLogger =
                loggerResolver?.GetLogger<SerializeYamlIntoStringStrategy>()
                ?? null;

            database.Add(
                typeof(StringArgument),
                new SerializeYamlIntoStringStrategy(stringArgumentLogger));
            
            ILogger streamArgumentLogger =
                loggerResolver?.GetLogger<SerializeYamlIntoStreamStrategy>()
                ?? null;

            database.Add(
                typeof(StreamArgument),
                new SerializeYamlIntoStreamStrategy(streamArgumentLogger));

            ILogger textFileArgumentLogger =
                loggerResolver?.GetLogger<SerializeYamlIntoTextFileStrategy>()
                ?? null;

            database.Add(
                typeof(TextFileArgument),
                new SerializeYamlIntoTextFileStrategy(textFileArgumentLogger));
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            ILogger logger =
                loggerResolver?.GetLogger<YAMLSerializer>()
                ?? null;

            return new YAMLSerializer(
                strategyRepository,
                logger);
        }
        
        public static CSVSerializer BuildSimpleCSVSerializer(
            ILoggerResolver loggerResolver = null)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            ILogger stringArgumentLogger =
                loggerResolver?.GetLogger<SerializeCsvIntoStringStrategy>()
                ?? null;

            database.Add(
                typeof(StringArgument),
                new SerializeCsvIntoStringStrategy(stringArgumentLogger));
            
            ILogger streamArgumentLogger =
                loggerResolver?.GetLogger<SerializeCsvIntoStreamStrategy>()
                ?? null;

            database.Add(
                typeof(StreamArgument),
                new SerializeCsvIntoStreamStrategy(streamArgumentLogger));

            ILogger textFileArgumentLogger =
                loggerResolver?.GetLogger<SerializeCsvIntoTextFileStrategy>()
                ?? null;

            database.Add(
                typeof(TextFileArgument),
                new SerializeCsvIntoTextFileStrategy(textFileArgumentLogger));
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);

            ILogger logger =
                loggerResolver?.GetLogger<CSVSerializer>()
                ?? null;

            return new CSVSerializer(
                strategyRepository,
                logger);
        }

        public static PlainTextSerializer BuildSimplePlainTextSerializer(
            ILoggerResolver loggerResolver = null)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();

            ILogger stringArgumentLogger =
                loggerResolver?.GetLogger<SerializePlainTextIntoStringStrategy>()
                ?? null;

            database.Add(
                typeof(StringArgument),
                new SerializePlainTextIntoStringStrategy(stringArgumentLogger));

            ILogger streamArgumentLogger =
                loggerResolver?.GetLogger<SerializePlainTextIntoStreamStrategy>()
                ?? null;

            database.Add(
                typeof(StreamArgument),
                new SerializePlainTextIntoStreamStrategy(streamArgumentLogger));

            ILogger textFileArgumentLogger =
                loggerResolver?.GetLogger<SerializePlainTextIntoTextFileStrategy>()
                ?? null;

            database.Add(
                typeof(TextFileArgument),
                new SerializePlainTextIntoTextFileStrategy(textFileArgumentLogger));

            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);

            ILogger logger =
                loggerResolver?.GetLogger<PlainTextSerializer>()
                ?? null;

            return new PlainTextSerializer(
                strategyRepository,
                logger);
        }
    }
}