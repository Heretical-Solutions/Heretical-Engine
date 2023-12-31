using System.Collections;
using System.Globalization;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

using HereticalSolutions.Logging;

using CsvHelper;

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeCsvIntoTextFileStrategy : ICsvSerializationStrategy
    {
        private readonly IFormatLogger logger;

        public SerializeCsvIntoTextFileStrategy(IFormatLogger logger)
        {
            this.logger = logger;
        }

        public bool Serialize(
            ISerializationArgument argument,
            Type valueType,
            object value)
        {
            FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;

            string csv;
            
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
                
                csv = stringWriter.ToString();
            }
            
            return TextFileIO.Write(
                filePathSettings,
                csv,
                logger);
        }

        public bool Deserialize(
            ISerializationArgument argument,
            Type valueType,
            out object value)
        {
            FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;

            bool result = TextFileIO.Read(
                filePathSettings,
                out string csv,
                logger);

            if (!result)
            {
                value = default;
                
                return false;
            }

            using (StringReader stringReader = new StringReader(csv))
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
            FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;
            
            TextFileIO.Erase(filePathSettings);
        }
    }
}