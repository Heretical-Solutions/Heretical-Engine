/*
using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.Logging;

using Silk.NET.Windowing;

using Silk.NET.Input;

using Autofac;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class WindowModule
		: ICoreModule
	{
		public const string WINDOW_RESOURCE_PATH = "Application/Window";

		public const string INPUT_CONTEXT_RESOURCE_PATH = "Application/Input context";

		public const string PRIMARY_KEYBOARD_RESOURCE_PATH = "Application/Primary keyboard";

		private readonly ContainerBuilder iocBuilder = null;

		private IWindow window = null;

		private IKeyboard primaryKeyboard = null;

		private IInputContext inputContext = null;

		private IFormatLogger logger = null;

		public WindowModule(
			ContainerBuilder iocBuilder)
		{
			this.iocBuilder = iocBuilder;
		}

		#region ICoreModule

		#region IModule

		public string Name => "Window module";

		#region IGenericLifetimeable<ApplicationContext>

		public void SetUp(
			ApplicationContext context)
		{
			if (IsSetUp)
				context.Logger?.ThrowException<WindowModule>(
					"ALREADY SET UP");

			var windowOptions = WindowOptions.Default;

			//FUCKING HELL!
			//I THOUGHT THAT WITH ENOUGH PATIENCE AND DEDICATION I CAN GET THIS ENTIRE THING TO WORK BY FOLLOWING TUTORIALS AND BE
			//THOROUGH AND CAREFUL. BUT THIS ONE IS OUTRIGHT RIDICULOUS. GUESS WHY MY SUZANNE WAS CLIPPING BOTH OF ITS EARS RIGHT
			//THROUGH ITS HEAD? APPARENTLY, BECAUSE IT SEEMS THAT
			//1. Silk.Net IS NOT ENABLING ITS DEPTH BUFFER BY DEFAULT. THAT WAS AN EASY FIX
			//2. Silk.Net IS NOT SETTING THE WINDOW'S DEPTH BUFFER BITS VALUE AND NOWHERE, NO-FUCKING-WHERE IS IT DOING SO IN THE
			//TUTORIALS THAT I CHECKED OUT. GOD BLESS THIS GUY https://old.reddit.com/r/opengl/comments/z5c4ge/please_help_depth_testing_not_working/ixxirpk/ FOR POINTING THIS OUT
			//3. Silk.Net HAS APPARENTLY FUCKED UP ITS OWN ENUMS OVER TIME SO I HAD SOME HARD TIME FIGURING OUT WHY CullFaceMode ENUM
			//DOES NOT EVEN EXIST
			//FIRST I WAS ANGRY FOR NOT PAYING ENOUGH ATTENTION AT SUCH TRIVIAL THINGS LIKE CASTING ARGUMENT VALUE TO PROPER TYPE TO
			//ENSURE THAT CORRECT OVERLOAD IS SELECTED BUT THIS ONE IS SOMETHING THAT I WOULD BE BEATING MY HEAD AGAINST AND STILL
			//NOT UNDERSTANDING WHAT DID I DO WRONG IF IT WAS NOT FOR SOME GOOD PEOPLE ON STACK OVERFLOW
			windowOptions.PreferredDepthBufferBits = 24;

			//Set up
			window = Window.Create(windowOptions);

			// Our loading function
			window.Load += () =>
			{
				for (int i = 0; i < context.Modules.Length; i++)
				{
					context.Modules[i].Initialize(context);
				}
			};

			window.Update += timeDelta =>
			{
				ProcessCommandsOnMainThread(context);

				for (int i = 0; i < context.Modules.Length; i++)
				{
					context.Modules[i].Update(
						context,
						(float)timeDelta);
				}
			};

			window.Render += timeDelta =>
			{
				for (int i = 0; i < context.Modules.Length; i++)
				{
					context.Modules[i].Draw(
						context,
						(float)timeDelta);
				}
			};

			// The closing function
			window.Closing += () =>
			{
				for (int i = 0; i < context.Modules.Length; i++)
				{
					context.Modules[i].Cleanup();
				}
			};


			var windowImporter = new DefaultPreallocatedAssetImporter<IWindow>(
				context);

			windowImporter.Initialize(
				WINDOW_RESOURCE_PATH,
				window);

			var task = windowImporter.Import();

			task.Wait();


			logger = context.Logger;

			IsSetUp = true;
		}

		private void ProcessCommandsOnMainThread(
			ApplicationContext context)
		{
			while (context.MainThreadCommandBuffer.TryConsume(out var command))
			{
				if (command.Async)
				{
					var task = command.ExecuteAsync();

					task.Wait();
				}
				else
					command.Execute();
			}
		}

		public bool IsSetUp { get; private set; } = false;

		public void Initialize(
			ApplicationContext context)
		{
			if (!IsSetUp)
			{
				context.Logger?.ThrowException<WindowModule>(
					"MODULE SHOULD BE SET UP BEFORE BEING INITIALIZED");
			}

			if (IsInitialized)
			{
				logger?.ThrowException<WindowModule>(
					"ATTEMPT TO INITIALIZE MODULE THAT IS ALREADY INITIALIZED");
			}

			inputContext = window.CreateInput();

			primaryKeyboard = inputContext.Keyboards.FirstOrDefault();


			var inputContextImporter = new DefaultPreallocatedAssetImporter<IInputContext>(
				context);

			inputContextImporter.Initialize(
				INPUT_CONTEXT_RESOURCE_PATH,
				inputContext);

			var task = inputContextImporter.Import();

			task.Wait();


			var primaryKeyboardImporter = new DefaultPreallocatedAssetImporter<IKeyboard>(
				context);

			primaryKeyboardImporter.Initialize(
				PRIMARY_KEYBOARD_RESOURCE_PATH,
				primaryKeyboard);

			task = primaryKeyboardImporter.Import();

			task.Wait();

			IsInitialized = true;

			OnInitialized?.Invoke();
		}

		public bool IsInitialized { get; private set; } = false;

		public Action OnInitialized { get; set; }

		public void Cleanup()
		{
			if (!IsInitialized)
				return;

			//Clean up
			// Dispose the input context
			inputContext?.Dispose();
			

			IsInitialized = false;

			OnCleanedUp?.Invoke();
		}

		public Action OnCleanedUp { get; set; }

		public void TearDown()
		{
			if (!IsSetUp)
				return;

			IsSetUp = false;

			Cleanup();

			//Tear down
			window.Dispose();

			window = null;

			primaryKeyboard = null;

			inputContext = null;

			logger = null;


			OnTornDown?.Invoke();

			OnInitialized = null;

			OnCleanedUp = null;

			OnTornDown = null;
		}

		public Action OnTornDown { get; set; }

		#endregion

		public void Update(
			ApplicationContext context,
			float timeDelta)
		{

		}

		public void Draw(
			ApplicationContext context,
			float timeDelta)
		{

		}

		#endregion

		public void Run(
			ApplicationContext context)
		{
			if (!IsSetUp)
			{
				context.Logger?.ThrowException<WindowModule>(
					"CORE MODULE SHOULD BE SET UP BEFORE BEING RUN");
			}

			// Now that everything's defined, let's run this bad boy!
			window.Run();
		}

		#endregion
	}
}
*/