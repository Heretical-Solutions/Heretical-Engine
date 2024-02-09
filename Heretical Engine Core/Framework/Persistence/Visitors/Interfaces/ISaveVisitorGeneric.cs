namespace HereticalSolutions.Persistence
{
    /// <summary>
    /// Represents an interface for saving a value and returning a Data Transfer Object
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be saved.</typeparam>
    /// <typeparam name="TDTO">The type of the Data Transfer Object.</typeparam>
    public interface ISaveVisitorGeneric<TValue, TDTO>
    {
        /// <summary>
        /// Saves the specified value and returns the corresponding Data Transfer Object
        /// </summary>
        /// <param name="value">The value to be saved.</param>
        /// <param name="DTO">The resulting Data Transfer Object.</param>
        /// <returns>A boolean value indicating whether the save operation was successful or not.</returns>
        bool Save(TValue value, out TDTO DTO);
    }
}