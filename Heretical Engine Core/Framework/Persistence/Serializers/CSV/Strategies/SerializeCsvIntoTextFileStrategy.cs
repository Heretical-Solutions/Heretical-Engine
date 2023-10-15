using System;
using System.Collections;
using System.Globalization;
using System.IO;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

using CsvHelper;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Serializes objects into a CSV and writes it into a text file.
    /// </summary>
    public class SerializeCsvIntoTextFileStrategy : ICsvSerializationStrategy
    {
        /// <summary>
        /// Serializes an object into a CSV and writes it into a text file.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="valueType">The type of the object being serialized.</param>
        /// <param name="value">The object to be serialized.</param>
        /// <returns>true if the serialization was successful, otherwise false.</returns>
        public bool Serialize(ISerializationArgument argument, Type valueType, object value)
        {
            FileSystemSettings fileSystemSettings = ((TextFileArgument)argument).Settings;

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
            
            return TextFileIO.Write(fileSystemSettings, csv);
        }

        /// <summary>
        /// Deserializes an object from a CSV in a text file.
        /// </summary>
        /// <param name="argument">The deserialization argument.</param>
        /// <param name="valueType">The type of the object being deserialized.</param>
        /// <param name="value">The deserialized object.</param>
        /// <returns>true if the deserialization was successful, otherwise false.</returns>
        public bool Deserialize(ISerializationArgument argument, Type valueType, out object value)
        {
            FileSystemSettings fileSystemSettings = ((TextFileArgument)argument).Settings;

            bool result = TextFileIO.Read(fileSystemSettings, out string csv);

            if (!result)
            {
                value = default(object);
                
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
        
        /// <summary>
        /// Erases the text file associated with the serialization argument.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        public void Erase(ISerializationArgument argument)
        {
            FileSystemSettings fileSystemSettings = ((TextFileArgument)argument).Settings;
            
            TextFileIO.Erase(fileSystemSettings);
        }
    }
}