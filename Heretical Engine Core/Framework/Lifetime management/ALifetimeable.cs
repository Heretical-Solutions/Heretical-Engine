using HereticalSolutions.Logging;

namespace HereticalSolutions.LifetimeManagement
{
	public abstract class ALifetimeable
		: ILifetimeable,
		  ISetUppable,
		  IInitializable,
		  ICleanUppable,
		  ITearDownable,
		  IDisposable
	{
		protected readonly ILogger logger;

		public ALifetimeable(
			ILogger logger = null)
		{
			this.logger = logger;
		}

		#region ILifetimeable

		public bool IsSetUp { get; private set; } = false;

		public bool IsInitialized { get; private set; } = false;

		public Action OnInitialized { get; set; }

		public Action OnCleanedUp { get; set; }

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

			//Set up
			SetUpInternal();

			IsSetUp = true;
		}

		#endregion

		#region IInitializable

		public virtual void Initialize(object[] args = null)
		{
			if (!IsSetUp)
			{
				throw new Exception(
					logger.TryFormat(
						GetType(),
						"LIFETIMEABLE SHOULD BE SET UP BEFORE BEING INITIALIZED"));
			}

			if (IsInitialized)
			{
				throw new Exception(
					logger.TryFormat(
						GetType(),
						"ATTEMPT TO INITIALIZE LIFETIMEABLE THAT IS ALREADY INITIALIZED"));
			}

			//Initialization
			InitializeInternal(args);

			IsInitialized = true;

			OnInitialized?.Invoke();
		}

		#endregion

		#region ICleanUppable

		public virtual void Cleanup()
		{
			if (!IsInitialized)
				return;

			//Clean up
			CleanupInternal();

			IsInitialized = false;

			OnCleanedUp?.Invoke();
		}

		#endregion

		#region ITearDownable

		public void TearDown()
		{
			if (!IsSetUp)
				return;

			IsSetUp = false;

			Cleanup();

			//Tear down
			TearDownInternal();


			OnTornDown?.Invoke();

			OnInitialized = null;

			OnCleanedUp = null;

			OnTornDown = null;
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			TearDown();
		}

		#endregion

		protected virtual void SetUpInternal()
		{

		}

		protected virtual void InitializeInternal(object[] args = null)
		{

		}

		protected virtual void CleanupInternal()
		{

		}

		protected virtual void TearDownInternal()
		{

		}
	}
}