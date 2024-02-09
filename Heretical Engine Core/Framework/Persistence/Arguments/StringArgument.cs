namespace HereticalSolutions.Persistence.Arguments
{
    /// <summary>
    /// Represents a string argument used for serialization
    /// </summary>
    public class StringArgument : ISerializationArgument
    {
        /// <summary>
        /// Gets or sets the value of the string argument
        /// </summary>
        public string Value { get; set; }
    }
}