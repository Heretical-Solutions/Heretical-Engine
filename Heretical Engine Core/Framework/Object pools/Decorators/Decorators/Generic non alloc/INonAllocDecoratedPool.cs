using HereticalSolutions.Pools.Arguments;

namespace HereticalSolutions.Pools
{
    public interface INonAllocDecoratedPool<T>
    {
        IPoolElement<T> Pop(IPoolDecoratorArgument[] args = null);

        void Push(
            IPoolElement<T> instance,
            bool decoratorsOnly = false);
    }
}