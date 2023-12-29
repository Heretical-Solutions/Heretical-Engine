using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.Logging;

using Silk.NET.Windowing;

using Silk.NET.Input;

using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class GizmoModule //WIP
		: IModule
	{
		private GL gl = null;

		private IFormatLogger logger = null;

		#region IModule

		public string Name => "Gizmo module";

		#region IGenericLifetimeable<ApplicationContext>

		public void SetUp(
			ApplicationContext context)
		{
			if (IsSetUp)
				context.Logger?.ThrowException<GizmoModule>(
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
				context.Logger?.ThrowException<GizmoModule>(
					"MODULE SHOULD BE SET UP BEFORE BEING INITIALIZED");
			}

			if (IsInitialized)
			{
				logger?.ThrowException<GizmoModule>(
					"ATTEMPT TO INITIALIZE MODULE THAT IS ALREADY INITIALIZED");
			}

			//Initialization
			var glStorageHandle = context.RuntimeResourceManager
				.GetDefaultResource(
					OpenGLModule.GL_RESOURCE_PATH.SplitAddressBySeparator())
				.StorageHandle;

			Task task;

			if (!glStorageHandle.Allocated)
			{
				task = glStorageHandle.Allocate();

				task.Wait();
			}

			gl = glStorageHandle.GetResource<GL>();


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
		}

		private void DrawAxis(GL gl)
		{
		}

		#endregion
	}
}