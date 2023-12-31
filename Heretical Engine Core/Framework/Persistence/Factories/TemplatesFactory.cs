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
            IFormatLogger logger)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(
                typeof(StreamArgument),
                new SerializeBinaryIntoStreamStrategy(logger));
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new BinarySerializer(
                strategyRepository,
                logger);
        }
        
        public static ProtobufSerializer BuildSimpleProtobufSerializer(
            IFormatLogger logger)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(
                typeof(StreamArgument),
                new SerializeProtobufIntoStreamStrategy(logger));
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new ProtobufSerializer(
                strategyRepository,
                logger);
        }
        
        public static JSONSerializer BuildSimpleJSONSerializer(
            IFormatLogger logger)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(
                typeof(StringArgument),
                new SerializeJsonIntoStringStrategy(logger));
            
            database.Add(
                typeof(StreamArgument),
                new SerializeJsonIntoStreamStrategy(logger));
            database.Add(
                typeof(TextFileArgument),
                new SerializeJsonIntoTextFileStrategy(logger));
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new JSONSerializer(
                strategyRepository,
                logger);
        }

        public static XMLSerializer BuildSimpleXMLSerializer(
            IFormatLogger logger)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(
                typeof(StringArgument),
                new SerializeXmlIntoStringStrategy(logger));
            
            database.Add(
                typeof(StreamArgument),
                new SerializeXmlIntoStreamStrategy(logger));
            database.Add(
                typeof(TextFileArgument),
                new SerializeXmlIntoTextFileStrategy(logger));
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new XMLSerializer(
                strategyRepository,
                logger);
        }
        
        public static YAMLSerializer BuildSimpleYAMLSerializer(
            IFormatLogger logger)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(
                typeof(StringArgument),
                new SerializeYamlIntoStringStrategy(logger));
            
            database.Add(
                typeof(StreamArgument),
                new SerializeYamlIntoStreamStrategy(logger));
            database.Add(
                typeof(TextFileArgument),
                new SerializeYamlIntoTextFileStrategy(logger));
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new YAMLSerializer(
                strategyRepository,
                logger);
        }
        
        public static CSVSerializer BuildSimpleCSVSerializer(
            IFormatLogger logger)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(
                typeof(StringArgument),
                new SerializeCsvIntoStringStrategy(logger));
            
            database.Add(
                typeof(StreamArgument),
                new SerializeCsvIntoStreamStrategy(logger));
            database.Add(
                typeof(TextFileArgument),
                new SerializeCsvIntoTextFileStrategy(logger));
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new CSVSerializer(
                strategyRepository,
                logger);
        }

        public static PlainTextSerializer BuildSimplePlainTextSerializer(
            IFormatLogger logger)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();

            database.Add(
                typeof(StringArgument),
                new SerializePlainTextIntoStringStrategy(logger));
            database.Add(
                typeof(StreamArgument),
                new SerializePlainTextIntoStreamStrategy(logger));
            database.Add(
                typeof(TextFileArgument),
                new SerializePlainTextIntoTextFileStrategy(logger));

            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);

            return new PlainTextSerializer(
                strategyRepository,
                logger);
        }
    }
}