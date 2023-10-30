using System.IO;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
    /// <summary>
    /// Serializes and deserializes JSON into and from a stream.
    /// </summary>
    public class SerializeJsonIntoStreamStrategy : IJsonSerializationStrategy
    {
        /// <summary>
        /// Serializes the given JSON into a stream.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="json">The JSON to be serialized.</param>
        /// <returns><c>true</c> if the serialization was successful; otherwise, <c>false</c>.</returns>
        public bool Serialize(ISerializationArgument argument, string json)
        {
            FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;

            if (!StreamIO.OpenWriteStream(filePathSettings, out StreamWriter streamWriter))
                return false;

            streamWriter.Write(json);

            StreamIO.CloseStream(streamWriter);

            return true;
        }

        /// <summary>
        /// Deserializes the JSON from a stream.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        /// <param name="json">The deserialized JSON.</param>
        /// <returns><c>true</c> if the deserialization was successful; otherwise, <c>false</c>.</returns>
        public bool Deserialize(ISerializationArgument argument, out string json)
        {
            FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;

            json = string.Empty;

            if (!StreamIO.OpenReadStream(filePathSettings, out StreamReader streamReader))
                return false;

            json = streamReader.ReadToEnd();

            StreamIO.CloseStream(streamReader);

            return true;
        }

        /// <summary>
        /// Erases the serialized JSON from a stream.
        /// </summary>
        /// <param name="argument">The serialization argument.</param>
        public void Erase(ISerializationArgument argument)
        {
            FilePathSettings filePathSettings = ((StreamArgument)argument).Settings;

            StreamIO.Erase(filePathSettings);
        }
    }
}