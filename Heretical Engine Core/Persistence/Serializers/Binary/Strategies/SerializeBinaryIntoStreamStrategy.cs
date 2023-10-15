using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

//BinaryFormatter.Serialize(Stream, object)' is obsolete: 'BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.
// Disable the warning.
#pragma warning disable SYSLIB0011

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Implements the <see cref="IBinarySerializationStrategy"/> interface and provides a strategy for serializing and deserializing binary data into and from a stream.
    /// </summary>
    public class SerializeBinaryIntoStreamStrategy : IBinarySerializationStrategy
    {
        /// <summary>
        /// Serializes the specified value into the stream using the given binary formatter and serialization argument.
        /// </summary>
        /// <param name="argument">The serialization argument representing the stream.</param>
        /// <param name="formatter">The binary formatter used for serialization.</param>
        /// <param name="value">The object to be serialized.</param>
        /// <returns><c>true</c> if serialization was successful; otherwise, <c>false</c>.</returns>
        public bool Serialize(ISerializationArgument argument, BinaryFormatter formatter, object value)
        {
            FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;
            
            if (!StreamIO.OpenWriteStream(fileSystemSettings, out FileStream fileStream))
                return false;
            
            formatter.Serialize(fileStream, value);
            
            StreamIO.CloseStream(fileStream);

            return true;
        }

        /// <summary>
        /// Deserializes the value from the stream using the given binary formatter and serialization argument.
        /// </summary>
        /// <param name="argument">The serialization argument representing the stream.</param>
        /// <param name="formatter">The binary formatter used for deserialization.</param>
        /// <param name="value">When this method returns, contains the deserialized object if deserialization was successful; otherwise, the default value for <see cref="object"/>.</param>
        /// <returns><c>true</c> if deserialization was successful; otherwise, <c>false</c>.</returns>
        public bool Deserialize(ISerializationArgument argument, BinaryFormatter formatter, out object value)
        {
            FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;
            
            if (!StreamIO.OpenReadStream(fileSystemSettings, out FileStream fileStream))
            {
                value = default(object);
                
                return false;
            }

            value = formatter.Deserialize(fileStream);
            
            StreamIO.CloseStream(fileStream);

            return true;
        }

        /// <summary>
        /// Erases the data in the stream using the given serialization argument.
        /// </summary>
        /// <param name="argument">The serialization argument representing the stream.</param>
        public void Erase(ISerializationArgument argument)
        {
            FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;
            
            StreamIO.Erase(fileSystemSettings);
        }
    }
}