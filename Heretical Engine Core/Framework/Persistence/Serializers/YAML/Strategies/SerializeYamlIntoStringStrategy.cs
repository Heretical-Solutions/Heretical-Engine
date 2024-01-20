using HereticalSolutions.Persistence.Arguments;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeYamlIntoStringStrategy : IYamlSerializationStrategy
    {
        private readonly ILogger logger;

        public SerializeYamlIntoStringStrategy(
            ILogger logger = null)
        {
            this.logger = logger;
        }

        public bool Serialize(ISerializationArgument argument, string yaml)
        {
            ((StringArgument)argument).Value = yaml;

            return true;
        }

        public bool Deserialize(ISerializationArgument argument, out string yaml)
        {
            yaml = ((StringArgument)argument).Value;
            
            return true;
        }
        
        public void Erase(ISerializationArgument argument)
        {
            ((StringArgument)argument).Value = string.Empty;
        }
    }
}