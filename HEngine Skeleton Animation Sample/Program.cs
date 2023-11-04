#define USE_THREAD_SAFE_RESOURCE_MANAGEMENT

using System.Numerics;

using System.Drawing;

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.ResourceManagement.Factories;
using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Rendering;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;
using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.HereticalEngine.Math;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Logging;

using Silk.NET.Maths;

using Silk.NET.Windowing;

using Silk.NET.Input;

using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;

namespace HereticalSolutions.HereticalEngine.Samples
{
	public class Program
	{
		private static readonly float[] Vertices =
		{
            //X    Y      Z       Normals
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			 0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
			-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			 0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			 0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			 0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
			-0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,

			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
			-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

			 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
			 0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
			 0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
			 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
			 0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
			 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			-0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
			-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
			 0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
			 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			-0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
			-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
		};

		private static readonly uint[] Indices =
		{
			0, 1, 3,
			1, 2, 3
		};

		private static BufferOpenGL<float> Vbo;
		private static BufferOpenGL<uint> Ebo;
		private static VertexArrayOpenGL<float, uint> VaoCube;
		private static Shader CubeShader;

		private const string PATH_TO_SHADERS = "Shaders/default";

		private static IWindow window;

		//Setup the camera's location, directions, and movement speed
		private static Vector3D<float> CameraPosition = new Vector3D<float>(
			0.0f,
			0.0f,
			3.0f);

		private static Vector3D<float> CameraFront = new Vector3D<float>(
			0.0f,
			0.0f,
			-1.0f);

		private static Vector3D<float> CameraUp = Vector3D<float>.UnitY;

		private static Vector3D<float> CameraDirection = Vector3D<float>.Zero;

		private static float CameraYaw = -90f;

		private static float CameraPitch = 0f;
		
		private static float CameraZoom = 45f;

		private static IKeyboard primaryKeyboard;

		//Used to track change in mouse movement to allow for moving of the Camera
		private static Vector2D<float> LastMousePosition;

		private static ModelOpenGL modelOpenGL;

		//TODO: https://github.com/dotnet/Silk.NET/discussions/534
		static void Main(string[] args)
		{
			//var program = new Program();

			IFormatLogger logger = new ConsoleLogger();

			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer =
				new ConcurrentGenericCircularBuffer<MainThreadCommand>(
					new MainThreadCommand[1024],
					new int[1024]);

			// Create a Silk.NET window as usual
			window = Window.Create(WindowOptions.Default);

			// Declare some variables
			ImGuiController controller = null;

			GL gl = null;

			IInputContext inputContext = null;

#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
			IRuntimeResourceManager runtimeResourceManager = ResourceManagementFactory.BuildConcurrentRuntimeResourceManager(logger);
#else
			IRuntimeResourceManager runtimeResourceManager = ResourceManagementFactory.BuildRuntimeResourceManager(logger);
#endif

			// Our loading function
			window.Load += () =>
			{
				gl = InitGL(window);

				logger.Log<Program>($"CURRENT THREAD ID: {Thread.CurrentThread.ManagedThreadId}");

				Ebo = new BufferOpenGL<uint>(Gl, Indices, BufferTargetARB.ElementArrayBuffer);
				Vbo = new BufferOpenGL<float>(Gl, Vertices, BufferTargetARB.ArrayBuffer);
				VaoCube = new VertexArrayObject<float, uint>(Gl, Vbo, Ebo);

				VaoCube.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 6, 0);
				VaoCube.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 6, 3);

				//The lighting shader will give our main cube it's colour multiplied by the light's intensity
				LightingShader = new Shader(Gl, "shader.vert", "lighting.frag");

				LoadAssets(
					runtimeResourceManager,
					gl,
					mainThreadCommandBuffer,
					logger);

				inputContext = InitInputContext(window);

				primaryKeyboard = inputContext.Keyboards.FirstOrDefault();

				if (primaryKeyboard != null)
				{
					primaryKeyboard.KeyDown += KeyDown;
				}
				for (int i = 0; i < inputContext.Mice.Count; i++)
				{
					inputContext.Mice[i].Cursor.CursorMode = CursorMode.Raw;
					inputContext.Mice[i].MouseMove += OnMouseMove;
					inputContext.Mice[i].Scroll += OnMouseWheel;
				}

				controller = InitIMGUI(
					window,
					gl,
					inputContext);
				
				//PerformInitAsserts(logger);
			};

			// Handle resizes
			window.FramebufferResize += s =>
			{
				// Adjust the viewport to the new window size
				gl.Viewport(s);
			};

			window.Update += delta =>
			{
				while (mainThreadCommandBuffer.TryConsume(out var command))
				{
					if (command.Async)
					{
						var task = command.ExecuteAsync();

						task.Wait();
					}
					else
						command.Execute();
				}

				Update(
					controller,
					(float)delta);
			};

			// The render function
			window.Render += delta =>
			{
				Render(
					gl,
					controller);
			};

			// The closing function
			window.Closing += () =>
			{
				// Dispose our controller first
				controller?.Dispose();

				// Dispose the input context
				inputContext?.Dispose();

				// Unload OpenGL
				gl?.Dispose();
			};

			// Now that everything's defined, let's run this bad boy!
			window.Run();

			window.Dispose();
		}

		#region Window inits

		public static GL InitGL(IWindow window)
		{
			return window.CreateOpenGL();
		}

		public static IInputContext InitInputContext(IWindow window)
		{
			return window.CreateInput();
		}

		public static ImGuiController InitIMGUI(
			IWindow window,
			GL gl,
			IInputContext inputContext)
		{
			return new ImGuiController(
				gl, // load OpenGL
				window, // pass in our window
				inputContext // create an input context
			);
		}

		public static async Task LoadAssets(
			IRuntimeResourceManager runtimeResourceManager,
			GL gl,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
		{
			var pathToExe = System.Reflection.Assembly.GetExecutingAssembly().Location;

			//TODO: change
			var pathToAssets = pathToExe.Substring(
				0,
				pathToExe.IndexOf("/bin/"))
				+ "/Assets/";

			/*
			var progress = new Progress<float>();

			progress.ProgressChanged += (sender, value) =>
			{
				logger.Log<Program>
					($"PROGRESS: {(int)(value * 100f)} %");
			};
			*/

			var tasks = new List<Task>();

			#region Shader import

			var vertexShaderArgument = new TextFileArgument();

			vertexShaderArgument.Settings = new FilePathSettings
			{
				RelativePath = PATH_TO_SHADERS + ".vert",
				ApplicationDataFolder = pathToAssets
			};

			var fragmentShaderArgument = new TextFileArgument();

			fragmentShaderArgument.Settings = new FilePathSettings
			{
				RelativePath = PATH_TO_SHADERS + ".frag",
				ApplicationDataFolder = pathToAssets
			};

			var shaderAssetImporter = new ShaderOpenGLAssetImporter(
				runtimeResourceManager,
				"Default shader",
				PersistenceFactory.BuildSimplePlainTextSerializer(),
				vertexShaderArgument,
				fragmentShaderArgument,
				gl,
				mainThreadCommandBuffer,
				logger);

			var shaderImportProgress = new Progress<float>();

			shaderImportProgress.ProgressChanged += (sender, value) =>
			{
				logger.Log<Program>
					($"SHADER IMPORT PROGRESS: {(int)(value * 100f)} %");
			};

			/*
			await shaderAssetImporter
				.Import(
					shaderImportProgress)
				.ThrowExceptions<IResourceVariantData, Program>(logger);
			*/

			tasks.Add(
				Task.Run(
					() => shaderAssetImporter.Import(
						shaderImportProgress)));
			
			#endregion

			#region Model import

			var modelAssetImporter = new ModelRAMAssetImporter(
				runtimeResourceManager,
				"Knight",
				new FilePathSettings
				{
					RelativePath = "3D/Characters/Knight/Models/strongknight.fbx",
					ApplicationDataFolder = pathToAssets
				},
				mainThreadCommandBuffer,
				logger);

			IResourceVariantData modelRAMData = null;

			var modelImportProgress = new Progress<float>();

			modelImportProgress.ProgressChanged += (sender, value) =>
			{
				logger.Log<Program>
					($"MODEL IMPORT PROGRESS: {(int)(value * 100f)} %");
			};

			/*
			modelRAMData = await modelAssetImporter
				.Import(
					modelImportProgress)
				.ThrowExceptions<IResourceVariantData, Program>(logger);

			var modelRAMStorageHandle = modelRAMData.StorageHandle;
			*/

			tasks.Add(
				Task.Run(
					async delegate { modelRAMData = await modelAssetImporter.Import(modelImportProgress); }));

			#endregion

			await Task
				.WhenAll(tasks)
				.ThrowExceptions<Program>(logger);

			var modelRAMStorageHandle = modelRAMData.StorageHandle;

			#region Model OpenGL import

			var modelOpenGLAssetImporter = new ModelOpenGLAssetImporter(
				runtimeResourceManager,
				"Knight",
				modelRAMStorageHandle,
				gl,
				mainThreadCommandBuffer,
				logger);

			var modelOpenGLImportProgress = new Progress<float>();

			modelOpenGLImportProgress.ProgressChanged += (sender, value) =>
			{
				logger.Log<Program>
					($"MODEL OPENGL IMPORT PROGRESS: {(int)(value * 100f)} %");
			};

			var modelOpenGLData = await modelOpenGLAssetImporter
				.Import(
					modelOpenGLImportProgress)
				.ThrowExceptions<IResourceVariantData, Program>(logger);

			modelOpenGL = modelOpenGLData
				.StorageHandle
				.GetResource<ModelOpenGL>();

			#endregion

			logger.Log<Program>("IMPORT FINISHED");
		}

		#endregion

		#region Asserts

		static void PerformInitAsserts(
			IFormatLogger logger)
		{
			AssertVectors();

			logger.Log<Program>("--------------");

			AssertMatrices();

			logger.Log<Program>("--------------");

			AssertNoTransformation();

			logger.Log<Program>("--------------");

			AssertTranslate();

			logger.Log<Program>("--------------");

			AssertScale();

			logger.Log<Program>("--------------");

			AssertRotate();

			logger.Log<Program>("--------------");

			AssertReflect();

			logger.Log<Program>("--------------");

			AssertOrtho();

			logger.Log<Program>("--------------");

			AssertPerspective();

			logger.Log<Program>("--------------");

			AssertCombinedTransformations();

			logger.Log<Program>("--------------");

			AssertQuaternions();
		}

		static void AssertVectors()
		{
			Console.WriteLine("Assert Vectors");

			//Expected:
			//(1, 3)
			Console.WriteLine(
				new Vector2D<float>(
					1.0f, 3.0f));

			//Expected:
			//(1, 3, 2)
			Console.WriteLine(
				new Vector3D<float>(
					1.0f, 3.0f, 2.0f));

			//Expected:
			//(0, 1, 0)
			Console.WriteLine(
				Vector3D<float>.UnitY);

			//Expected:
			//(0, 0, 0)
			Console.WriteLine(
				Vector3D<float>.Zero);

			//Expected:
			//(3, 1, 1)
			Console.WriteLine(
				new Vector3D<float>(
					1.0f, 3.0f, 2.0f)
				+ new Vector3D<float>(
					2.0f, -2.0f, -1.0f));
		}

		static void AssertMatrices()
		{
			Console.WriteLine("Assert Matrices");

			//Expected:
			//Result 1:
			//(4, 1)
			//(2, -1)
			Console.WriteLine("Result 1\n"
				+ new Matrix2X2<float>(
					4.0f, 1.0f,
					2.0f, -1.0f));

			//Expected:
			//Result 2:
			//(1, 0, 0)
			//(0, 1, 0)
			//(0, 0, 1)
			Console.WriteLine("Result 2\n"
				+ Matrix3X3<float>.Identity);

			//Expected:
			//Result 3:
			//(0, 0, 0, 0)
			//(0, 0, 0, 0)
			//(0, 0, 0, 0)
			//(0, 0, 0, 0)
			Console.WriteLine("Result 3\n"
				+ Matrix4X4Extensions.Zero<float>());

			//Expected:
			//Result 4:
			//(4, -7)
			//(2, -5)
			Console.WriteLine("Result 4\n"
				+ new Matrix2X2<float>(
					4.0f, 1.0f,
					2.0f, -1.0f)
				* new Matrix2X2<float>(
					1.0f, -2.0f,
					0.0f, 1.0f));

			//Expected:
			//Result 5:
			//(4, 5, -4)
			Console.WriteLine("Result 5\n"
				+ new Vector3D<float>(
					1.0f, 3.0f, 1.0f)
				* new Matrix3X3<float>(
					1.0f, -2.0f, 0.0f,
					1.0f, 2.0f, -2.0f,
					0.0f, 1.0f, 2.0f));
		}

		static void AssertNoTransformation()
		{
			Console.WriteLine("Assert No Transformation");

			var vec = new Vector4D<float>(
				5.0f, 2.0f, 0.0f, 1.0f);

			var mat = Matrix4X4<float>.Identity;

			//Expected:
			//(5, 2, 0, 1)
			Console.WriteLine(vec * mat);
		}

		static void AssertTranslate()
		{
			Console.WriteLine("Assert Translate Transformation");

			var vec = new Vector4D<float>(
				5.0f, 2.0f, 0.0f, 1.0f);

			var mat = Matrix4X4.CreateTranslation(
				-3.0f, -5.0f, 0.0f);

			//Expected:
			//(2, -3, 0, 1)
			Console.WriteLine(vec * mat);
		}

		static void AssertScale()
		{
			Console.WriteLine("Assert Scale Transformation");

			var vec = new Vector4D<float>(
				4.0f, 5.0f, 0.0f, 1.0f);

			var mat = Matrix4X4.CreateScale(
				0.5f, 0.5f, 0.0f);

			//Expected:
			//(2, 2.5, 0, 1)
			Console.WriteLine(vec * mat);
		}

		static void AssertRotate()
		{
			Console.WriteLine("Assert Rotate Transformation");

			var vec = new Vector4D<float>(
				5.0f, 2.0f, 0.0f, 1.0f);

			var mat = Matrix4X4.CreateRotationZ(
				Convert.ToSingle(System.Math.PI / 2));

			//Expected:
			//(-2, 5, 0, 1)
			Console.WriteLine(vec * mat);
		}

		static void AssertReflect()
		{
			Console.WriteLine("Assert Reflect Transformation");

			var vec = new Vector4D<float>(
				2.0f, 3.0f, 0.0f, 1.0f);

			var mat = Matrix4X4<float>.Identity;

			//mat[1, 1] = -1.0f;
			mat.Row2.Y = -1.0f;

			//Expected:
			//(2, -3, 0, 1)
			Console.WriteLine(vec * mat);
		}

		static void AssertOrtho()
		{
			Console.WriteLine("Assert Orthographic Projection");

			var vec = new Vector4D<float>(
				5.0f, 2.0f, 0.0f, 1.0f);

			var mat = Matrix4X4.CreateOrthographic(
				10f, 10f, -10f, 10f);

			//Expected:
			//(1, 0.4, 0, 1)
			//WARNING! RETURNED <1, 0.4, 0.5, 1>
			Console.WriteLine(vec * mat);
		}

		static void AssertPerspective()
		{
			Console.WriteLine("Assert Perspective Projection");

			var mat = Matrix4X4.CreatePerspectiveFieldOfView(
				Convert.ToSingle(System.Math.PI / 4), 1.0f, 1.0f, 50.0f);

			var vec1 = new Vector4D<float>(
				4.0f, 3.0f, -10.0f, 1.0f);

			var res1 = (vec1 * mat);

			res1 /= res1.W;

			//Expected:
			//(0.9656854, 0.7242641, 0.8367347, 1)
			//WARNING! RETURNED <0.96568537, 0.724264, 0.9183674, 1>
			Console.WriteLine(res1);

			var vec2 = new Vector4D<float>(
				4.0f, 3.0f, -50.0f, 1.0f);

			var res2 = (vec2 * mat);

			res2 /= res2.W;

			//Expected:
			//(0.1931371, 0.1448528, 1, 1)
			Console.WriteLine(res2);
		}

		static void AssertCombinedTransformations()
		{
			Console.WriteLine("Assert Combined Transformations");

			var vec = new Vector4D<float>(
				5.0f, 1.0f, 0.0f, 1.0f);

			var mat1 = Matrix4X4.CreateTranslation(
				0.0f, 1.0f, 0.0f);

			var mat2 = Matrix4X4.CreateRotationZ(
				Convert.ToSingle(System.Math.PI / 2));

			//Expected:
			//(-2, 5, 0, 1)
			Console.WriteLine(vec * mat1 * mat2);

			//Expected:
			//(-1, 6, 0, 1)
			Console.WriteLine(vec * mat2 * mat1);
		}

		static void AssertQuaternions()
		{
			Console.WriteLine("Assert Quaternions");

			var vec = new Vector4D<float>(
				5.0f, 2.0f, 0.0f, 1.0f);

			var mat = Matrix4X4.CreateRotationZ(
				Convert.ToSingle(System.Math.PI / 4));

			//Expected:
			//(2.12132, 4.949748, 0, 1)
			Console.WriteLine(vec * mat);

			var qua = Quaternion<float>.CreateFromAxisAngle(
				Vector3D<float>.UnitZ,
				Convert.ToSingle(System.Math.PI / 4));

			//Expected:
			//(2.12132, 4.949748, 0, 1)
			//Console.WriteLine(qua * vec);
			Console.WriteLine(Vector4DExtensions.Multiply(qua, vec));
		}

		#endregion

		#region Update

		static void Update(
			ImGuiController controller,
			float delta)
		{
			UpdateIMGUI(
				controller,
				delta);

			var moveSpeed = 2.5f * (float)delta;

			if (primaryKeyboard.IsKeyPressed(Key.W))
			{
				//Move forwards
				CameraPosition += moveSpeed * CameraFront;
			}
			if (primaryKeyboard.IsKeyPressed(Key.S))
			{
				//Move backwards
				CameraPosition -= moveSpeed * CameraFront;
			}
			if (primaryKeyboard.IsKeyPressed(Key.A))
			{
				//Move left
				CameraPosition -= Vector3D.Normalize(Vector3D.Cross(CameraFront, CameraUp)) * moveSpeed;
			}
			if (primaryKeyboard.IsKeyPressed(Key.D))
			{
				//Move right
				CameraPosition += Vector3D.Normalize(Vector3D.Cross(CameraFront, CameraUp)) * moveSpeed;
			}
		}

		static void UpdateIMGUI(
			ImGuiController controller,
			float delta)
		{
			// Make sure ImGui is up-to-date
			controller.Update((float)delta);
		}

		private static void OnMouseMove(
			IMouse mouse,
			Vector2 positionInput)
		{
			Vector2D<float> position = positionInput.ToSilkNetVector2D();

			var lookSensitivity = 0.1f;

			if (LastMousePosition == default)
			{
				LastMousePosition = position;
			}
			else
			{
				var xOffset = (position.X - LastMousePosition.X) * lookSensitivity;

				var yOffset = (position.Y - LastMousePosition.Y) * lookSensitivity;


				LastMousePosition = position;


				CameraYaw += xOffset;

				CameraPitch -= yOffset;

				//We don't want to be able to look behind us by going over our head or under our feet so make sure it stays within these bounds
				CameraPitch = float.Clamp(
					CameraPitch,
					-89.0f,
					89.0f);

				CameraDirection.X = 
					MathF.Cos(MathHelpers.DegreesToRadians(CameraYaw))
					* MathF.Cos(MathHelpers.DegreesToRadians(CameraPitch));

				CameraDirection.Y = MathF.Sin(MathHelpers.DegreesToRadians(CameraPitch));

				CameraDirection.Z = 
					MathF.Sin(MathHelpers.DegreesToRadians(CameraYaw))
					* MathF.Cos(MathHelpers.DegreesToRadians(CameraPitch));

				CameraFront = Vector3D.Normalize(CameraDirection);
			}
		}

		private static void OnMouseWheel(
			IMouse mouse,
			ScrollWheel scrollWheel)
		{
			//We don't want to be able to zoom in too close or too far away so clamp to these values
			CameraZoom = float.Clamp(
				CameraZoom - scrollWheel.Y,
				1.0f,
				45f);
		}

		private static void KeyDown(
			IKeyboard keyboard,
			Key key,
			int arg3)
		{
			if (key == Key.Escape)
			{
				window.Close();
			}
		}

		#endregion

		#region Rendering

		static void Render(
			GL gl,
			ImGuiController controller)
		{
			Clear(gl);

			if (modelOpenGL != null)
				RenderModel(
					gl,
					modelOpenGL);

			RenderIMGUI(controller);
		}

		static void Clear(
			GL gl)
		{
			// This is where you'll do any rendering beneath the ImGui context
			// Here, we just have a blank screen.
			gl.ClearColor(Color.FromArgb(255, (int)(.45f * 255), (int)(.55f * 255), (int)(.60f * 255)));

			gl.Clear((uint)ClearBufferMask.ColorBufferBit);
		}

		static void RenderIMGUI(ImGuiController controller)
		{
			// This is where you'll do all of your ImGUi rendering
			// Here, we're just showing the ImGui built-in demo window.
			ImGuiNET.ImGui.ShowDemoWindow();

			// Make sure ImGui renders too!
			controller.Render();
		}

		static void RenderModel(
			GL gl,
			ModelOpenGL modelOpenGL)
		{
			//var modelMatrix = Matrix4X4<float>.Identity;

			var difference = (float)(window.Time * 100);

			var modelMatrix =
				Matrix4X4.CreateRotationY(
					MathHelpers.DegreesToRadians(difference))
				* Matrix4X4.CreateRotationX(
					MathHelpers.DegreesToRadians(difference));

			var viewMatrix = Matrix4X4.CreateLookAt(
				CameraPosition,
				CameraPosition + CameraFront,
				CameraUp);
			
			//It's super important for the width / height calculation to regard each value as a float, otherwise
			//it creates rounding errors that result in viewport distortion
			var projectionMatrix = Matrix4X4.CreatePerspectiveFieldOfView(
				MathHelpers.DegreesToRadians(CameraZoom),
				(float)window.Size.X / (float)window.Size.Y,
				0.1f,
				1000.0f);

			RenderNode(
				gl,
				modelOpenGL.RootNode,
				modelMatrix,
				viewMatrix,
				projectionMatrix);
		}

		static void RenderNode(
			GL gl,
			ModelNodeOpenGL node,
			Matrix4X4<float> modelMatrix,
			Matrix4X4<float> viewMatrix,
			Matrix4X4<float> projectionMatrix)
		{
			var nodeTransform = node.Transform;

			var nodeModelMatrix = modelMatrix; // * nodeTransform.TRSMatrix;

			foreach (var mesh in node.Meshes)
			{
				RenderMesh(
					gl,
					mesh,
					nodeModelMatrix,
					viewMatrix,
					projectionMatrix);
			}

			foreach (var childNode in node.Children)
			{
				RenderNode(
					gl,
					childNode,
					nodeModelMatrix,
					viewMatrix,
					projectionMatrix);
			}
		}

		static void RenderMesh(
			GL gl,
			MeshOpenGL meshOpenGL,
			Matrix4X4<float> modelMatrix,
			Matrix4X4<float> viewMatrix,
			Matrix4X4<float> projectionMatrix)
		{
			var geometry = meshOpenGL.GeometryOpenGL;

			var material = meshOpenGL.MaterialOpenGL;

			var shader = material.Shader;

			geometry.Bind(gl);

			shader.Use(gl);

			foreach (var texture in material.Textures)
			{
				texture.Bind(gl);

				//FOR TEST PURPOSES
				break; 
			}

			shader.SetUniform(
				gl,
				"uTexture0",
				0);

			shader.SetUniform(
				gl,
				"uModel",
				modelMatrix);

			shader.SetUniform(
				gl,
				"uView",
				viewMatrix);

			shader.SetUniform(
				gl,
				"uProjection",
				projectionMatrix);

			//FOR TEST PURPOSES
			gl.PolygonMode(
				GLEnum.FrontAndBack,
				PolygonMode.Line);

			gl.DrawArrays(
				PrimitiveType.Triangles,
				0,
				(uint)geometry.VerticesBuffer.Length);
		}

		#endregion
	}
}