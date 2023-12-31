namespace HereticalSolutions.GameEntities
{
    [System.AttributeUsage(
        System.AttributeTargets.Class
        | System.AttributeTargets.Struct)]
    public class NetworkComponentAttribute : System.Attribute
    {
        public NetworkComponentAttribute()
        {
        }
    }
}