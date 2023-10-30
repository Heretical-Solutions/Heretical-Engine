using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Represents a JSON serialization strategy that serializes JSON into a text file.
    /// </summary>
    public class SerializeJsonIntoTextFileStrategy : IJsonSerializationStrategy
    {
        /// <summary>
        /// Serializes the specified JSON into a text file.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="json">The JSON string to be serialized.</param>
        /// <returns>true if the serialization was successful, false otherwise.</returns>
        public bool Serialize(ISerializationArgument argument, string json)
        {
            FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;
            
            return TextFileIO.Write(filePathSettings, json);
        }

        /// <summary>
        /// Deserializes JSON from a text file.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="json">The deserialized JSON string.</param>
        /// <returns>true if the deserialization was successful, false otherwise.</returns>
        public bool Deserialize(ISerializationArgument argument, out string json)
        {
            FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;
            
            return TextFileIO.Read(filePathSettings, out json);
        }
        
        /// <summary>
        /// Erases the contents of a text file.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        public void Erase(ISerializationArgument argument)
        {
            FilePathSettings filePathSettings = ((TextFileArgument)argument).Settings;
            
            TextFileIO.Erase(filePathSettings);
        }
    }
}