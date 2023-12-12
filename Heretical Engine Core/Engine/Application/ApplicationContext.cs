using HereticalSolutions.Collections.Managed;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.Logging;
using HereticalSolutions.HereticalEngine.AssetImport;

namespace HereticalSolutions.HereticalEngine.Application
{
	public class ApplicationContext
	{
		//Modules
		public IModule[] Modules { get; private set; }

		public ICoreModule RootModule { get; private set; }


		public IRuntimeResourceManager RuntimeResourceManager { get; private set; }

		public IAssetImportManager AssetImportManager { get; private set; }
		

		public ConcurrentGenericCircularBuffer<MainThreadCommand> MainThreadCommandBuffer { get; private set; }

		public IFormatLogger Logger { get; private set; }

		public ApplicationContext(
			IModule[] modules,
			ICoreModule rootModule,
			IRuntimeResourceManager runtimeResourceManager,
			IAssetImportManager assetImportManager,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
		{
			Modules = modules;

			RootModule = rootModule;


			RuntimeResourceManager = runtimeResourceManager;

			AssetImportManager = assetImportManager;


			MainThreadCommandBuffer = mainThreadCommandBuffer;

			Logger = logger;
		}
	}
}