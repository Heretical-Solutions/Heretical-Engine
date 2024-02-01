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

		protected IActiveModuleRegistry activeModuleRegistry;

		protected ILifetimeModule parentLifetime;

		protected ILogger logger;

		#region IModule

		public virtual string Name => "Abstract lifetimeable module";

		public virtual void Load(
			IApplicationContext context,
			ILifetimeModule parentLifetime)
		{
			this.context = context;

			activeModuleRegistry = context as IActiveModuleRegistry;

			this.parentLifetime = parentLifetime;


			TryResolveLogger(parentLifetime as ILifetimeScopeContainer);

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


			LifetimeSynchronizer.SyncEndOfLifetimes(
				this,
				parentLifetime);

			InitializeInternal();


			IsSetUp = true;

			IsInitialized = true;

			logger?.Log(
				GetType(),
				$"MODULE INITIALIZED");

			OnInitialized?.Invoke();
		}

		public virtual void Unload(
			IApplicationContext context)
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

		public virtual void TearDown()
		{
			if (!IsSetUp)
				return;

			if (!IsInitialized)
				return;

			IsSetUp = false;

			IsInitialized = false;


			//Cleanup
			CleanupInternal();

			logger?.Log(
				GetType(),
				$"MODULE DEINITIALIZED");

			logger = null;


			OnCleanedUp?.Invoke();

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

		protected virtual void InitializeInternal()
		{
			activeModuleRegistry.RegisterActiveModule(this);
		}

		protected virtual void CleanupInternal()
		{
			//This one IS needed. Module could be unloaded in two ways: by calling Unload and by parent lifetime tear down.
			//In both cases we need to ensure that module is unlisted from active ones without creating recursive calls
			activeModuleRegistry.UnregisterActiveModule(this);

			parentLifetime = null;

			activeModuleRegistry = null;

			context = null;
		}

		//Remember: we're trying to obtain a logger WITHOUT hooking up to the parent YET
		protected void TryResolveLogger(ILifetimeScopeContainer lifetimeScopeContainer)
		{
			if (lifetimeScopeContainer != null
				&& lifetimeScopeContainer.CurrentLifetimeScope != null)
			{
				if (lifetimeScopeContainer
					.CurrentLifetimeScope
					.TryResolve<ILoggerResolver>(
						out var resolver))
				{
					logger = resolver.GetLogger(GetType());
				}
			}
			else
			{
				var compositionRoot = context as ICompositionRoot;

				if (compositionRoot == null)
					return;

				if (compositionRoot.DIContainer == null)
					return;

				if (compositionRoot
					.DIContainer
					.TryResolve<ILoggerResolver>(
						out var resolver))
				{
					logger = resolver.GetLogger(GetType());
				}
			}
		}
	}
}