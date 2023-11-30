#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using System.Globalization;

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.ResourceManagement.Factories;
using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.HereticalEngine.Modules;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;

namespace HereticalSolutions.HereticalEngine.Samples
{
	public class Program
	{

		//TODO: https://github.com/dotnet/Silk.NET/discussions/534
		unsafe static void Main(string[] args)
		{
			//var program = new Program();

			var pathToExe = System.Reflection.Assembly.GetExecutingAssembly().Location;

			//TODO: change
			var applicationDataFolder = pathToExe.Substring(
				0,
				pathToExe.IndexOf("/bin/"));

			#region Logger initialization

			//Courtesy of https://stackoverflow.com/questions/114983/given-a-datetime-object-how-do-i-get-an-iso-8601-date-in-string-format
			//Read comments carefully

			// Prefer this, to avoid having to manually define a framework-provided format
			//string dateTimeNow = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);

			string dateTimeNow = DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture);

			//string dateTimeNow = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

			string logFileName = dateTimeNow;

			IFormatLogger logger = LoggersFactory.BuildDefaultLoggerWithFileDump(
				applicationDataFolder,
				$"Runtime logs/{logFileName}.log");

			#endregion

			var windowModule = new WindowModule();

			IModule[] modules = new IModule[]
			{
				windowModule,
				new OpenGLModule(),
				new MainCameraModule(),
				new OpenGLDrawTestMeshModule(),
				//new OpenGLDrawTestCubeModule(),
				//new OpenGLDrawTextureModule(),
				//new ImGuiModule()
			};

			foreach (var module in modules)
			{
				module.OnInitialized += () =>
				{
					logger?.Log<Program>(
						$"MODULE {module.GetType().Name} INITIALIZED");
				};

				module.OnCleanedUp += () =>
				{
					logger?.Log<Program>(
						$"MODULE {module.GetType().Name} CLEANED UP");
				};

				module.OnTornDown += () =>
				{
					logger?.Log<Program>(
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

			if (logger != null)
			{
				((IDumpable)logger).Dump();
			}
		}
	}
}