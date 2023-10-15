using System;

namespace HereticalSolutions.Delegates.Broadcasting
{
    /// <summary>
    /// Represents a generic broadcaster that can publish and subscribe to events with a single argument of type TValue.
    /// </summary>
    /// <typeparam name="TValue">The type of argument for the event.</typeparam>
    public class BroadcasterGeneric<TValue>
        : IPublisherSingleArgGeneric<TValue>,
          IPublisherSingleArg,
          ISubscribableSingleArgGeneric<TValue>,
          ISubscribableSingleArg
    {
        private Action<TValue> multicastDelegate;

        #region IPublisherSingleArgGeneric

        /// <summary>
        /// Publishes the event to all subscribed delegates.
        /// </summary>
        /// <param name="value">The argument for the event.</param>
        public void Publish(TValue value)
        {
            multicastDelegate?.Invoke(value);
        }

        #endregion

        #region IPublisherSingleArg

        /// <summary>
        /// Publishes the event to all subscribed delegates.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <param name="value">The argument for the event.</param>
        public void Publish<TArgument>(TArgument value)
        {
            if (!(typeof(TArgument).Equals(typeof(TValue))))
                throw new Exception($"[BroadcasterGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{typeof(TArgument).ToString()}\"");

            //DIRTY HACKS DO NOT REPEAT
            object valueObject = (object)value;

            Publish((TValue)valueObject); //It doesn't want to convert TArgument into TValue. Bastard
        }

        /// <summary>
        /// Publishes the event to all subscribed delegates.
        /// </summary>
        /// <param name="valueType">The type of the argument.</param>
        /// <param name="value">The argument for the event.</param>
        public void Publish(Type valueType, object value)
        {
            if (!(valueType.Equals(typeof(TValue))))
                throw new Exception($"[BroadcasterGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{valueType.ToString()}\"");

            Publish((TValue)value);
        }

        #endregion

        #region ISubscribableSingleArgGeneric

        /// <summary>
        /// Subscribes to the event with the given delegate.
        /// </summary>
        /// <param name="delegate">The delegate to subscribe.</param>
        public void Subscribe(Action<TValue> @delegate)
        {
            multicastDelegate += @delegate;
        }

        /// <summary>
        /// Subscribes to the event with the given delegate.
        /// </summary>
        /// <param name="delegate">The delegate to subscribe.</param>
        public void Subscribe(object @delegate)
        {
            multicastDelegate += (Action<TValue>)@delegate;
        }

        /// <summary>
        /// Unsubscribes from the event with the given delegate.
        /// </summary>
        /// <param name="delegate">The delegate to unsubscribe.</param>
        public void Unsubscribe(Action<TValue> @delegate)
        {
            multicastDelegate -= @delegate;
        }

        /// <summary>
        /// Unsubscribes from the event with the given delegate.
        /// </summary>
        /// <param name="delegate">The delegate to unsubscribe.</param>
        public void Unsubscribe(object @delegate)
        {
            multicastDelegate -= (Action<TValue>)@delegate;
        }

        #endregion

        #region ISubscribableSingleArg

        /// <summary>
        /// Subscribes to the event with the given delegate.
        /// </summary>
        /// <typeparam name="TArgument">The type of the delegate argument.</typeparam>
        /// <param name="delegate">The delegate to subscribe.</param>
        public void Subscribe<TArgument>(Action<TArgument> @delegate)
        {
            if (!(typeof(TArgument).Equals(typeof(TValue))))
                throw new Exception($"[BroadcasterGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{typeof(TArgument).ToString()}\"");

            //DIRTY HACKS DO NOT REPEAT
            object delegateObject = (object)@delegate;

            multicastDelegate += (Action<TValue>)delegateObject;
        }

        /// <summary>
        /// Subscribes to the event with the given delegate.
        /// </summary>
        /// <param name="valueType">The type of the delegate argument.</param>
        /// <param name="delegate">The delegate to subscribe.</param>
        public void Subscribe(Type valueType, object @delegate)
        {
            if (!(valueType.Equals(typeof(TValue))))
                throw new Exception($"[BroadcasterGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{valueType.ToString()}\"");

            multicastDelegate += (Action<TValue>)@delegate;
        }

        /// <summary>
        /// Unsubscribes from the event with the given delegate.
        /// </summary>
        /// <typeparam name="TArgument">The type of the delegate argument.</typeparam>
        /// <param name="delegate">The delegate to unsubscribe.</param>
        public void Unsubscribe<TArgument>(Action<TArgument> @delegate)
        {
            if (!(typeof(TArgument).Equals(typeof(TValue))))
                throw new Exception($"[BroadcasterGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{typeof(TArgument).ToString()}\"");

            //DIRTY HACKS DO NOT REPEAT
            object delegateObject = (object)@delegate;

            multicastDelegate -= (Action<TValue>)delegateObject;
        }

        /// <summary>
        /// Unsubscribes from the event with the given delegate.
        /// </summary>
        /// <param name="valueType">The type of the delegate argument.</param>
        /// <param name="delegate">The delegate to unsubscribe.</param>
        public void Unsubscribe(Type valueType, object @delegate)
        {
            if (!(valueType.Equals(typeof(TValue))))
                throw new Exception($"[BroadcasterGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{valueType.ToString()}\"");

            multicastDelegate -= (Action<TValue>)@delegate;
        }

        #endregion
    }
}