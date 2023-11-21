#define PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Application;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class GeometryOpenGLStorageHandle
		: AReadOnlyResourceStorageHandle<GeometryOpenGL>
	{
		private readonly string glPath;

		private readonly string shaderOpenGLPath;

		private readonly string shaderOpenGLVariantID;

		private readonly string geometryRAMPath;

		private readonly string geometryRAMVariantID;

		public GeometryOpenGLStorageHandle(
			string glPath,
			string shaderOpenGLPath,
			string shaderOpenGLVariantID,
			string geometryRAMPath,
			string geometryRAMVariantID,
			ApplicationContext context)
			: base(
				context)
		{
			this.glPath = glPath;

			this.shaderOpenGLPath = shaderOpenGLPath;

			this.shaderOpenGLVariantID = shaderOpenGLVariantID;

			this.geometryRAMPath = geometryRAMPath;

			this.geometryRAMVariantID = geometryRAMVariantID;
		}

		protected override async Task<GeometryOpenGL> AllocateResource(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			IReadOnlyResourceStorageHandle glStorageHandle = null;

			IReadOnlyResourceStorageHandle shaderOpenGLStorageHandle = null;

			IReadOnlyResourceStorageHandle geometryRAMStorageHandle = null;

#if PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES
			List<Task> loadDependencyTasks = new List<Task>();

			List<float> loadDependencyProgresses = new List<float>();

			IProgress<float> glLoadProgress = progress.CreateLocalProgress(
				0f,
				0.75f,
				loadDependencyProgresses,
				0);

			Func<Task> loadGL = async () =>
			{
				glStorageHandle = await LoadDependency(
					glPath,
					string.Empty,
					glLoadProgress)
					.ThrowExceptions<IReadOnlyResourceStorageHandle, GeometryOpenGLStorageHandle>(context.Logger);
			};

			loadDependencyTasks.Add(loadGL());

			IProgress<float> shaderOpenGLLoadProgress = progress.CreateLocalProgress(
				0f,
				0.75f,
				loadDependencyProgresses,
				1);

			Func<Task> loadShaderOpenGL = async () =>
			{
				shaderOpenGLStorageHandle = await LoadDependency(
					shaderOpenGLPath,
					shaderOpenGLVariantID,
					shaderOpenGLLoadProgress)
					.ThrowExceptions<IReadOnlyResourceStorageHandle, GeometryOpenGLStorageHandle>(context.Logger);
			};

			loadDependencyTasks.Add(loadShaderOpenGL());

			IProgress<float> geometryRAMLoadProgress = progress.CreateLocalProgress(
				0f,
				0.75f,
				loadDependencyProgresses,
				2);

			Func<Task> loadGeometryRAM = async () =>
			{
				geometryRAMStorageHandle = await LoadDependency(
					geometryRAMPath,
					geometryRAMVariantID,
					geometryRAMLoadProgress)
					.ThrowExceptions<IReadOnlyResourceStorageHandle, GeometryOpenGLStorageHandle>(context.Logger);
			};

			loadDependencyTasks.Add(loadGeometryRAM());


			await Task
				.WhenAll(loadDependencyTasks)
				.ThrowExceptions<GeometryOpenGLStorageHandle>(context.Logger);
#else
			IProgress<float> localProgress = progress.CreateLocalProgress(
				0f,
				0.25f);

			glStorageHandle = await LoadDependency(
				glPath,
				string.Empty,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, GeometryOpenGLStorageHandle>(context.Logger);

			progress?.Report(0.25f);

			localProgress = progress.CreateLocalProgress(
				0.25f,
				0.5f);

			shaderOpenGLStorageHandle = await LoadDependency(
				shaderOpenGLPath,
				shaderOpenGLVariantID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, GeometryOpenGLStorageHandle>(context.Logger);

			progress?.Report(0.5f);

			localProgress = progress.CreateLocalProgress(
				0.5f,
				0.75f);

			geometryRAMStorageHandle = await LoadDependency(
				geometryRAMPath,
				geometryRAMVariantID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, GeometryOpenGLStorageHandle>(context.Logger);
#endif

			GL gl = glStorageHandle.GetResource<GL>();

			ShaderOpenGL shaderOpenGL = shaderOpenGLStorageHandle.GetResource<ShaderOpenGL>();

			Geometry geometryRAM = geometryRAMStorageHandle.GetResource<Geometry>();

			progress?.Report(0.75f);

			GeometryOpenGL geometryOpenGL = null;

			Action buildShaderDelegate = () =>
			{
				geometryOpenGL = GeometryFactory.BuildGeometryOpenGL(
					gl,
					geometryRAMStorageHandle.GetResource<Geometry>(),
					shaderOpenGLStorageHandle.GetResource<ShaderOpenGL>().Descriptor,
					context.Logger);
			};

			await ExecuteOnMainThread(
				buildShaderDelegate)
				.ThrowExceptions<GeometryOpenGLStorageHandle>(context.Logger);

			progress?.Report(1f);

			return geometryOpenGL;
		}

		protected override async Task FreeResource(
			GeometryOpenGL resource,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			IProgress<float> localProgress = progress.CreateLocalProgress(
				0f,
				0.5f);

			var glStorageHandle = await LoadDependency(
				glPath,
				string.Empty,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, GeometryOpenGLStorageHandle>(context.Logger);

			GL gl = glStorageHandle.GetResource<GL>();

			progress?.Report(0.5f);

			//resource.Dispose(gl);

			Action deleteShaderDelegate = () =>
			{
				resource.Dispose(gl);
			};

			await ExecuteOnMainThread(
				deleteShaderDelegate)
				.ThrowExceptions<GeometryOpenGLStorageHandle>(context.Logger);

			progress?.Report(1f);
		}
	}
}