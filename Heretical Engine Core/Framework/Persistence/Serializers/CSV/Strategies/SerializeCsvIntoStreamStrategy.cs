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
    /// Represents a strategy for serializing and deserializing data into/from a CSV format.
    /// </summary>
    public class SerializeCsvIntoStreamStrategy : ICsvSerializationStrategy
    {
        /// <summary>
        /// Serializes the specified value into a CSV format and writes it to a stream.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="valueType">The type of the value being serialized.</param>
        /// <param name="value">The value to be serialized.</param>
        /// <returns>
        ///   <c>true</c> if the serialization is successful; otherwise, <c>false</c>.
        /// </returns>
        public bool Serialize(ISerializationArgument argument, Type valueType, object value)
        {
            FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;
            
            if (!StreamIO.OpenWriteStream(fileSystemSettings, out StreamWriter streamWriter))
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

        /// <summary>
        /// Deserializes the CSV data from a stream into an object of the specified type.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="valueType">The type of the value being deserialized.</param>
        /// <param name="value">The deserialized value.</param>
        /// <returns>
        ///   <c>true</c> if the deserialization is successful; otherwise, <c>false</c>.
        /// </returns>
        public bool Deserialize(ISerializationArgument argument, Type valueType, out object value)
        {
            FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;

            if (!StreamIO.OpenReadStream(fileSystemSettings, out StreamReader streamReader))
            {
                value = default(object);
                
                return false;
            }

            using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
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

        /// <summary>
        /// Erases the data from the underlying stream.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        public void Erase(ISerializationArgument argument)
        {
            FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;
            
            StreamIO.Erase(fileSystemSettings);
        }
    }
}