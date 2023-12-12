using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.HereticalEngine.Rendering;

using HereticalSolutions.HereticalEngine.Scenes;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

using Silk.NET.Maths;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class OpenGLDrawTestCubeModule
		: IModule
	{
		private static readonly float[] CubeVertices =
		{
            //X    Y      Z
            -0.5f, -0.5f, -0.5f,
			 0.5f, -0.5f, -0.5f,
			 0.5f,  0.5f, -0.5f,
			 0.5f,  0.5f, -0.5f,
			-0.5f,  0.5f, -0.5f,
			-0.5f, -0.5f, -0.5f,

			-0.5f, -0.5f,  0.5f,
			 0.5f, -0.5f,  0.5f,
			 0.5f,  0.5f,  0.5f,
			 0.5f,  0.5f,  0.5f,
			-0.5f,  0.5f,  0.5f,
			-0.5f, -0.5f,  0.5f,

			-0.5f,  0.5f,  0.5f,
			-0.5f,  0.5f, -0.5f,
			-0.5f, -0.5f, -0.5f,
			-0.5f, -0.5f, -0.5f,
			-0.5f, -0.5f,  0.5f,
			-0.5f,  0.5f,  0.5f,

			 0.5f,  0.5f,  0.5f,
			 0.5f,  0.5f, -0.5f,
			 0.5f, -0.5f, -0.5f,
			 0.5f, -0.5f, -0.5f,
			 0.5f, -0.5f,  0.5f,
			 0.5f,  0.5f,  0.5f,

			-0.5f, -0.5f, -0.5f,
			 0.5f, -0.5f, -0.5f,
			 0.5f, -0.5f,  0.5f,
			 0.5f, -0.5f,  0.5f,
			-0.5f, -0.5f,  0.5f,
			-0.5f, -0.5f, -0.5f,

			-0.5f,  0.5f, -0.5f,
			 0.5f,  0.5f, -0.5f,
			 0.5f,  0.5f,  0.5f,
			 0.5f,  0.5f,  0.5f,
			-0.5f,  0.5f,  0.5f,
			-0.5f,  0.5f, -0.5f,
		};

		private GL gl = null;

		private IResourceStorageHandle cameraStorageHandle = null;

		private MeshOpenGL meshOpenGL = null;

		private IFormatLogger logger = null;

		#region IModule

		#region IGenericLifetimeable<ApplicationContext>

		public void SetUp(
			ApplicationContext context)
		{
			if (IsSetUp)
				context.Logger?.ThrowException<OpenGLDrawTestCubeModule>(
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
				context.Logger?.ThrowException<OpenGLDrawTestCubeModule>(
					"MODULE SHOULD BE SET UP BEFORE BEING INITIALIZED");
			}

			if (IsInitialized)
			{
				logger?.ThrowException<OpenGLDrawTestCubeModule>(
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


			GenerateAssets(
				context);

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

			meshOpenGL = null;

			logger = null;


			OnTornDown?.Invoke();

			OnInitialized = null;

			OnCleanedUp = null;

			OnTornDown = null;
		}

		public Action OnTornDown { get; set; }

		#endregion

		private async Task GenerateAssets(
			ApplicationContext context)
		{
			logger?.Log<OpenGLDrawTestCubeModule>("IMPORT STARTED");

			List<Task> assetImportTasks = new List<Task>();


			//RAM

			Vertex[] vertices = new Vertex[36];

			for (int i = 0; i < 36; i++)
			{
				vertices[i] = new Vertex
				{
					Position = new Vector3D<float>(
						CubeVertices[i * 3],
						CubeVertices[i * 3 + 1],
						CubeVertices[i * 3 + 2])
				};
			}

			uint[] indices = new uint[36];

			for (int i = 0; i < 36; i++)
			{
				indices[i] = (uint)i;
			}

			string geometryResourcePath = "Test cube/geometry";

			var importGeometryRAMTask = context.AssetImportManager.Import<GeometryRAMAssetImporter>(
				(importer) =>
				{
					importer.Initialize(
						geometryResourcePath,
						new GeometryRAM
						{
							Vertices = vertices,

							Indices = indices
						});
				}
			);

			assetImportTasks.Add(importGeometryRAMTask);

			//GL

			var importGeometryOpenGLTask = context.AssetImportManager.Import<GeometryOpenGLAssetImporter>(
				(importer) =>
				{
					importer.Initialize(
						geometryResourcePath,
						"Default shader",
						AssetImportConstants.ASSET_SHADER_OPENGL_VARIANT_ID,
						geometryResourcePath,
						AssetImportConstants.ASSET_3D_MODEL_RAM_VARIANT_ID);
				}
			);

			assetImportTasks.Add(importGeometryOpenGLTask);


			MaterialPrototypeDescriptor materialPrototypeDescriptor = default;

			materialPrototypeDescriptor.Name = "Cube material";

			materialPrototypeDescriptor.ShaderResourcePath = "Default shader";

			materialPrototypeDescriptor.TextureResourcePaths = new string[0];

			string materialResourcePath = "Test cube/material";

			var importMaterialPrototypeDescriptorTask = context.AssetImportManager.Import<MaterialPrototypeDescriptorAssetImporter>(
				(importer) =>
				{
					importer.Initialize(
						materialResourcePath,
						materialPrototypeDescriptor);
				}
			);

			assetImportTasks.Add(importMaterialPrototypeDescriptorTask);


			string meshResourcePath = "Test cube/mesh";

			var importMeshPrototypeDescriptorTask = context.AssetImportManager.Import<MeshPrototypeDescriptorAssetImporter>(
				(importer) =>
				{
					importer.Initialize(
						meshResourcePath,
						new MeshPrototypeDescriptor
						{
							GeometryResourcePath = geometryResourcePath,

							MaterialResourcePath = materialResourcePath
						});
				}
			);
			
			assetImportTasks.Add(importMeshPrototypeDescriptorTask);


			var importMaterialOpenGLTask = context.AssetImportManager.Import<MaterialOpenGLAssetImporter>(
				(importer) =>
				{
					importer.Initialize(
						materialResourcePath,
						materialResourcePath,
						AssetImportConstants.ASSET_3D_MODEL_RAM_VARIANT_ID);
				}
			);

			assetImportTasks.Add(importMaterialOpenGLTask);


			var importMeshOpenGLTask = context.AssetImportManager.Import<MeshOpenGLAssetImporter>(
				(importer) =>
				{
					importer.Initialize(
						meshResourcePath,
						meshResourcePath,
						AssetImportConstants.ASSET_3D_MODEL_RAM_VARIANT_ID);
				}
			);

			assetImportTasks.Add(importMeshOpenGLTask);


			//Load

			await Task
				.WhenAll(assetImportTasks)
				.ThrowExceptions<OpenGLDrawTestCubeModule>(context.Logger);

			meshOpenGL = context.RuntimeResourceManager
				.GetResource(
					meshResourcePath.SplitAddressBySeparator())
				.GetVariant(
					AssetImportConstants.ASSET_3D_MODEL_OPENGL_VARIANT_ID)
				.StorageHandle
				.GetResource<MeshOpenGL>();


			logger?.Log<OpenGLDrawTestCubeModule>("IMPORT FINISHED");
		}

		public void Update(
			ApplicationContext context,
			float timeDelta)
		{

		}

		public unsafe void Draw(
			ApplicationContext context,
			float timeDelta)
		{
			if (meshOpenGL == null)
				return;

			var camera = cameraStorageHandle.GetResource<Camera>();

			var modelMatrix = Matrix4X4<float>.Identity;

			var viewMatrix = camera.ViewMatrix;

			var projectionMatrix = camera.ProjectionMatrix;


			var geometry = meshOpenGL.GeometryOpenGL;

			var material = meshOpenGL.MaterialOpenGL;

			var shader = material.Shader;

			//Draws lines instead of polys
			gl.PolygonMode(
				GLEnum.FrontAndBack,
				PolygonMode.Line);

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

			gl.DrawElements(
				PrimitiveType.Triangles,
				(uint)geometry.ElementBufferObject.Length,
				DrawElementsType.UnsignedInt,
				(void*)0);

			var error = gl.GetError();

			if (error != GLEnum.NoError)
			{
				logger?.LogError<OpenGLDrawTestCubeModule>($"OpenGL ERROR: {error}");
			}

			gl.BindVertexArray(0);
		}

		#endregion
	}
}