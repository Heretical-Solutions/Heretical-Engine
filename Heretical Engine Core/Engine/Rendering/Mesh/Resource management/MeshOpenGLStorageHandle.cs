#define PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MeshOpenGLStorageHandle
		: AReadOnlyResourceStorageHandle<MeshOpenGL>
	{
		private readonly string meshRAMPath;

		private readonly string meshRAMVariantID;

		public MeshOpenGLStorageHandle(
			string meshRAMPath,
			string meshRAMVariantID,
			ApplicationContext context)
			: base(
				context)
		{
			this.meshRAMPath = meshRAMPath;

			this.meshRAMVariantID = meshRAMVariantID;
		}

		protected override async Task<MeshOpenGL> AllocateResource(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			IReadOnlyResourceStorageHandle meshRAMStorageHandle = null;

			IProgress<float> localProgress = progress.CreateLocalProgress(
				0f,
				0.333f);

			meshRAMStorageHandle = await LoadDependency(
				meshRAMPath,
				meshRAMVariantID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, MeshOpenGLStorageHandle>(context.Logger);

			var meshDTO = meshRAMStorageHandle.GetResource<MeshDTO>();

			IReadOnlyResourceStorageHandle geometryOpenGLStorageHandle = null;

			IReadOnlyResourceStorageHandle materialOpenGLStorageHandle = null;

			progress?.Report(0.333f);

#if PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES
			List<Task> loadDependencyTasks = new List<Task>();

			List<float> loadDependencyProgresses = new List<float>();

			IProgress<float> geometryOpenGLLoadProgress = progress.CreateLocalProgress(
				0.333f,
				1f,
				loadDependencyProgresses,
				0);

			loadDependencyTasks.Add(
				Task.Run(async () => 
					{
						geometryOpenGLStorageHandle = await LoadDependency(
							meshDTO.GeometryResourceID,
							GeometryOpenGLAssetImporter.GEOMETRY_OPENGL_VARIANT_ID,
							geometryOpenGLLoadProgress)
							.ThrowExceptions<IReadOnlyResourceStorageHandle, MeshOpenGLStorageHandle>(context.Logger);
					}));

			IProgress<float> materialOpenGLLoadProgress = progress.CreateLocalProgress(
				0.333f,
				1f,
				loadDependencyProgresses,
				1);

			loadDependencyTasks.Add(
				Task.Run(async () =>
					{
						materialOpenGLStorageHandle = await LoadDependency(
							meshDTO.MaterialResourceID,
							MaterialOpenGLAssetImporter.MATERIAL_OPENGL_VARIANT_ID,
							materialOpenGLLoadProgress)
							.ThrowExceptions<IReadOnlyResourceStorageHandle, MeshOpenGLStorageHandle>(context.Logger);
					}));

			await Task
				.WhenAll(loadDependencyTasks)
				.ThrowExceptions<MeshOpenGLStorageHandle>(context.Logger);
#else
			localProgress = progress.CreateLocalProgress(
				0.333f,
				0.666f);

			geometryOpenGLStorageHandle = await LoadDependency(
				meshDTO.GeometryResourceID,
				GeometryOpenGLAssetImporter.GEOMETRY_OPENGL_VARIANT_ID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, MeshOpenGLStorageHandle>(context.Logger);

			progress?.Report(0.666f);

			localProgress = progress.CreateLocalProgress(
				0.666f,
				1f);

			materialOpenGLStorageHandle = await LoadDependency(
				meshDTO.MaterialResourceID,
				MaterialOpenGLAssetImporter.MATERIAL_OPENGL_VARIANT_ID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, MeshOpenGLStorageHandle>(context.Logger);
#endif

			var geometry = geometryOpenGLStorageHandle.GetResource<GeometryOpenGL>();

			var material = materialOpenGLStorageHandle.GetResource<MaterialOpenGL>();

			var mesh = new MeshOpenGL(
				geometry,
				material);

			progress?.Report(1f);

			return mesh;
		}

		protected override async Task FreeResource(
			MeshOpenGL resource,
			IProgress<float> progress = null)
		{
			progress?.Report(1f);
		}
	}
}