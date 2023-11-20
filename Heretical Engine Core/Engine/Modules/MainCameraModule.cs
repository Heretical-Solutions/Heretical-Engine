using System.Numerics;

using HereticalSolutions.HereticalEngine.Math;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.HereticalEngine.Scenes;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Logging;

using Silk.NET.Maths;

using Silk.NET.Windowing;

using Silk.NET.Input;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class MainCameraModule
		: IModule
	{
		public const string MAIN_CAMERA_RESOURCE_PATH = "Application/Main camera";

		private IWindow window = null;

		private IKeyboard primaryKeyboard = null;

		private IInputContext inputContext = null;

		private Camera camera;

		private IResourceStorageHandle cameraStorageHandle;

		//Setup the camera's location, directions, and movement speed
		private Vector3D<float> cameraPosition = new Vector3D<float>(
			0.0f,
			0.0f,
			3.0f);

		private Vector3D<float> cameraFront = new Vector3D<float>(
			0.0f,
			0.0f,
			-1.0f);

		private static Vector3D<float> cameraUp = Vector3D<float>.UnitY;

		private static Vector3D<float> cameraDirection = Vector3D<float>.Zero;

		private float cameraYaw = -90f;

		private float cameraPitch = 0f;

		private float cameraZoom = 45f;

		//Used to track change in mouse movement to allow for moving of the Camera
		private Vector2D<float> lastMousePosition;

		private IFormatLogger logger;

		#region IModule

		#region IGenericLifetimeable<ApplicationContext>

		public void SetUp(
			ApplicationContext context)
		{
			if (IsSetUp)
				context.Logger?.ThrowException<MainCameraModule>(
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
				context.Logger?.ThrowException<MainCameraModule>(
					"MODULE SHOULD BE SET UP BEFORE BEING INITIALIZED");
			}

			if (IsInitialized)
			{
				logger?.ThrowException<MainCameraModule>(
					"ATTEMPT TO INITIALIZE MODULE THAT IS ALREADY INITIALIZED");
			}

			//Initialization

			var windowStorageHandle = context.RuntimeResourceManager
				.GetDefaultResource(
					WindowModule.WINDOW_RESOURCE_PATH.SplitAddressBySeparator())
				.StorageHandle;

			Task task;

			if (!windowStorageHandle.Allocated)
			{
				task = windowStorageHandle.Allocate();

				task.Wait();
			}

			window = windowStorageHandle.GetResource<IWindow>();



			var primaryKeyboardStorageHandle = context.RuntimeResourceManager
				.GetDefaultResource(
					WindowModule.PRIMARY_KEYBOARD_RESOURCE_PATH.SplitAddressBySeparator())
				.StorageHandle;

			if (!primaryKeyboardStorageHandle.Allocated)
			{
				task = primaryKeyboardStorageHandle.Allocate();

				task.Wait();
			}

			primaryKeyboard = primaryKeyboardStorageHandle.GetResource<IKeyboard>();


			var inputContextStorageHandle = context.RuntimeResourceManager
				.GetDefaultResource(
					WindowModule.INPUT_CONTEXT_RESOURCE_PATH.SplitAddressBySeparator())
				.StorageHandle;

			if (!inputContextStorageHandle.Allocated)
			{
				task = inputContextStorageHandle.Allocate();

				task.Wait();
			}

			inputContext = inputContextStorageHandle.GetResource<IInputContext>();


			if (primaryKeyboard != null)
			{
				primaryKeyboard.KeyDown += KeyDown;
			}

			for (int i = 0; i < inputContext.Mice.Count; i++)
			{
				//inputContext.Mice[i].Cursor.CursorMode = CursorMode.Raw; //BASTARD! I WAS WONDERING WHY THE FUCK WAS THE CURSOR GONE BOTH FROM THE WINDOW AND FROM THE IDE EVERY TIME I START THE APPLICATION, MAKING IT FUCKING IMPOSSIBLE TO DEBUG OR SIMPLY CLOSE THE WINDOW WITH THE MOUSE. BE ADVISED: COPY THE CODE FROM TUTORIALS CAREFULLY

				inputContext.Mice[i].MouseMove += OnMouseMove;

				inputContext.Mice[i].Scroll += OnMouseWheel;
			}

			camera = default;

			var cameraImporter = new DefaultReadWriteAssetImporter<Camera>(
				MAIN_CAMERA_RESOURCE_PATH,
				camera,
				context);

			cameraStorageHandle = (IResourceStorageHandle)
				Task.Run(
					() => cameraImporter.Import())
					.Result
					.StorageHandle;


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
			if (primaryKeyboard != null)
			{
				primaryKeyboard.KeyDown -= KeyDown;
			}

			if (inputContext != null)
			{
				for (int i = 0; i < inputContext.Mice.Count; i++)
				{
					inputContext.Mice[i].MouseMove -= OnMouseMove;

					inputContext.Mice[i].Scroll -= OnMouseWheel;
				}
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

			primaryKeyboard = null;

			inputContext = null;

			cameraStorageHandle = null;


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
			var moveSpeed = 2.5f * timeDelta;

			if (primaryKeyboard.IsKeyPressed(Key.W))
			{
				//Move forwards
				cameraPosition += moveSpeed * cameraFront;
			}
			if (primaryKeyboard.IsKeyPressed(Key.S))
			{
				//Move backwards
				cameraPosition -= moveSpeed * cameraFront;
			}
			if (primaryKeyboard.IsKeyPressed(Key.A))
			{
				//Move left
				cameraPosition -= Vector3D.Normalize(
					Vector3D.Cross(
						cameraFront,
						cameraUp))
					* moveSpeed;
			}
			if (primaryKeyboard.IsKeyPressed(Key.D))
			{
				//Move right
				cameraPosition += Vector3D.Normalize(
					Vector3D.Cross(
						cameraFront,
						cameraUp))
					* moveSpeed;
			}
		}

		public void Draw(
			ApplicationContext context,
			float timeDelta)
		{
			camera.ViewMatrix = Matrix4X4.CreateLookAt(
				cameraPosition,
				cameraPosition + cameraFront,
				cameraUp);

			//It's super important for the width / height calculation to regard each value as a float, otherwise
			//it creates rounding errors that result in viewport distortion
			camera.ProjectionMatrix = Matrix4X4.CreatePerspectiveFieldOfView(
				MathHelpers.DegreesToRadians(cameraZoom),
				(float)window.Size.X / (float)window.Size.Y,
				0.1f,
				1000.0f);

			cameraStorageHandle.SetResource<Camera>(camera);
		}

		#endregion

		private void OnMouseMove(
			IMouse mouse,
			Vector2 positionInput)
		{
			Vector2D<float> position = positionInput.ToSilkNetVector2D();

			var lookSensitivity = 0.1f;

			if (lastMousePosition == default)
			{
				lastMousePosition = position;
			}
			else
			{
				var xOffset = (position.X - lastMousePosition.X) * lookSensitivity;

				var yOffset = (position.Y - lastMousePosition.Y) * lookSensitivity;


				lastMousePosition = position;


				cameraYaw += xOffset;

				cameraPitch -= yOffset;

				//We don't want to be able to look behind us by going over our head or under our feet so make sure it stays within these bounds
				cameraPitch = float.Clamp(
					cameraPitch,
					-89.0f,
					89.0f);

				cameraDirection.X =
					MathF.Cos(MathHelpers.DegreesToRadians(cameraYaw))
					* MathF.Cos(MathHelpers.DegreesToRadians(cameraPitch));

				cameraDirection.Y = MathF.Sin(MathHelpers.DegreesToRadians(cameraPitch));

				cameraDirection.Z =
					MathF.Sin(MathHelpers.DegreesToRadians(cameraYaw))
					* MathF.Cos(MathHelpers.DegreesToRadians(cameraPitch));

				cameraFront = Vector3D.Normalize(cameraDirection);
			}
		}

		private void OnMouseWheel(
			IMouse mouse,
			ScrollWheel scrollWheel)
		{
			//We don't want to be able to zoom in too close or too far away so clamp to these values
			cameraZoom = float.Clamp(
				cameraZoom - scrollWheel.Y,
				1.0f,
				45f);
		}

		private void KeyDown(
			IKeyboard keyboard,
			Key key,
			int arg3)
		{
			if (key == Key.Escape)
			{
				window.Close();
			}
		}
	}
}