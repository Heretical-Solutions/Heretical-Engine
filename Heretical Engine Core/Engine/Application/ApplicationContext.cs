using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.Logging;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Application
{
	public class ApplicationContext
		: IApplicationContext,
		  ICompositionRoot
	{
		private readonly Stack<ILifetimeScope> scopeStack;

		private readonly List<Action<ContainerBuilder>> containerActions;

		private readonly List<IModule> modules;

		private ILogger logger;

		public ApplicationContext(
			ContainerBuilder containerBuilder,
			Stack<ILifetimeScope> scopeStack,
			List<Action<ContainerBuilder>> containerActions,
			List<IModule> modules)
		{
			ContainerBuilder = containerBuilder;

			this.scopeStack = scopeStack;

			this.containerActions = containerActions;

			this.modules = modules;

			logger = null;
		}

		#region IApplicationContext

		public IContainer Container { get; private set; }

		public ILifetimeScope CurrentScope { get => scopeStack.Peek(); }

		public IEnumerable<IModule> ActiveModules { get => modules; }

		#endregion

		#region ICompositionRoot

		public ContainerBuilder ContainerBuilder { get; private set; }

		public void BuildContainer()
		{
			if (ContainerBuilder == null)
				throw new Exception(
					logger.TryFormat<ApplicationContext>(
						"CONTAINER BUILDER IS NULL"));

			if (Container != null)
				throw new Exception(
					logger.TryFormat<ApplicationContext>(
						"CONTAINER IS ALREADY BUILT"));

			Container = ContainerBuilder.Build();

			var loggerResolver = Container.Resolve<ILoggerResolver>();

			logger = loggerResolver.GetLogger<ApplicationContext>();
		}

		public void AddPendingContainerAction(Action<ContainerBuilder> containerAction)
		{
			containerActions.Add(containerAction);
		}

		public void PushLifetimeScope()
		{
			if (Container == null)
				throw new Exception(
					logger.TryFormat<ApplicationContext>(
						"CONTAINER IS NOT BUILT"));

			if (scopeStack == null)
				throw new Exception(
					logger?.TryFormat<ApplicationContext>(
					"SCOPE STACK IS NULL"));

			var currentLifetimeContainerActions = containerActions.ToArray();

			var newScope = Container.BeginLifetimeScope(
				(currentContainerBuilder) =>
				{
					foreach (var action in currentLifetimeContainerActions)
					{
						action?.Invoke(currentContainerBuilder);
					}
				});

			scopeStack.Push(newScope);

			containerActions.Clear();
		}

		public void PopLifetimeScope()
		{
			if (scopeStack == null)
				throw new Exception(
					logger.TryFormat<ApplicationContext>(
						"SCOPE STACK IS NULL"));

			if (scopeStack.Count == 0)
				throw new Exception(
					logger.TryFormat<ApplicationContext>(
						"SCOPE STACK IS EMPTY"));

			scopeStack
				.Pop()
				.Dispose();
		}

		#endregion
	}
}