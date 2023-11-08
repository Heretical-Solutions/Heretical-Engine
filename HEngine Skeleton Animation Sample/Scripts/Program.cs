﻿#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.ResourceManagement.Factories;
using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Samples
{
	public class Program
	{

		//TODO: https://github.com/dotnet/Silk.NET/discussions/534
		unsafe static void Main(string[] args)
		{
			//var program = new Program();

			IFormatLogger logger = new ConsoleLogger();

			var windowModule = new WindowModule();

			IModule[] modules = new IModule[]
			{
				windowModule,
				new OpenGLModule(),
				new MainCameraModule(),
				//new OpenGLDrawTestCubeModule(),
				new OpenGLDrawTestMeshModule(),
				//new ImGuiModule()
			};

			foreach (var module in modules)
			{
				module.OnInitialized += () =>
				{
					logger.Log<Program>(
						$"MODULE {module.GetType().Name} INITIALIZED");
				};

				module.OnCleanedUp += () =>
				{
					logger.Log<Program>(
						$"MODULE {module.GetType().Name} CLEANED UP");
				};

				module.OnTornDown += () =>
				{
					logger.Log<Program>(
						$"MODULE {module.GetType().Name} TORN DOWN");
				};
			}

#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
			IRuntimeResourceManager runtimeResourceManager = ResourceManagementFactory.BuildConcurrentRuntimeResourceManager(logger);
#else
			IRuntimeResourceManager runtimeResourceManager = ResourceManagementFactory.BuildRuntimeResourceManager(logger);
#endif

			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer =
				new ConcurrentGenericCircularBuffer<MainThreadCommand>(
					new MainThreadCommand[1024],
					new int[1024]);

			ApplicationContext context = new ApplicationContext(
				modules,
				(ICoreModule)windowModule,
				runtimeResourceManager,
				mainThreadCommandBuffer,
				logger);

			for (int i = 0; i < context.Modules.Length; i++)
			{
				context.Modules[i].SetUp(context);
			}

			context.RootModule.Run(context);

			for (int i = 0; i < context.Modules.Length; i++)
			{
				context.Modules[i].TearDown();
			}
		}
	}
}