using HereticalSolutions.Pools.Arguments;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Represents a builder class for creating an array of IPoolDecoratorArguments without allocating memory.
    /// </summary>
    public class ArgumentBuilderNonAlloc
    {
        private readonly IPoolDecoratorArgument[] argumentChain;

        private int count;

        /// <summary>
        /// Initializes a new instance of the ArgumentBuilderNonAlloc class with the specified argument chain.
        /// </summary>
        /// <param name="argumentChain">The array of IPoolDecoratorArguments to be used as the argument chain.</param>
        public ArgumentBuilderNonAlloc(IPoolDecoratorArgument[] argumentChain)
        {
            this.argumentChain = argumentChain;

            count = 0;
        }

        /// <summary>
        /// Resets the argument chain by setting all elements to null and resetting the count to 0.
        /// </summary>
        /// <returns>The current instance of the ArgumentBuilderNonAlloc class.</returns>
        public ArgumentBuilderNonAlloc Clean()
        {
            for (int i = 0; i < count; i++)
                argumentChain[i] = null;

            count = 0;

            return this;
        }

        /// <summary>
        /// Adds an IPoolDecoratorArgument to the argument chain.
        /// </summary>
        /// <param name="argument">The IPoolDecoratorArgument to be added.</param>
        /// <returns>The current instance of the ArgumentBuilderNonAlloc class.</returns>
        public ArgumentBuilderNonAlloc Add(IPoolDecoratorArgument argument)
        {
            argumentChain[count] = argument;

            count++;

            return this;
        }

        /// <summary>
        /// Builds and returns the argument chain as an array of IPoolDecoratorArguments.
        /// </summary>
        /// <returns>An array of IPoolDecoratorArguments representing the argument chain.</returns>
        public IPoolDecoratorArgument[] Build()
        {
            return argumentChain;
        }
    }
}