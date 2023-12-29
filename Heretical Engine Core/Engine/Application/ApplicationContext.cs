using HereticalSolutions.Collections.Managed;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Application
{
	public class ApplicationContext
	{
		//Modules
		public IModule[] Modules { get; private set; }

		//TODO: remove
		public ICoreModule RootModule { get; private set; }

		//TODO: replace with DI injections
		public IRuntimeResourceManager RuntimeResourceManager { get; private set; }

		//TODO: replace with DI injections
		public ConcurrentGenericCircularBuffer<MainThreadCommand> MainThreadCommandBuffer { get; private set; }

		//TODO: replace with DI injections
		public IFormatLogger Logger { get; private set; }

		public ApplicationContext(
			IModule[] modules,
			ICoreModule rootModule,
			IRuntimeResourceManager runtimeResourceManager,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
		{
			Modules = modules;

			RootModule = rootModule;


			RuntimeResourceManager = runtimeResourceManager;


			MainThreadCommandBuffer = mainThreadCommandBuffer;

			Logger = logger;
		}
	}
}