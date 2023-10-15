using System;

namespace HereticalSolutions.Delegates
{
    /// <summary>
    /// Represents an interface for publishing events with a single argument.
    /// </summary>
    public interface IPublisherSingleArg
    {
        /// <summary>
        /// Publishes an event with a single argument of type TValue.
        /// </summary>
        /// <typeparam name="TValue">The type of the argument.</typeparam>
        /// <param name="value">The argument value.</param>
        void Publish<TValue>(TValue value);

        /// <summary>
        /// Publishes an event with a single argument of the specified type.
        /// </summary>
        /// <param name="valueType">The type of the argument.</param>
        /// <param name="value">The argument value.</param>
        void Publish(Type valueType, object value);
    }
}