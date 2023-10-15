namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Attribute used to mark a struct as a network event component.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Struct)]
    public class NetworkEventComponentAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkEventComponentAttribute"/> class.
        /// </summary>
        public NetworkEventComponentAttribute()
        {
        }
    }
}