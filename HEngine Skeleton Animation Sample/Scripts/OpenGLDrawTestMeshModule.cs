using System.Text;

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;
using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.HereticalEngine.Rendering;

using HereticalSolutions.HereticalEngine.Scenes;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

using Silk.NET.Maths;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class OpenGLDrawTestMeshModule
		: IModule
	{
		private const string PATH_TO_SHADERS = "Shaders/default";

		private GL gl = null;

		private IResourceStorageHandle cameraStorageHandle = null;

		private ModelOpenGL modelOpenGL = null;

		private IFormatLogger logger = null;

		private int debugCountdown = 0;

		private int debugCountdownDuration = 10;

		private int debugIndicesCounter;

		#region IModule

		#region IGenericLifetimeable<ApplicationContext>

		public void SetUp(
			ApplicationContext context)
		{
			if (IsSetUp)
				context.Logger.ThrowException<OpenGLDrawTestMeshModule>(
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
				context.Logger.ThrowException<OpenGLDrawTestMeshModule>(
					"MODULE SHOULD BE SET UP BEFORE BEING INITIALIZED");
			}

			if (IsInitialized)
			{
				logger.ThrowException<OpenGLDrawTestMeshModule>(
					"ATTEMPT TO INITIALIZE MODULE THAT IS ALREADY INITIALIZED");
			}

			//Initialization
			var glStorageHandle = context.RuntimeResourceManager
				.GetDefaultResource(
					"Application/GL".SplitAddressBySeparator())
				.StorageHandle;

			Task task;

			if (!glStorageHandle.Allocated)
			{
				task = glStorageHandle.Allocate();

				task.Wait();
			}

			gl = glStorageHandle.GetResource<GL>();


			cameraStorageHandle = (IResourceStorageHandle)context.RuntimeResourceManager
				.GetDefaultResource(
					"Application/Main camera".SplitAddressBySeparator())
				.StorageHandle;

			if (!cameraStorageHandle.Allocated)
			{
				task = cameraStorageHandle.Allocate();

				task.Wait();
			}


			LoadAssets(
				context.RuntimeResourceManager,
				gl,
				context.MainThreadCommandBuffer,
				logger);


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

			cameraStorageHandle = null;

			modelOpenGL = null;

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
			if (modelOpenGL != null)
				RenderModel(
					gl,
					modelOpenGL);
		}

		#endregion

		private async Task LoadAssets(
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
					() => shaderAssetImporter.Import()));
			//shaderImportProgress)));

			#endregion

			#region Model import

			var modelAssetImporter = new ModelRAMAssetImporter(
				runtimeResourceManager,
				"Suzanne", //"Knight",
				new FilePathSettings
				{
					RelativePath = "3D/Characters/Suzanne/Models/suzanne_triangulated.fbx", //"3D/Characters/Knight/Models/strongknight.fbx",
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
					async delegate { modelRAMData = await modelAssetImporter.Import(); }));
			//async delegate { modelRAMData = await modelAssetImporter.Import(modelImportProgress); }));

			#endregion

			await Task
				.WhenAll(tasks)
				.ThrowExceptions<Program>(logger);

			var modelRAMStorageHandle = modelRAMData.StorageHandle;

			#region Model OpenGL import

			var modelOpenGLAssetImporter = new ModelOpenGLAssetImporter(
				runtimeResourceManager,
				"Suzanne", //"Knight",
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
				.Import()
				//modelOpenGLImportProgress)
				.ThrowExceptions<IResourceVariantData, Program>(logger);

			modelOpenGL = modelOpenGLData
				.StorageHandle
				.GetResource<ModelOpenGL>();

			#endregion

			logger.Log<Program>("IMPORT FINISHED");
		}

		private void RenderModel(
			GL gl,
			ModelOpenGL modelOpenGL)
		{
			var camera = cameraStorageHandle.GetResource<Camera>();

			var modelMatrix = Matrix4X4<float>.Identity;

			//modelMatrix *= Matrix4X4.CreateScale(0.02f);

			var viewMatrix = camera.ViewMatrix;

			var projectionMatrix = camera.ProjectionMatrix;

			RenderNode(
				gl,
				modelOpenGL.RootNode,
				modelMatrix,
				viewMatrix,
				projectionMatrix);
		}

		private void RenderNode(
			GL gl,
			ModelNodeOpenGL node,
			Matrix4X4<float> modelMatrix,
			Matrix4X4<float> viewMatrix,
			Matrix4X4<float> projectionMatrix)
		{
			var nodeTransform = node.Transform;

			var nodeModelMatrix = modelMatrix;

			//var nodeModelMatrix = modelMatrix * nodeTransform.TRSMatrix;

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

		private unsafe void RenderMesh(
			GL gl,
			MeshOpenGL meshOpenGL,
			Matrix4X4<float> modelMatrix,
			Matrix4X4<float> viewMatrix,
			Matrix4X4<float> projectionMatrix)
		{
			var geometry = meshOpenGL.GeometryOpenGL;

			var material = meshOpenGL.MaterialOpenGL;

			var shader = material.Shader;


			//FOR TEST PURPOSES
			gl.PolygonMode(
				GLEnum.FrontAndBack,
				PolygonMode.Line);


			//geometry.VertexArray.Bind(gl);

			shader.Use(gl);

			/*
			shader.SetUniform(
				gl,
				"uTexture0",
				0);
			*/

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


			debugCountdown--;

			if (debugCountdown <= 0)
			{
				debugCountdown = debugCountdownDuration;

				debugIndicesCounter += 3;

				debugIndicesCounter %= geometry.IndicesBuffer.Length;

				//Console.WriteLine($"DRAWING {debugIndicesCounter}");
			}

			geometry.VertexArray.Bind(gl);

			//geometry.VerticesBuffer.Bind(gl);

			//geometry.IndicesBuffer.Bind(gl);

			uint[] indicesFromBuffer = new uint[30];

			fixed (uint* indicesFromBufferPointer = &indicesFromBuffer[0])
			{
				gl.GetBufferSubData(
					BufferTargetARB.ElementArrayBuffer,
					(nint)0,
					(uint)(30 * sizeof(uint)),
					indicesFromBufferPointer);
			}

			StringBuilder sb = new StringBuilder("INDICES DUMP:\n");

			for (int i = 0; i < 30; i++)
			{
				sb.Append(indicesFromBuffer[i]);

				sb.Append(" ");
			}

			Console.WriteLine(sb.ToString());

			float[] verticesFromBuffer = new float[30];

			fixed (float* verticesFromBufferPointer = &verticesFromBuffer[0])
			{
				gl.GetBufferSubData(
					BufferTargetARB.ArrayBuffer,
					(nint)0,
					(uint)(30 * sizeof(float)),
					verticesFromBufferPointer);
			}

			sb = new StringBuilder("VERTICES DUMP:\n");

			for (int i = 0; i < 30; i++)
			{
				sb.Append(verticesFromBuffer[i]);

				sb.Append(" ");
			}

			Console.WriteLine(sb.ToString());

			///*
			gl.DrawElements(
				PrimitiveType.Triangles,
				//(uint)debugIndicesCounter,
				(uint)geometry.IndicesBuffer.Length,
				DrawElementsType.UnsignedInt,
				0);
			//*/

			/*
			gl.DrawArrays(
				PrimitiveType.Triangles,
				0,
				(uint)debugIndicesCounter);
			*/

			var error = gl.GetError();

			if (error != GLEnum.NoError)
			{
				Console.WriteLine($"ERROR: {error}");
			}

			gl.BindVertexArray(0);

			/*
			gl.BindBuffer(
				BufferTargetARB.ArrayBuffer,
				0);

			gl.BindBuffer(
				BufferTargetARB.ElementArrayBuffer,
				0);
			*/
		}
	}
}