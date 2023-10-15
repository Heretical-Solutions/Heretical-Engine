using System;
using HereticalSolutions.Persistence.Serializers;
using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Persistence.Factories
{
    /// <summary>
    /// A factory class for building different serializers.
    /// </summary>
    public static partial class PersistenceFactory
    {
        /// <summary>
        /// Builds a simple binary serializer.
        /// </summary>
        /// <returns>The built binary serializer.</returns>
        public static BinarySerializer BuildSimpleBinarySerializer()
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(typeof(StreamArgument), new SerializeBinaryIntoStreamStrategy());
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new BinarySerializer(strategyRepository);
        }
        
        /// <summary>
        /// Builds a simple protobuf serializer.
        /// </summary>
        /// <returns>The built protobuf serializer.</returns>
        public static ProtobufSerializer BuildSimpleProtobufSerializer()
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(typeof(StreamArgument), new SerializeProtobufIntoStreamStrategy());
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new ProtobufSerializer(strategyRepository);
        }
        
        /// <summary>
        /// Builds a simple JSON serializer.
        /// </summary>
        /// <returns>The built JSON serializer.</returns>
        public static JSONSerializer BuildSimpleJSONSerializer()
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(typeof(StringArgument), new SerializeJsonIntoStringStrategy());
            
            database.Add(typeof(StreamArgument), new SerializeJsonIntoStreamStrategy());
            database.Add(typeof(TextFileArgument), new SerializeJsonIntoTextFileStrategy());
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new JSONSerializer(strategyRepository);
        }

        /// <summary>
        /// Builds a simple XML serializer.
        /// </summary>
        /// <returns>The built XML serializer.</returns>
        public static XMLSerializer BuildSimpleXMLSerializer()
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(typeof(StringArgument), new SerializeXmlIntoStringStrategy());
            
            database.Add(typeof(StreamArgument), new SerializeXmlIntoStreamStrategy());
            database.Add(typeof(TextFileArgument), new SerializeXmlIntoTextFileStrategy());
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new XMLSerializer(strategyRepository);
        }
        
        /// <summary>
        /// Builds a simple YAML serializer.
        /// </summary>
        /// <returns>The built YAML serializer.</returns>
        public static YAMLSerializer BuildSimpleYAMLSerializer()
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(typeof(StringArgument), new SerializeYamlIntoStringStrategy());
            
            database.Add(typeof(StreamArgument), new SerializeYamlIntoStreamStrategy());
            database.Add(typeof(TextFileArgument), new SerializeYamlIntoTextFileStrategy());
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new YAMLSerializer(strategyRepository);
        }
        
        /// <summary>
        /// Builds a simple CSV serializer.
        /// </summary>
        /// <returns>The built CSV serializer.</returns>
        public static CSVSerializer BuildSimpleCSVSerializer()
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(typeof(StringArgument), new SerializeCsvIntoStringStrategy());
            
            database.Add(typeof(StreamArgument), new SerializeCsvIntoStreamStrategy());
            database.Add(typeof(TextFileArgument), new SerializeCsvIntoTextFileStrategy());
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new CSVSerializer(strategyRepository);
        }
    }
}