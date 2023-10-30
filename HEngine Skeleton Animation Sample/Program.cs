using System.Drawing;

using Silk.NET.Maths;

using Silk.NET.Windowing;

using Silk.NET.Input;

using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;

using HereticalSolutions.ResourceManagement.Factories;
using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Rendering;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

using HereticalSolutions.HereticalEngine.Math;
using HereticalSolutions.Persistence.Factories;

namespace HereticalSolutions.HereticalEngine.Samples
{
	public class Program
	{
		private const string PATH_TO_SHADERS = "Shaders/default";

		//TODO: https://github.com/dotnet/Silk.NET/discussions/534
		static void Main(string[] args)
		{
			//var program = new Program();

			// Create a Silk.NET window as usual
			using var window = Window.Create(WindowOptions.Default);

			// Declare some variables
			ImGuiController controller = null;

			GL gl = null;

			IInputContext inputContext = null;

			IRuntimeResourceManager runtimeResourceManager = ResourceManagementFactory.BuildRuntimeResourceManager();

			// Our loading function
			window.Load += () =>
			{
				gl = InitGL(window);

				LoadAssets(
					runtimeResourceManager,
					gl);

				inputContext = InitInputContext(window);

				controller = InitIMGUI(
					window,
					gl,
					inputContext);
				
				PerformInitAsserts();
			};

			// Handle resizes
			window.FramebufferResize += s =>
			{
				// Adjust the viewport to the new window size
				gl.Viewport(s);
			};

			// The render function
			window.Render += delta =>
			{
				Update(
					controller,
					(float)delta);

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

		public static void LoadAssets(
			IRuntimeResourceManager runtimeResourceManager,
			GL gl)
		{
			var pathToExe = System.Reflection.Assembly.GetExecutingAssembly().Location;

			//TODO: change
			var pathToAssets = pathToExe.Substring(
				0,
				pathToExe.IndexOf("/bin/"))
				+ "/Assets/";

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

			var shaderAssimp = new ShaderAssetImporter(
				runtimeResourceManager,
				"Default shader",
				PersistenceFactory.BuildSimplePlainTextSerializer(),
				vertexShaderArgument,
				fragmentShaderArgument,
				new ShaderVisitor(gl));

			shaderAssimp.Import();

			#endregion

			#region Model import

			var modelAssimp = new ModelRAMAssetImporter(
				runtimeResourceManager,
				"Suit",
				new FilePathSettings
				{
					RelativePath = "3D/Characters/Knight/Models/strongknight.fbx",
					ApplicationDataFolder = pathToAssets
				},
				gl);

				modelAssimp.Import();

			#endregion

			Console.WriteLine("Import finished");
		}

		#endregion

		#region Asserts

		static void PerformInitAsserts()
		{
			AssertVectors();

			Console.WriteLine("--------------");

			AssertMatrices();

			Console.WriteLine("--------------");

			AssertNoTransformation();

			Console.WriteLine("--------------");

			AssertTranslate();

			Console.WriteLine("--------------");

			AssertScale();

			Console.WriteLine("--------------");

			AssertRotate();

			Console.WriteLine("--------------");

			AssertReflect();

			Console.WriteLine("--------------");

			AssertOrtho();

			Console.WriteLine("--------------");

			AssertPerspective();

			Console.WriteLine("--------------");

			AssertCombinedTransformations();

			Console.WriteLine("--------------");

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
		}

		static void UpdateIMGUI(
			ImGuiController controller,
			float delta)
		{
			// Make sure ImGui is up-to-date
			controller.Update((float)delta);
		}

		#endregion

		#region Rendering

		static void Render(
			GL gl,
			ImGuiController controller)
		{
			Clear(gl);

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

		#endregion
	}
}