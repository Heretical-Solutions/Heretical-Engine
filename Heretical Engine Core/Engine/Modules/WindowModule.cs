using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.Logging;

using Silk.NET.Windowing;

using Silk.NET.Input;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class WindowModule
		: ICoreModule
	{
		private IWindow window = null;

		private IKeyboard primaryKeyboard = null;

		private IInputContext inputContext = null;

		private IFormatLogger logger = null;

		#region ICoreModule

		#region IModule

		#region IGenericLifetimeable<ApplicationContext>

		public void SetUp(
			ApplicationContext context)
		{
			if (IsSetUp)
				context.Logger.ThrowException<WindowModule>(
					"ALREADY SET UP");

			//Set up
			window = Window.Create(WindowOptions.Default);

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
				context.RuntimeResourceManager,
				"Application/Window",
				window,
				context.Logger);

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
				context.Logger.ThrowException<WindowModule>(
					"MODULE SHOULD BE SET UP BEFORE BEING INITIALIZED");
			}

			if (IsInitialized)
			{
				logger.ThrowException<WindowModule>(
					"ATTEMPT TO INITIALIZE MODULE THAT IS ALREADY INITIALIZED");
			}

			inputContext = window.CreateInput();

			primaryKeyboard = inputContext.Keyboards.FirstOrDefault();


			var inputContextImporter = new DefaultPreallocatedAssetImporter<IInputContext>(
				context.RuntimeResourceManager,
				"Application/Input context",
				inputContext,
				context.Logger);

			var task = inputContextImporter.Import();

			task.Wait();


			var primaryKeyboardImporter = new DefaultPreallocatedAssetImporter<IKeyboard>(
				context.RuntimeResourceManager,
				"Application/Primary keyboard",
				primaryKeyboard,
				context.Logger);

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
				context.Logger.ThrowException<WindowModule>(
					"CORE MODULE SHOULD BE SET UP BEFORE BEING RUN");
			}

			// Now that everything's defined, let's run this bad boy!
			window.Run();

			/*
			for (int i = 0; i < context.Modules.Length; i++)
			{
				context.Modules[i].TearDown();
			}
			*/
		}

		#endregion
	}
}