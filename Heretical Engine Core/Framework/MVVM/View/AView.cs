using System;
using HereticalSolutions.LifetimeManagement;
using HereticalSolutions.Logging;

namespace HereticalSolutions.MVVM.View
{
    /// <summary>
    /// Represents a base class for views in the MVVM architecture.
    /// </summary>
    public abstract class AView : ILifetimeable
    {
        protected IViewModel viewModel;
        protected IFormatLogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AView"/> class.
        /// </summary>
        /// <param name="viewModel">The view model associated with this view.</param>
        /// <param name="logger">The logger to be used for logging.</param>
        public AView(IViewModel viewModel, IFormatLogger logger)
        {
            this.viewModel = viewModel;
            this.logger = logger;
        }

        #region ILifetimable

        /// <summary>
        /// Gets or sets a value indicating whether this view is set up.
        /// </summary>
        public bool IsSetUp { get; protected set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether this view is initialized.
        /// </summary>
        public bool IsInitialized { get; protected set; } = false;

        /// <summary>
        /// Gets or sets the callback to be invoked when the view is initialized.
        /// </summary>
        public Action OnInitialized { get; set; }

        /// <summary>
        /// Gets or sets the callback to be invoked when the view is cleaned up.
        /// </summary>
        public Action OnCleanedUp { get; set; }

        /// <summary>
        /// Gets or sets the callback to be invoked when the view is torn down.
        /// </summary>
        public Action OnTornDown { get; set; }

        /// <summary>
        /// Sets up the view.
        /// </summary>
        public virtual void SetUp()
        {
            if (IsSetUp)
                logger.ThrowException(
                    GetType(),
                    "ALREADY SET UP");

            IsSetUp = true;
        }

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public virtual void Initialize()
        {
            if (!IsSetUp)
            {
                logger.ThrowException(
                    GetType(),
                    "VIEW SHOULD BE SET UP BEFORE BEING INITIALIZED");
            }

            if (IsInitialized)
            {
                logger.ThrowException(
                    GetType(),
                    "INITIALIZING VIEW THAT IS ALREADY INITIALIZED");
            }

            IsInitialized = true;

            OnInitialized?.Invoke();
        }

        /// <summary>
        /// Cleans up the view.
        /// </summary>
        public virtual void Cleanup()
        {
            if (!IsInitialized)
                return;

            IsInitialized = false;

            OnCleanedUp?.Invoke();
        }

        /// <summary>
        /// Tears down the view.
        /// </summary>
        public virtual void TearDown()
        {
            if (!IsSetUp)
                return;

            IsSetUp = false;
            
            Cleanup();

            OnTornDown?.Invoke();

            OnInitialized = null;
            OnCleanedUp = null;
            OnTornDown = null;
        }

        #endregion

        /// <summary>
        /// Tries to obtain an observable property from the view model.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyID">The ID of the property.</param>
        /// <param name="property">The resulting <see cref="IObservableProperty{T}"/>.</param>
        protected void TryObtainProperty<T>(string propertyID, out IObservableProperty<T> property)
        {
            bool propertyObtained = viewModel.GetObservable<T>(propertyID, out property);

            if (!propertyObtained)
            {
                logger.ThrowException(
                    GetType(),
                    $"COULD NOT OBTAIN PROPERTY {propertyID} FROM VIEWMODEL {viewModel.GetType().Name}");
            }
        }
    }
}