using System;

namespace HereticalSolutions.MVVM
{
    /// <summary>
    /// Represents an observable property.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    public interface IObservableProperty<T>
    {
        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        T Value { get; set; }
        
        /// <summary>
        /// Gets or sets the value of the property as an object.
        /// </summary>
        object ObjectValue { get; set; }
        
        /// <summary>
        /// Gets or sets the callback function to be called when the value of the property changes.
        /// </summary>
        Action<T> OnValueChanged { get; set; }
        
        /// <summary>
        /// Removes all the registered listeners for the property.
        /// </summary>
        void RemoveAllListeners();

        /// <summary>
        /// Cleans up the property.
        /// </summary>
        void Cleanup();
    }
}