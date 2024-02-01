using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.Logging;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Application
{
	public class ApplicationContext
		: IApplicationContext,
		  ICompositionRoot,
		  IRootLifetimeManager,
		  IApplicationStatusManager,
		  IModuleManager,
		  IActiveModuleRegistry
	{
		private readonly List<IModule> activeModules;

		private EApplicationStatus currentStatus;

		private ILifetimeModule rootLifetime;

		private ILogger logger;

		public ApplicationContext(
			ContainerBuilder containerBuilder,
			List<IModule> activeModules)
		{
			ContainerBuilder = containerBuilder;

			this.activeModules = activeModules;

			rootLifetime = null;

			logger = null;

			currentStatus = EApplicationStatus.UNINITIALIZED;
		}

		#region IApplicationContext

		public EApplicationStatus CurrentStatus { get => currentStatus; }

		public IEnumerable<IModule> ActiveModules { get => activeModules; }

		public ILifetimeModule RootLifetime { get => rootLifetime; }

		#endregion

		#region ICompositionRoot

		public IContainer DIContainer { get; private set; }

		public ContainerBuilder ContainerBuilder { get; private set; }

		public void BuildContainer()
		{
			if (ContainerBuilder == null)
				throw new Exception(
					logger.TryFormat<ApplicationContext>(
						"CONTAINER BUILDER IS NULL"));

			if (DIContainer != null)
				throw new Exception(
					logger.TryFormat<ApplicationContext>(
						"CONTAINER IS ALREADY BUILT"));

			DIContainer = ContainerBuilder.Build();


			var loggerResolver = DIContainer.Resolve<ILoggerResolver>();

			logger = loggerResolver.GetLogger<ApplicationContext>();
		}

		#endregion

		#region IRootLifetimeManager

		public void SetRootLifetime(ILifetimeModule rootLifetime)
		{
			this.rootLifetime = rootLifetime;
		}

		#endregion

		#region IApplicationStatusManager

		public void SetStatus(EApplicationStatus status)
		{
			currentStatus = status;
		}

		#endregion

		#region IModuleManager
		
		public void LoadModule(
			IModule module,
			ILifetimeModule parentLifetime)
		{
			module.Load(
				this,
				parentLifetime);
		}

		public void UnloadModule(
			IModule module)
		{
			module.Unload(this);
		}

		#endregion

		#region IActiveModuleRegistry

		public void RegisterActiveModule(IModule module)
		{
			if (module == null)
				throw new Exception(
					logger.TryFormat<ApplicationContext>(
						"MODULE IS NULL"));

			if (activeModules.Contains(module))
				throw new Exception(
					logger.TryFormat<ApplicationContext>(
						"MODULE IS ALREADY LOADED"));

			activeModules.Add(module);
		}

		public void UnregisterActiveModule(IModule module)
		{
			if (module == null)
				throw new Exception(
					logger.TryFormat<ApplicationContext>(
						"MODULE IS NULL"));

			if (!activeModules.Contains(module))
				return;

			activeModules.Remove(module);
		}

		#endregion
	}
}