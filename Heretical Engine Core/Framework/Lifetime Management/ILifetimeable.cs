using System;

namespace HereticalSolutions.LifetimeManagement
{
    /// <summary>
    /// Represents an object that can be managed throughout its lifetime.
    /// </summary>
    public interface ILifetimeable
    {
        #region Set up

        /// <summary>
        /// Sets up the object.
        /// </summary>
        void SetUp();

        /// <summary>
        /// Gets a value indicating whether the object has been set up.
        /// </summary>
        bool IsSetUp { get; }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes the object.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Gets a value indicating whether the object has been initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Gets or sets the action to be performed when the object is initialized.
        /// </summary>
        Action OnInitialized { get; set; }

        #endregion

        #region Cleanup

        /// <summary>
        /// Cleans up the object.
        /// </summary>
        void Cleanup();

        /// <summary>
        /// Gets or sets the action to be performed when the object is cleaned up.
        /// </summary>
        Action OnCleanedUp { get; set; }

        #endregion

        #region Tear down

        /// <summary>
        /// Tears down the object.
        /// </summary>
        void TearDown();

        /// <summary>
        /// Gets or sets the action to be performed when the object is torn down.
        /// </summary>
        Action OnTornDown { get; set; }

        #endregion
    }
}