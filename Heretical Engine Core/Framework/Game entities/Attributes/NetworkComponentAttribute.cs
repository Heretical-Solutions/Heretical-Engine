namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Attribute used to mark a class or struct as a network component.
    /// </summary>
    [System.AttributeUsage(
        System.AttributeTargets.Class
        | System.AttributeTargets.Struct)]
    public class NetworkComponentAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkComponentAttribute"/> class.
        /// </summary>
        public NetworkComponentAttribute()
        {
        }
    }
}