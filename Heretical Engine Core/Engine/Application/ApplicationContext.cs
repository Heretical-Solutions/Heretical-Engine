using HereticalSolutions.HereticalEngine.Modules;

using Autofac;

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

		private List<IModule> modules;

		public ApplicationContext(
			ContainerBuilder containerBuilder,
			List<IModule> modules)
		{
			ContainerBuilder = containerBuilder;

			this.modules = modules;
		}

		#region IApplicationContext

		public IContainer Container { get; private set; }

		public IEnumerable<IModule> ActiveModules { get => modules; }

		#endregion

		#region ICompositionRoot

		public ContainerBuilder ContainerBuilder { get; private set; }

		public void BuildContainer()
		{

		}

		#endregion
	}
}