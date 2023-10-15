using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Implements the <see cref="IYamlSerializationStrategy"/> interface for serializing YAML into a text file.
    /// </summary>
    public class SerializeYamlIntoTextFileStrategy : IYamlSerializationStrategy
    {
        /// <summary>
        /// Serializes the specified YAML string into a text file.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="yaml">The YAML string to be serialized.</param>
        /// <returns>true if serialization is successful, otherwise false.</returns>
        public bool Serialize(ISerializationArgument argument, string yaml)
        {
            FileSystemSettings fileSystemSettings = ((TextFileArgument)argument).Settings;
            
            return TextFileIO.Write(fileSystemSettings, yaml);
        }

        /// <summary>
        /// Deserializes the YAML string from a text file.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="yaml">When this method returns, contains the deserialized YAML string if successful, otherwise an empty string.</param>
        /// <returns>true if deserialization is successful, otherwise false.</returns>
        public bool Deserialize(ISerializationArgument argument, out string yaml)
        {
            FileSystemSettings fileSystemSettings = ((TextFileArgument)argument).Settings;
            
            return TextFileIO.Read(fileSystemSettings, out yaml);
        }
        
        /// <summary>
        /// Erases the contents of the text file.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        public void Erase(ISerializationArgument argument)
        {
            FileSystemSettings fileSystemSettings = ((TextFileArgument)argument).Settings;
            
            TextFileIO.Erase(fileSystemSettings);
        }
    }
}