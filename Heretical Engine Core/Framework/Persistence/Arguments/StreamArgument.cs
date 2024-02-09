using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Arguments
{
    /// <summary>
    /// Represents a serialization argument for working with stream-based file systems
    /// </summary>
    public class StreamArgument : ISerializationArgument
    {
        /// <summary>
        /// Gets or sets the file system settings
        /// </summary>
        public FilePathSettings Settings { get; set; }
    }
}