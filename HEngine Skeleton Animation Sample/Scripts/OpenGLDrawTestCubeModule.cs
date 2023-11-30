using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.HereticalEngine.Rendering;
using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Scenes;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;
using Silk.NET.Maths;
using HereticalSolutions.HereticalEngine.AssetImport;

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

			List<AssetImporter> assetImporters = new List<AssetImporter>();


			//RAM

			MaterialDTO materialDTO = default;

			materialDTO.Name = "Cube material";

			materialDTO.ShaderResourceID = "Default shader";

			materialDTO.TextureResourceIDs = new string[0];

			string materialResourceID = "Test cube/material";

			var materialRAMImporter = new MaterialRAMAssetImporter(
				materialResourceID,
				materialDTO,
				context);

			assetImporters.Add(materialRAMImporter);


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

			string geometryResourceID = "Test cube/geometry";

			var geometryRAMImporter = new GeometryRAMAssetImporter(
				geometryResourceID,
				new Geometry
				{
					Vertices = vertices,

					Indices = indices
				},
				context);

			assetImporters.Add(geometryRAMImporter);


			string meshResourceID = "Test cube/mesh";

			var meshRAMImporter = new MeshRAMAssetImporter(
				meshResourceID,
				new MeshDTO
				{
					GeometryResourceID = geometryResourceID,

					MaterialResourceID = materialResourceID
				},
				context);

			assetImporters.Add(meshRAMImporter);


			//GL

			var geometryOpenGLImporter = new GeometryOpenGLAssetImporter(
				geometryResourceID,
				"Default shader",
				ShaderOpenGLAssetImporter.SHADER_OPENGL_VARIANT_ID,
				geometryResourceID,
				GeometryRAMAssetImporter.GEOMETRY_RAM_VARIANT_ID,
				context);

			assetImporters.Add(geometryOpenGLImporter);


			var materialOpenGLImporter = new MaterialOpenGLAssetImporter(
				materialResourceID,
				materialResourceID,
				MaterialRAMAssetImporter.MATERIAL_RAM_VARIANT_ID,
				context);

			assetImporters.Add(materialOpenGLImporter);


			var meshOpenGLImporter = new MeshOpenGLAssetImporter(
				meshResourceID,
				meshResourceID,
				MeshRAMAssetImporter.MESH_RAM_VARIANT_ID,
				context);

			assetImporters.Add(meshOpenGLImporter);


			//Load

			List<Task> assetImportersTasks = new List<Task>();

			foreach (var assetImporter in assetImporters)
			{
				Func<Task> load = async () =>
					{
						logger?.Log<OpenGLDrawTestCubeModule>($"IMPORTING {assetImporter.GetType().Name} INITIATED");

						await assetImporter
							.Import()
							.ThrowExceptions<IResourceVariantData, OpenGLDrawTestCubeModule>(context.Logger);

						logger?.Log<OpenGLDrawTestCubeModule>($"IMPORTING {assetImporter.GetType().Name} FINISHED");
					};

				assetImportersTasks.Add(load());

				//assetImportersTasks.Add(assetImporter
				//	.Import());
			}

			await Task
				.WhenAll(assetImportersTasks)
				.ThrowExceptions<OpenGLDrawTestCubeModule>(context.Logger);

			meshOpenGL = context.RuntimeResourceManager
				.GetResource(
					meshResourceID.SplitAddressBySeparator())
				.GetVariant(
					MeshOpenGLAssetImporter.MESH_OPENGL_VARIANT_ID)
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