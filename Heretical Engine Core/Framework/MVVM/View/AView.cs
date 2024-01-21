using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.MVVM.View
{
    /// <summary>
    /// Represents a base class for views in the MVVM architecture.
    /// </summary>
    public abstract class AView
        : ILifetimeable,
          ISetUppable,
          IInitializable,
          ICleanUppable,
          ITearDownable
    {
        protected IViewModel viewModel;

        protected ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AView"/> class.
        /// </summary>
        /// <param name="viewModel">The view model associated with this view.</param>
        /// <param name="logger">The logger to be used for logging.</param>
        public AView(
            IViewModel viewModel,
            ILogger logger = null)
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

        #endregion

        #region ISetUppable

        public virtual void SetUp()
        {
            if (IsSetUp)
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        "ALREADY SET UP"));

            IsSetUp = true;
        }

        #endregion

        #region IInitializable

        public virtual void Initialize(object[] args)
        {
            if (!IsSetUp)
            {
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        "VIEW SHOULD BE SET UP BEFORE BEING INITIALIZED"));
            }

            if (IsInitialized)
            {
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        "INITIALIZING VIEW THAT IS ALREADY INITIALIZED"));
            }

            IsInitialized = true;

            OnInitialized?.Invoke();
        }

        #endregion

        #region ICleanUppable

        public virtual void Cleanup()
        {
            if (!IsInitialized)
                return;

            IsInitialized = false;

            OnCleanedUp?.Invoke();
        }

        #endregion

        #region ITearDownable

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

        protected void TryObtainProperty<T>(string propertyID, out IObservableProperty<T> property)
        {
            bool propertyObtained = viewModel.GetObservable<T>(propertyID, out property);

            if (!propertyObtained)
            {
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        $"COULD NOT OBTAIN PROPERTY {propertyID} FROM VIEWMODEL {viewModel.GetType().Name}"));
            }
        }
    }
}