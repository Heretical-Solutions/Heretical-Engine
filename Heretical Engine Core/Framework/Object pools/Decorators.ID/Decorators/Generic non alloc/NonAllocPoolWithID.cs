using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Decorators
{
    public class NonAllocPoolWithID<T> : ANonAllocDecoratorPool<T>
    {
        public string ID { get; private set; }

        public NonAllocPoolWithID(
            INonAllocDecoratedPool<T> innerPool,
            string id,
            ILogger logger = null)
            : base(
                innerPool,
                logger)
        {
            ID = id;
        }
    }
}