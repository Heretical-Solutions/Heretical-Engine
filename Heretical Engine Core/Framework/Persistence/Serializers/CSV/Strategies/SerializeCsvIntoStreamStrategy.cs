using System.Collections;
using System.Globalization;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

using HereticalSolutions.Logging;

using CsvHelper;

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeCsvIntoStreamStrategy : ICsvSerializationStrategy
    {
        private readonly ILogger logger;

        public SerializeCsvIntoStreamStrategy(
            ILogger logger = null)
        {
            this.logger = logger;
        }

        public bool Serialize(
            ISerializationArgument argument,
            Type valueType,
            object value)
        {
            FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;
            
            if (!StreamIO.OpenWriteStream(
                filePathSettings,
                out StreamWriter streamWriter,
                logger))
                return false;
            
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
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
            
            StreamIO.CloseStream(streamWriter);

            return true;
        }

        public bool Deserialize(ISerializationArgument argument, Type valueType, out object value)
        {
            FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;

            if (!StreamIO.OpenReadStream(
                filePathSettings,
                out StreamReader streamReader,
                logger))
            {
                value = default;
                
                return false;
            }

            using (var csvReader = new CsvReader(
                streamReader,
                CultureInfo.InvariantCulture))
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
            
            StreamIO.CloseStream(streamReader);

            return true;
        }

        public void Erase(ISerializationArgument argument)
        {
            FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;
            
            StreamIO.Erase(filePathSettings);
        }
    }
}