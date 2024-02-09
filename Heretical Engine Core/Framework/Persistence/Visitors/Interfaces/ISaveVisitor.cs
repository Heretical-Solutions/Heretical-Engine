namespace HereticalSolutions.Persistence
{
    /// <summary>
    /// Represents an interface for saving objects and converting them to corresponding data transfer objects
    /// </summary>
    public interface ISaveVisitor
    {
        /// <summary>
        /// Saves the specified value and converts it to a data transfer object
        /// </summary>
        /// <typeparam name="TValue">The type of the value to be saved.</typeparam>
        /// <param name="value">The value to be saved.</param>
        /// <param name="DTO">The resulting data transfer object.</param>
        /// <returns><c>true</c> if the value was saved successfully; otherwise, <c>false</c>.</returns>
        bool Save<TValue>(TValue value, out object DTO);
        
        /// <summary>
        /// Saves the specified value and converts it to a data transfer object
        /// </summary>
        /// <typeparam name="TValue">The type of the value to be saved.</typeparam>
        /// <typeparam name="TDTO">The type of the data transfer object.</typeparam>
        /// <param name="value">The value to be saved.</param>
        /// <param name="DTO">The resulting data transfer object.</param>
        /// <returns><c>true</c> if the value was saved successfully; otherwise, <c>false</c>.</returns>
        bool Save<TValue, TDTO>(TValue value, out TDTO DTO);
    }
}