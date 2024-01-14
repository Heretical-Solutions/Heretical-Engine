using HereticalSolutions.HereticalEngine.Modules;

using Autofac;
using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Application
{
	public class ApplicationContext
	{
		/*
		//TODO: replace with DI injections
		public IRuntimeResourceManager RuntimeResourceManager { get; private set; }

		//TODO: replace with DI injections
		public ConcurrentGenericCircularBuffer<MainThreadCommand> MainThreadCommandBuffer { get; private set; }

		//TODO: replace with DI injections
		public IFormatLogger Logger { get; private set; }
		*/

		private readonly List<IModule> modules;

		private readonly Stack<ILifetimeScope> scopeStack;

		private readonly IFormatLogger logger;

		public ApplicationContext(
			ContainerBuilder containerBuilder,
			Stack<ILifetimeScope> scopeStack,
			List<IModule> modules,
			IFormatLogger logger = null)
		{
			ContainerBuilder = containerBuilder;

			this.scopeStack = scopeStack;

			this.modules = modules;

			this.logger = logger;
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
				logger?.ThrowException<ApplicationContext>(
					"CONTAINER BUILDER IS NULL");

			if (Container != null)
				logger?.ThrowException<ApplicationContext>(
					"CONTAINER IS ALREADY BUILT");

			Container = ContainerBuilder.Build();
		}

		public void PushLifetimeScope(
			Action<ContainerBuilder> configurationAction)
		{
			if (Container == null)
				logger?.ThrowException<ApplicationContext>(
					"CONTAINER IS NOT BUILT");

			if (scopeStack == null)
				logger?.ThrowException<ApplicationContext>(
					"SCOPE STACK IS NULL");

			var newScope = Container.BeginLifetimeScope(configurationAction);

			scopeStack.Push(newScope);
		}

		public void PopLifetimeScope()
		{
			if (scopeStack == null)
				logger?.ThrowException<ApplicationContext>(
					"SCOPE STACK IS NULL");

			if (scopeStack.Count == 0)
				logger?.ThrowException<ApplicationContext>(
					"SCOPE STACK IS EMPTY");

			scopeStack
				.Pop()
				.Dispose();
		}

		#endregion
	}
}