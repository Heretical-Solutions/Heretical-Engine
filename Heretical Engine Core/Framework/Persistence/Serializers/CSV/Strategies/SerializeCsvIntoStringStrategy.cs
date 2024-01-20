using System.Collections;
using System.Globalization;

using HereticalSolutions.Persistence.Arguments;

using HereticalSolutions.Logging;

using CsvHelper;

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeCsvIntoStringStrategy : ICsvSerializationStrategy
    {
        private readonly ILogger logger;

        public SerializeCsvIntoStringStrategy(
            ILogger logger = null)
        {
            this.logger = logger;
        }

        public bool Serialize(
            ISerializationArgument argument,
            Type valueType,
            object value)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                using (var csvWriter = new CsvWriter(stringWriter, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteHeader(valueType);
                    
                    csvWriter.NextRecord();
                    
                    if (valueType.IsTypeGenericArray()
                        || valueType.IsTypeEnumerable()
                        || valueType.IsTypeGenericEnumerable())
                    {
                        csvWriter.WriteRecords((IEnumerable)value);
                    }
                    else
                        csvWriter.WriteRecord(value);
                }
                
                ((StringArgument)argument).Value = stringWriter.ToString();
            }
            
            return true;
        }

        public bool Deserialize(
            ISerializationArgument argument,
            Type valueType,
            out object value)
        {
            using (StringReader stringReader = new StringReader(((StringArgument)argument).Value))
            {
                using (var csvReader = new CsvReader(stringReader, CultureInfo.InvariantCulture))
                {
                    csvReader.Read();
                
                    csvReader.ReadHeader();
                    
                    if (valueType.IsTypeGenericArray()
                        || valueType.IsTypeEnumerable()
                        || valueType.IsTypeGenericEnumerable())
                    {
                        var underlyingType = (valueType.IsTypeGenericArray() || valueType.IsTypeEnumerable())
                            ? valueType.GetGenericArrayUnderlyingType()
                            : valueType.GetGenericEnumerableUnderlyingType();

                        var records = csvReader.GetRecords(underlyingType);

                        value = records;
                    }
                    else
                    {
                        csvReader.Read();   
                    
                        value = csvReader.GetRecord(valueType);
                    }
                }
            }
            
            return true;
        }
        

        public void Erase(ISerializationArgument argument)
        {
            ((StringArgument)argument).Value = string.Empty;
        }
    }
}