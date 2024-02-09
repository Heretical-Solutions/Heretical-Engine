namespace HereticalSolutions.Persistence
{
    /// <summary>
    /// Represents a generic interface for loading values from a data transfer object (DTO)
    /// </summary>
    /// <typeparam name="TValue">The type of value to load.</typeparam>
    /// <typeparam name="TDTO">The type of data transfer object.</typeparam>
    public interface ILoadVisitorGeneric<TValue, TDTO>
    {
        /// <summary>
        /// Loads a value from the specified data transfer object (DTO) and returns a boolean indicating whether the loading was successful
        /// </summary>
        /// <param name="DTO">The data transfer object to load from.</param>
        /// <param name="value">When this method returns, contains the loaded value if the loading was successful, or the default value of <typeparamref name="TValue"/> if the loading failed.</param>
        /// <returns>True if the loading was successful; otherwise, false.</returns>
        bool Load(TDTO DTO, out TValue value);
        
        /// <summary>
        /// Loads a value from the specified data transfer object (DTO) and populates the provided value with the loaded data, returning a boolean indicating whether the loading was successful
        /// </summary>
        /// <param name="DTO">The data transfer object to load from.</param>
        /// <param name="valueToPopulate">The value to populate with the loaded data.</param>
        /// <returns>True if the loading was successful; otherwise, false.</returns>
        bool Load(TDTO DTO, TValue valueToPopulate);
    }
}