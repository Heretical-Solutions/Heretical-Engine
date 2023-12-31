using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.Persistence.IO;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.GameEntities;
using HereticalSolutions.GameEntities.Factories;

using HereticalSolutions.HereticalEngine.Rendering;

using HereticalSolutions.Logging;

using DefaultEcs;

using Silk.NET.Maths;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class TempModule
		: IModule
	{
		private IFormatLogger logger = null;

		#region IModule

		public string Name => "Temp module";

		#region IGenericLifetimeable<ApplicationContext>

		public void SetUp(
			ApplicationContext context)
		{
			if (IsSetUp)
				context.Logger?.ThrowException<TempModule>(
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
				context.Logger?.ThrowException<TempModule>(
					"MODULE SHOULD BE SET UP BEFORE BEING INITIALIZED");
			}

			if (IsInitialized)
			{
				logger?.ThrowException<TempModule>(
					"ATTEMPT TO INITIALIZE MODULE THAT IS ALREADY INITIALIZED");
			}

			//Initialization
			CreateCameraEntityAndSerializeIntJson(context);

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

		private void CreateCameraEntityAndSerializeIntJson(
			ApplicationContext context)
		{
			Vector3D<float> cameraPosition = new Vector3D<float>(
				0.0f,
				0.0f,
				3.0f);

			Vector3D<float> cameraFront = new Vector3D<float>(
				0.0f,
				0.0f,
				-1.0f);

			Vector3D<float> cameraUp = Vector3D<float>.UnitY;

			Vector3D<float> cameraDirection = Vector3D<float>.Zero;

			float cameraYaw = -90f;

			float cameraPitch = 0f;

			float cameraZoom = 45f;

			float fov = MathHelpers.DegreesToRadians(cameraZoom);

			//TODO: change
			float aspectRatio = (float)1280 / (float)720;

			float nearClippingPlane = 0.1f;

			float farClippingPlane = 1000.0f;

			var viewMatrix = Matrix4X4.CreateLookAt(
				cameraPosition,
				cameraPosition + cameraFront,
				cameraUp);

			var projectionMatrix = Matrix4X4.CreatePerspectiveFieldOfView(
				fov,
				aspectRatio,
				nearClippingPlane,
				farClippingPlane);

			var transform = new Transform
			{
				Position = cameraPosition,
				Rotation = new Quaternion<float>( //TODO: ENSURE THAT THIS IS CORRECT
					new Vector3D<float>(
						MathHelpers.DegreesToRadians(cameraPitch),
						MathHelpers.DegreesToRadians(cameraYaw),
						0.0f),
					0.0f),
				Scale = Vector3D<float>.One
			};

			var trsMatrix = transform.TRSMatrix;

			var prototypeRepository = EntitiesFactory.BuildPrototypesRepository();

			prototypeRepository.TryAllocatePrototype(
				"Camera",
				out Entity cameraEntity);

			cameraEntity.Set<HierarchyComponent>( //a must
				new HierarchyComponent
				{
					Parent = default,

					ChildrenListID = -1
				});
			cameraEntity.Set<TransformComponent>( //a must
				new TransformComponent
				{
					Transform = transform,

					TRSMatrix = trsMatrix
				});
			cameraEntity.Set<WorldTransformComponent>( //a must
				new WorldTransformComponent
				{
					Transform = transform,

					TRSMatrix = trsMatrix
				});
			cameraEntity.Set<CameraPoseComponent>(
				new CameraPoseComponent
				{
					Pose = new CameraPose
					{
						ViewMatrix = viewMatrix,
						
						ProjectionMatrix = projectionMatrix
					}
				}
			);
			cameraEntity.Set<CameraComponent>(
				new CameraComponent
				{
					Camera = new Camera
					{
						FOV = fov,
						AspectRatio = aspectRatio,
						NearPlane = nearClippingPlane,
						FarPlane = farClippingPlane
					}
				});

			var visitor = new EntityPrototypeVisitor(
				prototypeRepository,
				context.Logger);

			visitor.Save(cameraEntity, out EntityPrototypeDTO DTO);

			var pathToExe = System.Reflection.Assembly.GetExecutingAssembly().Location;

			//TODO: change
			var pathToAssets = pathToExe.Substring(
				0,
				pathToExe.IndexOf("/bin/"))
				+ "/Assets/";

			var pathToEngineAssets = pathToExe.Substring(
				0,
				pathToExe.IndexOf("/bin/"))
				+ "/Engine assets/";

			var serializer = PersistenceFactory.BuildSimpleJSONSerializer(context.Logger);

			var serializationArgument = new TextFileArgument();

			serializationArgument.Settings = new FilePathSettings
			{
				RelativePath = $"Entities/Camera template.json",
				
				ApplicationDataFolder = pathToEngineAssets
			};

			serializer.Serialize<EntityPrototypeDTO>(
				serializationArgument,
				DTO);

			logger?.Log<TempModule>(
				$"Camera entity serialized to {serializationArgument.Settings.ApplicationDataFolder + serializationArgument.Settings.RelativePath}");
		}
	}
}