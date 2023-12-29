using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;
using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.HereticalEngine.Rendering;

using HereticalSolutions.HereticalEngine.AssetImport;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

using Silk.NET.Maths;

namespace HereticalSolutions.HereticalEngine.Modules
{
	#if FIXME

	public class OpenGLDrawTestMeshModule
		: IModule
	{
		private const string PATH_TO_SHADERS = "Shaders/";

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

			var pathToEngineAssets = pathToExe.Substring(
				0,
				pathToExe.IndexOf("/bin/"))
				+ "/Engine assets/";

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

			#region Default shader

			var defaultVertexShaderArgument = new TextFileArgument();

			defaultVertexShaderArgument.Settings = new FilePathSettings
			{
				RelativePath = $"{PATH_TO_SHADERS}Default/default.vert",
				ApplicationDataFolder = pathToEngineAssets
			};

			var defaultFragmentShaderArgument = new TextFileArgument();

			defaultFragmentShaderArgument.Settings = new FilePathSettings
			{
				RelativePath = $"{PATH_TO_SHADERS}Default/default.frag",
				ApplicationDataFolder = pathToEngineAssets
			};

			var defaultShaderImportProgress = new Progress<float>();

			defaultShaderImportProgress.ProgressChanged += (sender, value) =>
			{
				logger?.Log<OpenGLDrawTestMeshModule>
					($"SHADER IMPORT PROGRESS: {(int)(value * 100f)} %");
			};

			var importDefaultShaderTask = context.AssetImportManager.Import<ShaderOpenGLAssetImporter>(
				(importer) =>
				{
					importer.Initialize(
						$"{AssetImportConstants.ASSET_SHADER_ROOT_RESOURCE_ID}/Default shader",
						PersistenceFactory.BuildSimplePlainTextSerializer(),
						defaultVertexShaderArgument,
						defaultFragmentShaderArgument);
				}
			);

			tasks.Add(importDefaultShaderTask);

			#endregion

			#region Color shader

			var colorVertexShaderArgument = new TextFileArgument();

			colorVertexShaderArgument.Settings = new FilePathSettings
			{
				RelativePath = $"{PATH_TO_SHADERS}Color/color.vert",
				ApplicationDataFolder = pathToEngineAssets
			};

			var colorFragmentShaderArgument = new TextFileArgument();

			colorFragmentShaderArgument.Settings = new FilePathSettings
			{
				RelativePath = $"{PATH_TO_SHADERS}Color/color.frag",
				ApplicationDataFolder = pathToEngineAssets
			};

			var colorShaderImportProgress = new Progress<float>();

			colorShaderImportProgress.ProgressChanged += (sender, value) =>
			{
				logger?.Log<OpenGLDrawTestMeshModule>
					($"SHADER IMPORT PROGRESS: {(int)(value * 100f)} %");
			};

			var importColorShaderTask = context.AssetImportManager.Import<ShaderOpenGLAssetImporter>(
				(importer) =>
				{
					importer.Initialize(
						$"{AssetImportConstants.ASSET_SHADER_ROOT_RESOURCE_ID}/Color shader",
						PersistenceFactory.BuildSimplePlainTextSerializer(),
						colorVertexShaderArgument,
						colorFragmentShaderArgument);
				}
			);

			tasks.Add(importColorShaderTask);

			#endregion

			#region Depth shader

			var depthVertexShaderArgument = new TextFileArgument();

			depthVertexShaderArgument.Settings = new FilePathSettings
			{
				RelativePath = $"{PATH_TO_SHADERS}Depth/depth.vert",
				ApplicationDataFolder = pathToEngineAssets
			};

			var depthFragmentShaderArgument = new TextFileArgument();

			depthFragmentShaderArgument.Settings = new FilePathSettings
			{
				RelativePath = $"{PATH_TO_SHADERS}Depth/depth.frag",
				ApplicationDataFolder = pathToEngineAssets
			};

			var depthShaderImportProgress = new Progress<float>();

			depthShaderImportProgress.ProgressChanged += (sender, value) =>
			{
				logger?.Log<OpenGLDrawTestMeshModule>
					($"SHADER IMPORT PROGRESS: {(int)(value * 100f)} %");
			};

			var importDepthShaderTask = context.AssetImportManager.Import<ShaderOpenGLAssetImporter>(
				(importer) =>
				{
					importer.Initialize(
						$"{AssetImportConstants.ASSET_SHADER_ROOT_RESOURCE_ID}/Depth shader",
						PersistenceFactory.BuildSimplePlainTextSerializer(),
						depthVertexShaderArgument,
						depthFragmentShaderArgument);
				}
			);

			tasks.Add(importDepthShaderTask);

			#endregion

			#region Diffuse shader

			var diffuseVertexShaderArgument = new TextFileArgument();

			diffuseVertexShaderArgument.Settings = new FilePathSettings
			{
				RelativePath = $"{PATH_TO_SHADERS}Diffuse/diffuse.vert",
				ApplicationDataFolder = pathToEngineAssets
			};

			var diffuseFragmentShaderArgument = new TextFileArgument();

			diffuseFragmentShaderArgument.Settings = new FilePathSettings
			{
				RelativePath = $"{PATH_TO_SHADERS}Diffuse/diffuse.frag",
				ApplicationDataFolder = pathToEngineAssets
			};

			var diffuseShaderImportProgress = new Progress<float>();

			diffuseShaderImportProgress.ProgressChanged += (sender, value) =>
			{
				logger?.Log<OpenGLDrawTestMeshModule>
					($"SHADER IMPORT PROGRESS: {(int)(value * 100f)} %");
			};

			var importDiffuseShaderTask = context.AssetImportManager.Import<ShaderOpenGLAssetImporter>(
				(importer) =>
				{
					importer.Initialize(
						$"{AssetImportConstants.ASSET_SHADER_ROOT_RESOURCE_ID}/Diffuse shader",
						PersistenceFactory.BuildSimplePlainTextSerializer(),
						diffuseVertexShaderArgument,
						diffuseFragmentShaderArgument);
				}
			);

			tasks.Add(importDiffuseShaderTask);

			#endregion

			#region Error checker UV shader

			var errorCheckerUVVertexShaderArgument = new TextFileArgument();

			errorCheckerUVVertexShaderArgument.Settings = new FilePathSettings
			{
				RelativePath = $"{PATH_TO_SHADERS}Error checker UV/errorCheckerUV.vert",
				ApplicationDataFolder = pathToEngineAssets
			};

			var errorCheckerUVFragmentShaderArgument = new TextFileArgument();

			errorCheckerUVFragmentShaderArgument.Settings = new FilePathSettings
			{
				RelativePath = $"{PATH_TO_SHADERS}Error checker UV/errorCheckerUV.frag",
				ApplicationDataFolder = pathToEngineAssets
			};

			var errorCheckerUVShaderImportProgress = new Progress<float>();

			errorCheckerUVShaderImportProgress.ProgressChanged += (sender, value) =>
			{
				logger?.Log<OpenGLDrawTestMeshModule>
					($"SHADER IMPORT PROGRESS: {(int)(value * 100f)} %");
			};

			var importErrorCheckerUVShaderTask = context.AssetImportManager.Import<ShaderOpenGLAssetImporter>(
				(importer) =>
				{
					importer.Initialize(
						$"{AssetImportConstants.ASSET_SHADER_ROOT_RESOURCE_ID}/Error checker UV shader",
						PersistenceFactory.BuildSimplePlainTextSerializer(),
						errorCheckerUVVertexShaderArgument,
						errorCheckerUVFragmentShaderArgument);
				}
			);

			tasks.Add(importErrorCheckerUVShaderTask);

			#endregion

			#region Error checker screenspace shader

			var errorCheckerScreenspaceVertexShaderArgument = new TextFileArgument();

			errorCheckerScreenspaceVertexShaderArgument.Settings = new FilePathSettings
			{
				RelativePath = $"{PATH_TO_SHADERS}Error checker screenspace/errorCheckerScreenspace.vert",
				ApplicationDataFolder = pathToEngineAssets
			};

			var errorCheckerScreenspaceFragmentShaderArgument = new TextFileArgument();

			errorCheckerScreenspaceFragmentShaderArgument.Settings = new FilePathSettings
			{
				RelativePath = $"{PATH_TO_SHADERS}Error checker screenspace/errorCheckerScreenspace.frag",
				ApplicationDataFolder = pathToEngineAssets
			};

			var errorCheckerScreenspaceShaderImportProgress = new Progress<float>();

			errorCheckerScreenspaceShaderImportProgress.ProgressChanged += (sender, value) =>
			{
				logger?.Log<OpenGLDrawTestMeshModule>
					($"SHADER IMPORT PROGRESS: {(int)(value * 100f)} %");
			};

			var importErrorCheckerScreenspaceShaderTask = context.AssetImportManager.Import<ShaderOpenGLAssetImporter>(
				(importer) =>
				{
					importer.Initialize(
						$"{AssetImportConstants.ASSET_SHADER_ROOT_RESOURCE_ID}/Error checker screenspace shader",
						PersistenceFactory.BuildSimplePlainTextSerializer(),
						errorCheckerScreenspaceVertexShaderArgument,
						errorCheckerScreenspaceFragmentShaderArgument);
				}
			);

			tasks.Add(importErrorCheckerScreenspaceShaderTask);

			#endregion

			#region Import post processors

			//TODO: separate list of tasks for post processors
			await context
				.AssetImportManager
				.RegisterPostProcessor<MaterialAssetDescriptorAssetImporter, CreateMaterialPrototypeDescriptorPostProcessor>(
					new CreateMaterialPrototypeDescriptorPostProcessor(
						$"{AssetImportConstants.ASSET_SHADER_ROOT_RESOURCE_ID}/Error checker UV shader",
						context));

			await context
				.AssetImportManager
				.RegisterPostProcessor<MaterialAssetDescriptorAssetImporter, CreateMaterialPrototypeDescriptorPostProcessor>(
					new CreateMaterialPrototypeDescriptorPostProcessor(
					$"{AssetImportConstants.ASSET_SHADER_ROOT_RESOURCE_ID}/Diffuse shader",
					context));


			#endregion

			#endregion

			#region Model import

			var modelImportProgress = new Progress<float>();

			modelImportProgress.ProgressChanged += (sender, value) =>
			{
				logger?.Log<OpenGLDrawTestMeshModule>
					($"MODEL IMPORT PROGRESS: {(int)(value * 100f)} %");
			};

			var importModelRAMTask = context.AssetImportManager.Import<ModelRAMAssetImporter>(
				(importer) =>
				{
					importer.Initialize(
						"Knight", //"Suzanne", //"Knight",
						new FilePathSettings
						{
							RelativePath = "3D/Characters/Knight/Models/strongknight.fbx", // "3D/Characters/Suzanne/Models/suzanne_triangulated.fbx", //"3D/Characters/Knight/Models/strongknight.fbx",
							ApplicationDataFolder = pathToAssets
						});
				}
			);

			tasks.Add(importModelRAMTask);

			#endregion

			#region Model OpenGL import

			var modelOpenGLImportProgress = new Progress<float>();

			modelOpenGLImportProgress.ProgressChanged += (sender, value) =>
			{
				logger?.Log<OpenGLDrawTestMeshModule>
					($"MODEL OPENGL IMPORT PROGRESS: {(int)(value * 100f)} %");
			};

			var importModelOpenGLTask = context.AssetImportManager.Import<ModelOpenGLAssetImporter>(
				(importer) =>
				{
					importer.Initialize(
						"Knight", //"Suzanne", //"Knight",
						"Knight",
						AssetImportConstants.ASSET_3D_MODEL_RAM_VARIANT_ID);
				}
			);

			tasks.Add(importModelOpenGLTask);

			#endregion

			await Task
				.WhenAll(tasks)
				.ThrowExceptions<OpenGLDrawTestMeshModule>(logger);

			modelOpenGL = importModelOpenGLTask
				.Result
				.StorageHandle
				.GetResource<ModelOpenGL>();

			logger?.Log<OpenGLDrawTestMeshModule>("IMPORT FINISHED");
		}

		private void RenderModel(
			GL gl,
			ModelOpenGL modelOpenGL)
		{
			var camera = cameraStorageHandle.GetResource<CameraPose>();

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

			material.Textures[1].Bind(gl);

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

	#endif
}