namespace HereticalSolutions.Persistence
{
    /// <summary>
    /// Represents a visitor interface for loading values from Data Transfer Objects (DTOs)
    /// </summary>
    public interface ILoadVisitor
    {
        /// <summary>
        /// Loads the value of type TValue from the given DTO object
        /// </summary>
        /// <typeparam name="TValue">The type of the value to load.</typeparam>
        /// <param name="DTO">The Data Transfer Object to load from.</param>
        /// <param name="value">The loaded value.</param>
        /// <returns><c>true</c> if the value was successfully loaded; otherwise, <c>false</c>.</returns>
        bool Load<TValue>(object DTO, out TValue value);
        
        /// <summary>
        /// Loads the value of type TValue from the given DTO object
        /// </summary>
        /// <typeparam name="TValue">The type of the value to load.</typeparam>
        /// <typeparam name="TDTO">The type of the Data Transfer Object.</typeparam>
        /// <param name="DTO">The Data Transfer Object to load from.</param>
        /// <param name="value">The loaded value.</param>
        /// <returns><c>true</c> if the value was successfully loaded; otherwise, <c>false</c>.</returns>
        bool Load<TValue, TDTO>(TDTO DTO, out TValue value);

        /// <summary>
        /// Loads the value of type TValue from the given DTO object and populates an existing value
        /// </summary>
        /// <typeparam name="TValue">The type of the value to load.</typeparam>
        /// <param name="DTO">The Data Transfer Object to load from.</param>
        /// <param name="valueToPopulate">The value to populate.</param>
        /// <returns><c>true</c> if the value was successfully loaded and populated; otherwise, <c>false</c>.</returns>
        bool Load<TValue>(object DTO, TValue valueToPopulate);
        
        /// <summary>
        /// Loads the value of type TValue from the given DTO object and populates an existing value
        /// </summary>
        /// <typeparam name="TValue">The type of the value to load.</typeparam>
        /// <typeparam name="TDTO">The type of the Data Transfer Object.</typeparam>
        /// <param name="DTO">The Data Transfer Object to load from.</param>
        /// <param name="valueToPopulate">The value to populate.</param>
        /// <returns><c>true</c> if the value was successfully loaded and populated; otherwise, <c>false</c>.</returns>
        bool Load<TValue, TDTO>(TDTO DTO, TValue valueToPopulate);
    }
}