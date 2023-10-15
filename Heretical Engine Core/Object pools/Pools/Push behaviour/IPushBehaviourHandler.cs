namespace HereticalSolutions.Pools
{
    /// <summary>
    /// Represents a contract for handling the push behavior in a pool.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the pool.</typeparam>
    public interface IPushBehaviourHandler<T>
    {
        /// <summary>
        /// Pushes the given pool element back into the pool.
        /// </summary>
        /// <param name="poolElement">The pool element to push.</param>
        void Push(IPoolElement<T> poolElement);
    }
}