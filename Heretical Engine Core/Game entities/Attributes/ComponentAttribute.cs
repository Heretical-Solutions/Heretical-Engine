namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents an attribute that can be applied to a struct.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Struct)]
    public class ComponentAttribute : System.Attribute
    {
        /// <summary>
        /// Gets or sets the path associated with the component.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentAttribute"/> class.
        /// </summary>
        /// <param name="path">The path associated with the component.</param>
        public ComponentAttribute(string path = "")
        {
            Path = path;
        }
    }
}