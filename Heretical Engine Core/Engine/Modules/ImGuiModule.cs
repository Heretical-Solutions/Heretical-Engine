using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.Logging;

using Silk.NET.Windowing;

using Silk.NET.Input;

using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class ImGuiModule
		: IModule
	{
		private IWindow window = null;

		private IInputContext inputContext = null;

		private GL gl = null;

		private ImGuiController controller = null;

		private IFormatLogger logger = null;

		#region IModule

		#region IGenericLifetimeable<ApplicationContext>

		public void SetUp(
			ApplicationContext context)
		{
			if (IsSetUp)
				context.Logger.ThrowException<ImGuiModule>(
					"ALREADY SET UP");

			//Set up
			logger = context.Logger;

			IsSetUp = true;
		}

		public bool IsSetUp { get; private set; } = false;

		public void Initialize(
			ApplicationContext context)
		{
			if (!IsSetUp)
			{
				context.Logger.ThrowException<ImGuiModule>(
					"MODULE SHOULD BE SET UP BEFORE BEING INITIALIZED");
			}

			if (IsInitialized)
			{
				logger.ThrowException<ImGuiModule>(
					"ATTEMPT TO INITIALIZE MODULE THAT IS ALREADY INITIALIZED");
			}

			//Initialization
			var windowStorageHandle = context.RuntimeResourceManager
							.GetDefaultResource(
								"Application/Window".SplitAddressBySeparator())
							.StorageHandle;

			Task task;

			if (!windowStorageHandle.Allocated)
			{
				task = windowStorageHandle.Allocate();

				task.Wait();
			}

			window = windowStorageHandle.GetResource<IWindow>();


			var inputContextStorageHandle = context.RuntimeResourceManager
				.GetDefaultResource(
					"Application/Input context".SplitAddressBySeparator())
				.StorageHandle;

			if (!inputContextStorageHandle.Allocated)
			{
				task = inputContextStorageHandle.Allocate();

				task.Wait();
			}

			inputContext = inputContextStorageHandle.GetResource<IInputContext>();


			var glStorageHandle = context.RuntimeResourceManager
				.GetDefaultResource(
					"Application/GL".SplitAddressBySeparator())
				.StorageHandle;

			if (!glStorageHandle.Allocated)
			{
				task = glStorageHandle.Allocate();

				task.Wait();
			}

			gl = glStorageHandle.GetResource<GL>();

			controller = new ImGuiController(
				gl, // load OpenGL
				window, // pass in our window
				inputContext // create an input context
			);


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
			
			// Dispose our controller first
			controller?.Dispose();


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
			window = null;

			inputContext = null;

			gl = null;

			controller = null;
			

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
			controller.Update(timeDelta);
		}

		public void Draw(
			ApplicationContext context,
			float timeDelta)
		{
			// This is where you'll do all of your ImGUi rendering
			// Here, we're just showing the ImGui built-in demo window.
			ImGuiNET.ImGui.ShowDemoWindow();

			// Make sure ImGui renders too!
			controller.Render();
		}

		#endregion
	}
}