namespace HereticalSolutions.Collections
{
    /// <summary>
    /// Represents a collection that can be topped up with a value of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value to top up the collection with.</typeparam>
    public interface ITopUppable<T>
    {
        /// <summary>
        /// Tops up the collection with the specified value.
        /// </summary>
        /// <param name="value">The value to top up the collection with.</param>
        void TopUp(T value);
    }
}