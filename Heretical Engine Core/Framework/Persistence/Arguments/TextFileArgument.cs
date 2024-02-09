using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Arguments
{
    /// <summary>
    /// Represents an argument for serializing data into a text file
    /// </summary>
    public class TextFileArgument : ISerializationArgument
    {
        /// <summary>
        /// Gets or sets the file system settings
        /// </summary>
        public FilePathSettings Settings { get; set; }
    }
}