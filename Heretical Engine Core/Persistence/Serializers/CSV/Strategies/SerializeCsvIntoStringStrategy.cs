using System;
using System.Collections;
using System.Globalization;
using System.IO;

using HereticalSolutions.Persistence.Arguments;

using CsvHelper;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Represents a serialization strategy that serializes objects to CSV format and deserializes CSV strings into objects.
    /// </summary>
    public class SerializeCsvIntoStringStrategy : ICsvSerializationStrategy
    {
        /// <summary>
        /// Serializes the specified value into a CSV string.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="valueType">The type of the value to be serialized.</param>
        /// <param name="value">The value to be serialized.</param>
        /// <returns>True if the serialization is successful, false otherwise.</returns>
        public bool Serialize(ISerializationArgument argument, Type valueType, object value)
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

        /// <summary>
        /// Deserializes a CSV string into an object of the specified type.
        /// </summary>
        /// <param name="argument">The deserialization argument.</param>
        /// <param name="valueType">The type of the object to be deserialized into.</param>
        /// <param name="value">When this method returns, contains the deserialized object. If deserialization fails, contains null.</param>
        /// <returns>True if the deserialization is successful, false otherwise.</returns>
        public bool Deserialize(ISerializationArgument argument, Type valueType, out object value)
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
        
        /// <summary>
        /// Erases the value stored in the serialization argument.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        public void Erase(ISerializationArgument argument)
        {
            ((StringArgument)argument).Value = string.Empty;
        }
    }
}