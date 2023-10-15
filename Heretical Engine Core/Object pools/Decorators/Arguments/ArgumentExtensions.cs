using HereticalSolutions.Pools; 

namespace HereticalSolutions.Pools.Arguments
{
    /// <summary>
    /// A static class that contains extension methods for handling arguments.
    /// </summary>
    public static class ArgumentExtensions
    {
        /// <summary>
        /// Attempts to get the argument of the specified type from an array of pool decorator arguments.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument to retrieve.</typeparam>
        /// <param name="args">The array of pool decorator arguments.</param>
        /// <param name="value">When this method returns, contains the argument of the specified type if it was found; otherwise, the default value for <typeparamref name="TArgument"/>.</param>
        /// <returns><see langword="true"/> if an argument of the specified type was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetArgument<TArgument>(this IPoolDecoratorArgument[] args, out TArgument value) where TArgument : IPoolDecoratorArgument
        {
            value = default(TArgument);

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is TArgument)
                {
                    value = (TArgument)args[i];

                    return true;
                }
            }

            return false;
        }
    }
}