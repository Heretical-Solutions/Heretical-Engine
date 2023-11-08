using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.Logging;

using Silk.NET.Windowing;

using Silk.NET.OpenGL;

using Silk.NET.Maths;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class OpenGLModule
		: IModule
	{
		private IWindow window = null;

		private GL gl = null;

		private IFormatLogger logger = null;

		#region IModule

		#region IGenericLifetimeable<ApplicationContext>

		public void SetUp(
			ApplicationContext context)
		{
			if (IsSetUp)
				context.Logger.ThrowException<OpenGLModule>(
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
				context.Logger.ThrowException<OpenGLModule>(
					"MODULE SHOULD BE SET UP BEFORE BEING INITIALIZED");
			}

			if (IsInitialized)
			{
				logger.ThrowException<OpenGLModule>(
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


			gl = window.CreateOpenGL();

			var glImporter = new DefaultPreallocatedAssetImporter<GL>(
				context.RuntimeResourceManager,
				"Application/GL",
				gl,
				context.Logger);

			task = glImporter.Import();

			task.Wait();


			window.FramebufferResize += Resize;


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
			// Unload OpenGL
			gl?.Dispose();

			if (window != null)
			{
				window.FramebufferResize -= Resize;
			}


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

			gl = null;

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
			Clear(gl);
		}

		private void Clear(
			GL gl)
		{
			// Here, we just have a blank screen.
			gl.ClearColor(
				System.Drawing.Color.FromArgb(
					255,
					(int)(.45f * 255),
					(int)(.55f * 255),
					(int)(.60f * 255)));

			gl.Clear(
				(uint)ClearBufferMask.ColorBufferBit);
		}

		#endregion

		private void Resize(
			Vector2D<int> newSize)
		{
			if (gl == null)
				return;

			// Adjust the viewport to the new window size
			gl.Viewport(newSize);
		}
	}
}