using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Application;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	#if FIXME

	public class ConcurrentGeometryOpenGLStorageHandle
		: AConcurrentReadOnlyResourceStorageHandle<GeometryOpenGL>
	{
		private readonly string glResourcePath;


		private readonly string shaderOpenGLResourcePath;

		private readonly string shaderOpenGLResourceVariantID;


		private readonly string geometryRAMResourcePath;

		private readonly string geometryRAMResourceVariantID;

		public ConcurrentGeometryOpenGLStorageHandle(
			string glResourcePath,

			string shaderOpenGLResourcePath,
			string shaderOpenGLResourceVariantID,

			string geometryRAMResourcePath,
			string geometryRAMResourceVariantID,

			SemaphoreSlim semaphore,
			ApplicationContext context)
			: base(
				semaphore,
				context)
		{
			this.glResourcePath = glResourcePath;


			this.shaderOpenGLResourcePath = shaderOpenGLResourcePath;

			this.shaderOpenGLResourceVariantID = shaderOpenGLResourceVariantID;


			this.geometryRAMResourcePath = geometryRAMResourcePath;

			this.geometryRAMResourceVariantID = geometryRAMResourceVariantID;
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
					glResourcePath,
					string.Empty,
					glLoadProgress)
					.ThrowExceptions<IReadOnlyResourceStorageHandle, ConcurrentGeometryOpenGLStorageHandle>(context.Logger);
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
					shaderOpenGLResourcePath,
					shaderOpenGLResourceVariantID,
					shaderOpenGLLoadProgress)
					.ThrowExceptions<IReadOnlyResourceStorageHandle, ConcurrentGeometryOpenGLStorageHandle>(context.Logger);
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
					geometryRAMResourcePath,
					geometryRAMResourceVariantID,
					geometryRAMLoadProgress)
					.ThrowExceptions<IReadOnlyResourceStorageHandle, ConcurrentGeometryOpenGLStorageHandle>(context.Logger);
			};

			loadDependencyTasks.Add(loadGeometryRAM());


			await Task
				.WhenAll(loadDependencyTasks)
				.ThrowExceptions<ConcurrentGeometryOpenGLStorageHandle>(context.Logger);
#else
			IProgress<float> localProgress = progress.CreateLocalProgress(
				0f,
				0.25f);

			glStorageHandle = await LoadDependency(
				glResourcePath,
				string.Empty,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, ConcurrentGeometryOpenGLStorageHandle>(context.Logger);

			progress?.Report(0.25f);

			localProgress = progress.CreateLocalProgress(
				0.25f,
				0.5f);

			shaderOpenGLStorageHandle = await LoadDependency(
				shaderOpenGLResourcePath,
				shaderOpenGLResourceVariantID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, ConcurrentGeometryOpenGLStorageHandle>(context.Logger);

			progress?.Report(0.5f);

			localProgress = progress.CreateLocalProgress(
				0.5f,
				0.75f);

			geometryRAMStorageHandle = await LoadDependency(
				geometryRAMResourcePath,
				geometryRAMResourceVariantID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, ConcurrentGeometryOpenGLStorageHandle>(context.Logger);
#endif

			GL gl = glStorageHandle.GetResource<GL>();

			ShaderOpenGL shaderOpenGL = shaderOpenGLStorageHandle.GetResource<ShaderOpenGL>();

			GeometryRAM geometryRAM = geometryRAMStorageHandle.GetResource<GeometryRAM>();

			progress?.Report(0.75f);

			GeometryOpenGL geometryOpenGL = null;

			Action buildShaderDelegate = () =>
			{
				geometryOpenGL = GeometryFactory.BuildGeometryOpenGL(
					gl,
					geometryRAMStorageHandle.GetResource<GeometryRAM>(),
					shaderOpenGLStorageHandle.GetResource<ShaderOpenGL>().Descriptor,
					context.Logger);
			};

			await ExecuteOnMainThread(
				buildShaderDelegate)
				.ThrowExceptions<ConcurrentGeometryOpenGLStorageHandle>(context.Logger);

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
				glResourcePath,
				string.Empty,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, ConcurrentGeometryOpenGLStorageHandle>(context.Logger);

			GL gl = glStorageHandle.GetResource<GL>();

			progress?.Report(0.5f);

			//resource.Dispose(gl);

			Action deleteShaderDelegate = () =>
			{
				resource.Dispose(gl);
			};

			await ExecuteOnMainThread(
				deleteShaderDelegate)
				.ThrowExceptions<ConcurrentGeometryOpenGLStorageHandle>(context.Logger);

			progress?.Report(1f);
		}
	}

	#endif
}