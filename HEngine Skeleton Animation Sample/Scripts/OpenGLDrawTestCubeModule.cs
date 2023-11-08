using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Application;

using HereticalSolutions.HereticalEngine.Rendering;
using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Scenes;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;
using Silk.NET.Maths;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public class OpenGLDrawTestCubeModule
		: IModule
	{
		public struct CubeVertex
		{
			public Vector3D<float> Position;

			public Vector3D<float> Normal;
		}

		private static string CubeVertShader = @"
		#version 330 core
		layout (location = 0) in vec3 vPos;
		layout (location = 1) in vec3 vNormal;

		uniform mat4 uModel;
		uniform mat4 uView;
		uniform mat4 uProjection;

		out vec3 fNormal;
		out vec3 fPos;

		void main()
		{
			//Multiplying our uniform with the vertex position, the multiplication order here does matter.
			gl_Position = uProjection * uView * uModel * vec4(vPos, 1.0);
			//We want to know the fragment's position in World space, so we multiply ONLY by uModel and not uView or uProjection
			fPos = vec3(uModel * vec4(vPos, 1.0));
			//The Normal needs to be in World space too, but needs to account for Scaling of the object
			fNormal = mat3(transpose(inverse(uModel))) * vNormal;
		}";

		private static string CubeFragShader = @"
		#version 330 core
		out vec4 FragColor;

		void main()
		{
			FragColor = vec4(1.0f);
		}";

		private static readonly float[] CubeVertices =
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

		private static readonly uint[] CubeIndices =
		{
			0, 1, 3,
			1, 2, 3
		};

		private BufferObjectOpenGL<float> CubeVBO = null;

		private BufferObjectOpenGL<uint> CubeEBO = null;

		private VertexArrayObjectOpenGL<CubeVertex> CubeVAO = null;

		private ShaderOpenGL CubeShader = null;

		private GL gl = null;

		private IResourceStorageHandle cameraStorageHandle = null;

		private float timeAccumulator = 0f;

		private IFormatLogger logger = null;

		#region IModule

		#region IGenericLifetimeable<ApplicationContext>

		public void SetUp(
			ApplicationContext context)
		{
			if (IsSetUp)
				context.Logger.ThrowException<OpenGLDrawTestCubeModule>(
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
				context.Logger.ThrowException<OpenGLDrawTestCubeModule>(
					"MODULE SHOULD BE SET UP BEFORE BEING INITIALIZED");
			}

			if (IsInitialized)
			{
				logger.ThrowException<OpenGLDrawTestCubeModule>(
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


			CubeEBO = BufferFactory.BuildBufferOpenGL<uint>(
				gl,
				BufferTargetARB.ElementArrayBuffer);

			CubeEBO.Bind(
				gl);

			CubeEBO.Update(
				gl,
				CubeIndices);


			CubeVBO = BufferFactory.BuildBufferOpenGL<float>(
				gl,
				BufferTargetARB.ArrayBuffer);

			CubeVBO.Bind(
				gl);

			CubeVBO.Update(
				gl,
				CubeVertices);


			CubeVAO = VertexFactory.BuildVertexArrayOpenGL<CubeVertex>(
				gl);

			CubeVAO.Bind(gl);

			CubeVBO.Bind(gl);

			CubeEBO.Bind(gl);

			CubeVAO.VertexAttributePointer(
				gl,
				0,
				3,
				VertexAttribPointerType.Float,
				0);

			CubeVAO.VertexAttributePointer(
				gl,
				1,
				3,
				VertexAttribPointerType.Float,
				3);

			//The lighting shader will give our main cube it's colour multiplied by the light's intensity
			ShaderFactory.BuildShaderProgram(
				CubeVertShader,
				CubeFragShader,
				gl,
				out uint cubeShaderHandle,
				out var _1,
				out var _2,
				out var _3);

			CubeShader = new ShaderOpenGL(cubeShaderHandle);


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
			CubeVBO = null;

			CubeEBO = null;

			CubeVAO = null;

			CubeShader = null;

			gl = null;

			cameraStorageHandle = null;

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

		public unsafe void Draw(
			ApplicationContext context,
			float timeDelta)
		{
			timeAccumulator += timeDelta;

			var camera = cameraStorageHandle.GetResource<Camera>();

			CubeVAO.Bind(gl);

			CubeShader.Use(gl);

			//Slightly rotate the cube to give it an angled face to look at
			CubeShader.SetUniform(
				gl,
				"uModel",
				Matrix4X4.CreateRotationY(
					MathHelpers.DegreesToRadians(25f)));
			
			CubeShader.SetUniform(
				gl,
				"uView",
				camera.ViewMatrix);
			
			CubeShader.SetUniform(
				gl,
				"uProjection",
				camera.ProjectionMatrix);

			//CubeShader.SetUniform(gl, "viewPos", CubeCamera.Position);

			//Track the difference in time so we can manipulate variables as time changes
			//var difference = (float)(window.Time * 100);
			//var lightColor = Vector3.Zero;
			//lightColor.X = MathF.Sin(difference * 2.0f);
			//lightColor.Y = MathF.Sin(difference * 0.7f);
			//lightColor.Z = MathF.Sin(difference * 1.3f);

			//var diffuseColor = lightColor * new Vector3(0.5f);
			//var ambientColor = diffuseColor * new Vector3(0.2f);

			//LightingShader.SetUniform("light.ambient", ambientColor);
			//LightingShader.SetUniform("light.diffuse", diffuseColor); // darkened
			//LightingShader.SetUniform("light.specular", new Vector3(1.0f, 1.0f, 1.0f));
			//LightingShader.SetUniform("light.position", LampPosition);

			//FOR TEST PURPOSES
			gl.PolygonMode(
				GLEnum.FrontAndBack,
				PolygonMode.Line);

			//We're drawing with just vertices and no indicies, and it takes 36 verticies to have a six-sided textured cube
			gl.DrawArrays(PrimitiveType.Triangles, 0, 36);

			CubeShader.Use(gl);

			var difference = (float)(timeAccumulator * 100);

			//The Lamp cube is going to be a scaled down version of the normal cubes verticies moved to a different screen location
			var lampMatrix = Matrix4X4<float>.Identity;

			lampMatrix *= Matrix4X4.CreateScale(0.2f);

			lampMatrix *= Matrix4X4.CreateRotationX(MathHelpers.DegreesToRadians(difference));

			CubeShader.SetUniform(gl, "uModel", lampMatrix);
			CubeShader.SetUniform(gl, "uView", camera.ViewMatrix);
			CubeShader.SetUniform(gl, "uProjection", camera.ProjectionMatrix);

			//FOR TEST PURPOSES
			gl.PolygonMode(
				GLEnum.FrontAndBack,
				PolygonMode.Line);

			gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
		}

		#endregion
	}
}