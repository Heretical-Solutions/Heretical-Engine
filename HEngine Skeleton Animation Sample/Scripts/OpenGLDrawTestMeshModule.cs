using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;
using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Application;

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

		#region IModule

		#region IGenericLifetimeable<ApplicationContext>

		public void SetUp(
			ApplicationContext context)
		{
			if (IsSetUp)
				context.Logger?.ThrowException<OpenGLDrawTestMeshModule>(
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
				context.Logger?.ThrowException<OpenGLDrawTestMeshModule>(
					"MODULE SHOULD BE SET UP BEFORE BEING INITIALIZED");
			}

			if (IsInitialized)
			{
				logger?.ThrowException<OpenGLDrawTestMeshModule>(
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


			cameraStorageHandle = (IResourceStorageHandle)context.RuntimeResourceManager
				.GetDefaultResource(
					MainCameraModule.MAIN_CAMERA_RESOURCE_PATH.SplitAddressBySeparator())
				.StorageHandle;

			if (!cameraStorageHandle.Allocated)
			{
				task = cameraStorageHandle.Allocate();

				task.Wait();
			}


			LoadAssets(
				context,
				gl);


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
			ApplicationContext context,
			GL gl)
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
				logger?.Log<OpenGLDrawTestMeshModule>
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
				"Default shader",
				PersistenceFactory.BuildSimplePlainTextSerializer(),
				vertexShaderArgument,
				fragmentShaderArgument,
				context);

			var shaderImportProgress = new Progress<float>();

			shaderImportProgress.ProgressChanged += (sender, value) =>
			{
				logger?.Log<OpenGLDrawTestMeshModule>
					($"SHADER IMPORT PROGRESS: {(int)(value * 100f)} %");
			};

			/*
			await shaderAssetImporter
				.Import(
					shaderImportProgress)
				.ThrowExceptions<IResourceVariantData, OpenGLDrawTestMeshModule>(logger);
			*/

			tasks.Add(
				shaderAssetImporter.Import());
			//shaderImportProgress)));

			#endregion

			#region Model import

			var modelAssetImporter = new ModelRAMAssetImporter(
				"Knight", //"Suzanne", //"Knight",
				new FilePathSettings
				{
					RelativePath = "3D/Characters/Knight/Models/strongknight.fbx", // "3D/Characters/Suzanne/Models/suzanne_triangulated.fbx", //"3D/Characters/Knight/Models/strongknight.fbx",
					ApplicationDataFolder = pathToAssets
				},
				context);

			IResourceVariantData modelRAMData = null;

			var modelImportProgress = new Progress<float>();

			modelImportProgress.ProgressChanged += (sender, value) =>
			{
				logger?.Log<OpenGLDrawTestMeshModule>
					($"MODEL IMPORT PROGRESS: {(int)(value * 100f)} %");
			};

			/*
			modelRAMData = await modelAssetImporter
				.Import(
					modelImportProgress)
				.ThrowExceptions<IResourceVariantData, OpenGLDrawTestMeshModule>(logger);

			var modelRAMStorageHandle = modelRAMData.StorageHandle;
			*/

			Func<Task> importModelIntoRAM = async () =>
			{
				modelRAMData = await modelAssetImporter
					.Import()
					.ThrowExceptions<IResourceVariantData, OpenGLDrawTestMeshModule>(logger);
			};

			tasks.Add(importModelIntoRAM());

			//async delegate{ modelRAMData = await modelAssetImporter.Import(modelImportProgress).ThrowExceptions<IResourceVariantData, OpenGLDrawTestMeshModule>(logger); }));

			#endregion

			await Task
				.WhenAll(tasks)
				.ThrowExceptions<OpenGLDrawTestMeshModule>(logger);

			#region Model OpenGL import

			var modelOpenGLAssetImporter = new ModelOpenGLAssetImporter(
				"Knight", //"Suzanne", //"Knight",
				"Knight",
				ModelRAMAssetImporter.MODEL_RAM_VARIANT_ID,
				context);

			var modelOpenGLImportProgress = new Progress<float>();

			modelOpenGLImportProgress.ProgressChanged += (sender, value) =>
			{
				logger?.Log<OpenGLDrawTestMeshModule>
					($"MODEL OPENGL IMPORT PROGRESS: {(int)(value * 100f)} %");
			};

			var modelOpenGLData = await modelOpenGLAssetImporter
				.Import()
				//modelOpenGLImportProgress)
				.ThrowExceptions<IResourceVariantData, OpenGLDrawTestMeshModule>(logger);

			modelOpenGL = modelOpenGLData
				.StorageHandle
				.GetResource<ModelOpenGL>();

			#endregion

			logger?.Log<OpenGLDrawTestMeshModule>("IMPORT FINISHED");
		}

		private void RenderModel(
			GL gl,
			ModelOpenGL modelOpenGL)
		{
			var camera = cameraStorageHandle.GetResource<Camera>();

			var modelMatrix = Matrix4X4<float>.Identity;

			//FOR DEBUG PURPOSES ONLY
			//Use when the imported model (like some FBXes) is huge
			modelMatrix *= Matrix4X4.CreateScale(0.02f);

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

			var nodeModelMatrix = modelMatrix * nodeTransform.TRSMatrix;

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

			/*
			//FOR TEST PURPOSES
			//Draws lines instead of polys
			gl.PolygonMode(
				GLEnum.FrontAndBack,
				PolygonMode.Line);
			*/

			geometry.VertexArrayObject.Bind(gl);

			shader.Use(gl);

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

			#region Debug

			//If I ever need to retrieve data back from OpenGL - well, here it is how it's done
			/*
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

			logger?.Log<OpenGLDrawTestMeshModule>(sb.ToString());

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

			logger?.Log<OpenGLDrawTestMeshModule>(sb.ToString());
			*/

			#endregion

			/*
			foreach (var textureOpenGL in material.Textures)
			{
				// Much like our texture creation earlier, we must first set our active texture unit, and then bind the
				// texture to use it during draw!
				textureOpenGL.Bind(gl);
			}
			*/

			//material.Textures[0].Bind(gl);

			/*
			gl.DrawArrays(
				PrimitiveType.Triangles,
				0,
				(uint)geometry.VertexBufferObject.Length);
			*/

			gl.DrawElements(
				PrimitiveType.Triangles,
				(uint)geometry.ElementBufferObject.Length,
				DrawElementsType.UnsignedInt,
				(void*)0); //MOTHERFUCKER! DUE TO THE ABUNDANCE OF FUCKING OVERLOADS FOR THIS METHOD IN Silk.NET, WHEN I PUT THE BLAND "0" AS AN ARGUMENT IT SWITCHES TO AN OVERLOAD THAT THINKS OF THIS ARGUMENT AS IF IT WAS A REFERENCE TO INDICES ARRAY. THE ONLY REASON I NOTICED IT IS THAT IT SOMEHOW SHOWED THE "in" MODIFIER WHEN I HOVERED THE MOUSE OVER THE CALL IN THE IDE. ALL THE FUCKING ISSUES I HAD FOR SEVERAL FUCKING DAYS IN A ROW WERE NOT BECAUSE I WAS POORLY TREATING GL COMMANDS BUT RATHER DUE TO THE FACT THAT THIS STUPID ASS 0 HAD TO BE CAST TO (void*) FOR THE PROPER OVERLOAD TO BE SELECTED

			var error = gl.GetError();

			if (error != GLEnum.NoError)
			{
				logger?.LogError<OpenGLDrawTestMeshModule>($"OpenGL ERROR: {error}");
			}

			gl.BindVertexArray(0);
		}
	}
}