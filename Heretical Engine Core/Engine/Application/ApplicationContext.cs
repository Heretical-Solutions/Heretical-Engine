using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Application
{
	public class ApplicationContext
		: IApplicationContext,
		  ICompositionRoot
	{
		private readonly Stack<ILifetimeScope> lifetimeScopeStack;

		private readonly List<Action<ContainerBuilder>> lifetimeScopeActions;

		private readonly List<IModule> activeModules;

		private ILifetimeable rootLifetime;

		private ILifetimeable currentLifetime;

		private ILogger logger;

		public ApplicationContext(
			ContainerBuilder containerBuilder,
			Stack<ILifetimeScope> lifetimeScopeStack,
			List<Action<ContainerBuilder>> lifetimeScopeActions,
			List<IModule> activeModules)
		{
			ContainerBuilder = containerBuilder;

			this.lifetimeScopeStack = lifetimeScopeStack;

			this.lifetimeScopeActions = lifetimeScopeActions;

			this.activeModules = activeModules;

			rootLifetime = null;

			currentLifetime = null;

			logger = null;
		}

		#region IApplicationContext

		public IContainer DIContainer { get; private set; }

		public ILifetimeScope CurrentLifetimeScope
		{
			get 
			{
				lifetimeScopeStack.TryPeek(out var result);

				return result;
			}
		}

		public ILifetimeable RootLifetime { get => rootLifetime; }

		public ILifetimeable CurrentLifetime { get => currentLifetime; }

		public IEnumerable<IModule> ActiveModules { get => activeModules; }

		#endregion

		#region ICompositionRoot

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

		public void NestLifetime(ILifetimeable lifetime)
		{
			//LifetimeSynchronizer.SyncLifetimes( //We don't want the module to initialize itself upon parent lifetime's initialization
			LifetimeSynchronizer.SyncEndOfLifetimes(
				lifetime,
				currentLifetime);
		}

		public void SetLifetimeAsCurrent(
			ILifetimeable lifetime,
			bool root = false)
		{
			currentLifetime = lifetime;

			if (root)
				rootLifetime = lifetime;
		}

		public void QueueLifetimeScopeAction(Action<ContainerBuilder> lifetimeScopeAction)
		{
			lifetimeScopeActions.Add(lifetimeScopeAction);
		}

		public void PushLifetimeScope()
		{
			if (DIContainer == null)
				throw new Exception(
					logger.TryFormat<ApplicationContext>(
						"CONTAINER IS NOT BUILT"));

			if (lifetimeScopeStack == null)
				throw new Exception(
					logger.TryFormat<ApplicationContext>(
					"SCOPE STACK IS NULL"));


			var currentLifetimeScopeActions = lifetimeScopeActions.ToArray();

			Action<ContainerBuilder> configurationAction =
				(currentContainerBuilder) =>
				{
					foreach (var action in currentLifetimeScopeActions)
					{
						action?.Invoke(currentContainerBuilder);
					}
				};


			ILifetimeScope currentLifetimeScope = CurrentLifetimeScope;

			ILifetimeScope newScope = null;

			if (currentLifetimeScope != null)
			{
				newScope = currentLifetimeScope.BeginLifetimeScope(configurationAction);
			}
			else
			{
				newScope = DIContainer.BeginLifetimeScope(configurationAction);
			}


			lifetimeScopeStack.Push(newScope);

			lifetimeScopeActions.Clear();
		}

		public void PopLifetimeScope()
		{
			if (lifetimeScopeStack == null)
				throw new Exception(
					logger.TryFormat<ApplicationContext>(
						"SCOPE STACK IS NULL"));

			if (lifetimeScopeStack.Count == 0)
				throw new Exception(
					logger.TryFormat<ApplicationContext>(
						"SCOPE STACK IS EMPTY"));

			lifetimeScopeStack
				.Pop()
				.Dispose();
		}

		public void AddActiveModule(IModule module)
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

		public void RemoveActiveModule(IModule module)
		{
			if (module == null)
				throw new Exception(
					logger.TryFormat<ApplicationContext>(
						"MODULE IS NULL"));

			if (!activeModules.Contains(module))
				return;

			activeModules.Remove(module);
		}
		
		public void LoadModule(IModule module)
		{
			module.Load(this);

			AddActiveModule(module);
		}

		public void UnloadModule(IModule module)
		{
			RemoveActiveModule(module);

			module.Unload(this);
		}

		#endregion
	}
}