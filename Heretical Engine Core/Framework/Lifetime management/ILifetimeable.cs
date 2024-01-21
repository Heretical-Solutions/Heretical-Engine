namespace HereticalSolutions.LifetimeManagement
{
    /// <summary>
    /// Represents an object that can be managed throughout its lifetime.
    /// </summary>
    public interface ILifetimeable
    {
        #region Set up

        //void SetUp(); //Moved to ISetUppable

        bool IsSetUp { get; }

        #endregion

        #region Initialize

        //void Initialize(object[] args = null); //Moved to IInitializable

        bool IsInitialized { get; }

        Action OnInitialized { get; set; }

        #endregion

        #region Cleanup

        //void Cleanup(); //Moved to ICleanUppable

        Action OnCleanedUp { get; set; }

        #endregion

        #region Tear down

        //void TearDown(); //Moved to ITearDownable

        Action OnTornDown { get; set; }

        #endregion
    }
}