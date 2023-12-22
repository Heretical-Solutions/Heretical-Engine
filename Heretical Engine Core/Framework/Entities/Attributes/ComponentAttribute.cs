namespace HereticalSolutions.GameEntities
{
    [System.AttributeUsage(System.AttributeTargets.Struct)]
    public class ComponentAttribute : System.Attribute
    {
        public string Path { get; private set; }

        public ComponentAttribute(string path = "")
        {
            Path = path;
        }
    }
}