using System;
using System.Collections.Generic;

namespace HereticalSolutions.MVVM.Observable
{
    /// <summary>
    /// Represents a delegate that determines whether a value has changed.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="previousValue">The previous value.</param>
    /// <param name="newValue">When this method returns, contains the new value. If the method returns <c>true</c>, the new value is considered changed; otherwise, if <c>false</c>, the new value is considered the same as the previous value.</param>
    /// <returns><c>true</c> if the value has changed; otherwise, <c>false</c>.</returns>
    public delegate bool PollDelegate<T>(T previousValue, out T newValue);

    /// <summary>
    /// Represents an observable property.
    /// </summary>
    /// <typeparam name="T">The type of the value of the property.</typeparam>
    public class ObservableProperty<T> : IObservableProperty<T>, IPropertyModifiedNotifiable<T>, IValuePollable
    {
        #region Active polling

        private PollDelegate<T> activePollDelegate;

        /// <summary>
        /// Gets or sets a value indicating whether the property is actively being polled.
        /// </summary>
        public bool IsActivePollable { get; private set; }

        /// <summary>
        /// Polls the value of the property and notifies subscribers if the value has changed.
        /// </summary>
        public void PollValue()
        {
            if (IsActivePollable && activePollDelegate(value, out value))
            {
                if (hasSubscribers)
                {
                    OnValueChanged.Invoke(value);
                }
            }
        }

        #endregion

        #region Model modification

        private Action<T> modifyModelDelegate;

        /// <summary>
        /// Gets or sets a value indicating whether the model is being modified.
        /// </summary>
        public bool IsModifyingModel { get; private set; }

        #endregion

        #region Value

        //I used to do OnValueChanged?.Invoke(value)
        //Until I discovered that any kind of comparisons (even null checks) for delegates produce allocations
        private bool hasSubscribers = false;

        private Action<T> onValueChanged = null;

        /// <summary>
        /// Gets or sets the action to be invoked when the value of the property changes.
        /// </summary>
        public Action<T> OnValueChanged
        {
            get { return onValueChanged; }
            set
            {
                onValueChanged = value;

                hasSubscribers = onValueChanged != null;
            }
        }

        private T value;

        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        public T Value
        {
            get { return value; }
            set
            {
                this.value = value;

                if (IsModifyingModel)
                {
                    modifyModelDelegate(value);
                }

                if (hasSubscribers)
                {
                    OnValueChanged.Invoke(this.value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the object value of the property.
        /// </summary>
        public object ObjectValue
        {
            get { return value; }
            set
            {
                this.value = (T)value;

                if (IsModifyingModel)
                {
                    modifyModelDelegate((T)value);
                }

                if (hasSubscribers)
                {
                    OnValueChanged.Invoke(this.value);
                }
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableProperty{T}"/> class with the specified default value, active poll delegate, and modify model delegate.
        /// </summary>
        /// <param name="defaultValue">The default value of the property.</param>
        /// <param name="activePollDelegate">The delegate used to actively poll the value of the property.</param>
        /// <param name="modifyModelDelegate">The delegate used to modify the model when the value of the property changes.</param>
        public ObservableProperty(
            T defaultValue = default(T),
            PollDelegate<T> activePollDelegate = null,
            Action<T> modifyModelDelegate = null)
        {
            value = defaultValue;
            this.activePollDelegate = activePollDelegate;
            this.modifyModelDelegate = modifyModelDelegate;

            IsActivePollable = activePollDelegate != null;
            IsModifyingModel = modifyModelDelegate != null;

            RemoveAllListeners();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableProperty{T}"/> class with the specified poll value delegate and modify model delegate.
        /// </summary>
        /// <param name="pollValueDelegate">The delegate used to poll the value of the property.</param>
        /// <param name="modifyModelDelegate">The delegate used to modify the model when the value of the property changes.</param>
        public ObservableProperty(
            Func<T> pollValueDelegate,
            Action<T> modifyModelDelegate = null)
        {
            activePollDelegate =
                (T previousValue, out T result) =>
                {
                    result = pollValueDelegate();

                    //Courtesy of https://stackoverflow.com/questions/488250/c-sharp-compare-two-generic-values
                    return !EqualityComparer<T>.Default.Equals(previousValue, result);
                };

            this.modifyModelDelegate = modifyModelDelegate;

            IsActivePollable = true;

            IsModifyingModel = modifyModelDelegate != null;

            RemoveAllListeners();

            activePollDelegate.Invoke(default(T), out value);
        }

        #region Cleanup

        /// <summary>
        /// Removes all listeners from the <see cref="OnValueChanged"/> event.
        /// </summary>
        public void RemoveAllListeners()
        {
            OnValueChanged = null;
        }

        /// <summary>
        /// Cleans up and resets the properties of the observable property.
        /// </summary>
        public void Cleanup()
        {
            value = default(T);

            activePollDelegate = null;

            modifyModelDelegate = null;

            IsActivePollable = false;

            IsModifyingModel = false;

            RemoveAllListeners();
        }

        #endregion
    }
}