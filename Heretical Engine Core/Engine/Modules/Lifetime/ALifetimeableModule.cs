using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public abstract class ALifetimeableModule
		: IModule,
		  ILifetimeable,
		  ITearDownable,
		  IDisposable
	{
		protected IApplicationContext context;

		protected ILogger logger;

		#region IModule

		public virtual string Name => "Abstract lifetimeable module";

		public virtual void Load(IApplicationContext context)
		{
			this.context = context;

			if (context
				.DIContainer
				.TryResolve<ILoggerResolver>(
					out var resolver))
			{
				logger = resolver.GetLogger(GetType());
			}

			if (IsSetUp)
				throw new Exception(
					logger.TryFormat(
						GetType(),
						"MODULE ALREADY SET UP"));

			if (IsInitialized)
			{
				throw new Exception(
					logger.TryFormat(
						GetType(),
						"ATTEMPT TO INITIALIZE MODULE THAT IS ALREADY INITIALIZED"));
			}

			//This should be placed here because InitializeInternal may call compositionRoot.SetLifetimeAsCurrent(this); that will cause the module to replace the current lifetime scope. We don't want it to subscribe its lifetime to itself now do we?
			((ICompositionRoot)context).NestLifetime(this);

			InitializeInternal();

			IsSetUp = true;

			IsInitialized = true;

			OnInitialized?.Invoke();

			logger?.Log(
				GetType(),
				$"MODULE INITIALIZED");
		}

		public virtual void Unload(IApplicationContext context)
		{
			TearDown();
		}

		#endregion

		#region ILifetimeable

		public bool IsSetUp { get; private set; } = false;

		public bool IsInitialized { get; private set; } = false;

		public Action OnInitialized { get; set; }

		public Action OnCleanedUp { get; set; }

		public Action OnTornDown { get; set; }

		#endregion

		#region ITearDownable

		public void TearDown()
		{
			if (!IsSetUp)
				return;

			if (!IsInitialized)
				return;

			IsSetUp = false;

			IsInitialized = false;


			//Cleanup
			CleanupInternal();


			OnCleanedUp?.Invoke();

			OnTornDown?.Invoke();


			OnInitialized = null;

			OnCleanedUp = null;

			OnTornDown = null;


			logger?.Log(
				GetType(),
				$"MODULE TORN DOWN");

			logger = null;

			context = null;
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			TearDown();
		}

		#endregion

		protected virtual void InitializeInternal()
		{

		}

		protected virtual void CleanupInternal()
		{
			//This one IS needed. Module could be unloaded in two ways: by calling Unload and by parent lifetime tear down.
			//In both cases we need to ensure that module is unlisted from active ones without creating recursive calls
			((ICompositionRoot)context).RemoveActiveModule(this);
		}
	}
}