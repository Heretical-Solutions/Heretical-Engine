namespace HereticalSolutions.MVVM
{
    /// <summary>
    /// Represents an interface for notifying when a property has been modified.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    public interface IPropertyModifiedNotifiable<T>
    {
        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        T Value { set; }
    }
}