#define PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Scenes;

using HereticalSolutions.HereticalEngine.Math;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ModelOpenGLStorageHandle
		: AReadOnlyResourceStorageHandle<ModelOpenGL>
	{
		private readonly string modelRAMPath;

		private readonly string modelRAMVariantID;

		public ModelOpenGLStorageHandle(
			string modelRAMPath,
			string modelRAMVariantID,
			ApplicationContext context)
			: base(
				context)
		{
			this.modelRAMPath = modelRAMPath;

			this.modelRAMVariantID = modelRAMVariantID;
		}

		protected override async Task<ModelOpenGL> AllocateResource(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			IProgress<float> localProgress = progress.CreateLocalProgress(
				0f,
				(1f / 6f));

			var modelRAMStorageHandle = await LoadDependency(
				modelRAMPath,
				modelRAMVariantID,
				localProgress)
				.ThrowExceptions<IReadOnlyResourceStorageHandle, ModelOpenGLStorageHandle>(context.Logger);

			var modelDTO = modelRAMStorageHandle.GetResource<ModelAssetDescriptor>();

			MeshOpenGL[] meshes = new MeshOpenGL[modelDTO.MeshResourcePaths.Length];

			GeometryOpenGL[] geometries = new GeometryOpenGL[modelDTO.GeometryResourcePaths.Length];

			MaterialOpenGL[] materials = new MaterialOpenGL[modelDTO.MaterialResourcePaths.Length];

			TextureOpenGL[] textures = new TextureOpenGL[modelDTO.TextureResourcePaths.Length];

			progress?.Report(1f / 6f);

#if PARALLELIZE_AWAITING_FOR_RESOURCE_DEPENDENCIES
			List<Task> loadDependencyTasks = new List<Task>();

			List<float> loadDependencyProgresses = new List<float>();

			ScheduleLoading<MeshOpenGL>(
				meshes,
				modelDTO.MeshResourcePaths,
				MeshOpenGLAssetImporter.MESH_OPENGL_VARIANT_ID,

				loadDependencyTasks,
				loadDependencyProgresses,

				(1f / 6f),
				(5f / 6f),
				progress);

			ScheduleLoading<GeometryOpenGL>(
				geometries,
				modelDTO.GeometryResourcePaths,
				GeometryOpenGLAssetImporter.GEOMETRY_OPENGL_VARIANT_ID,

				loadDependencyTasks,
				loadDependencyProgresses,

				(1f / 6f),
				(5f / 6f),
				progress);

			ScheduleLoading<MaterialOpenGL>(
				materials,
				modelDTO.MaterialResourcePaths,
				MaterialOpenGLAssetImporter.MATERIAL_OPENGL_VARIANT_ID,

				loadDependencyTasks,
				loadDependencyProgresses,

				(1f / 6f),
				(5f / 6f),
				progress);

			ScheduleLoading<TextureOpenGL>(
				textures,
				modelDTO.TextureResourcePaths,
				TextureOpenGLAssetImporter.TEXTURE_OPENGL_VARIANT_ID,

				loadDependencyTasks,
				loadDependencyProgresses,

				(1f / 6f),
				(5f / 6f),
				progress);


			await Task
				.WhenAll(loadDependencyTasks)
				.ThrowExceptions<ModelOpenGLStorageHandle>(context.Logger);

#else

			await Load<MeshOpenGL>(
				meshes,
				modelDTO.MeshResourcePaths,
				MeshOpenGLAssetImporter.MESH_OPENGL_VARIANT_ID,
				(1f / 6f),
				(2f / 6f),
				progress)
				.ThrowExceptions<ModelOpenGLStorageHandle>(context.Logger);

			progress?.Report(2f / 6f);

			await Load<GeometryOpenGL>(
				geometries,
				modelDTO.GeometryResourcePaths,
				GeometryOpenGLAssetImporter.GEOMETRY_OPENGL_VARIANT_ID,
				(2f / 6f),
				(3f / 6f),
				progress)
				.ThrowExceptions<ModelOpenGLStorageHandle>(context.Logger);

			progress?.Report(3f / 6f);

			await Load<MaterialOpenGL>(
				materials,
				modelDTO.MaterialResourcePaths,
				MaterialOpenGLAssetImporter.MATERIAL_OPENGL_VARIANT_ID,
				(3f / 6f),
				(4f / 6f),
				progress)
				.ThrowExceptions<ModelOpenGLStorageHandle>(context.Logger);

			progress?.Report(4f / 6f);

			await Load<TextureOpenGL>(
				textures,
				modelDTO.TextureResourcePaths,
				TextureOpenGLAssetImporter.TEXTURE_OPENGL_VARIANT_ID,
				(4f / 6f),
				(5f / 6f),
				progress)
				.ThrowExceptions<ModelOpenGLStorageHandle>(context.Logger);

			progress?.Report(5f / 6f);

#endif

			ModelNodeOpenGL rootNode = await BuildModelNodeOpenGL(
				modelDTO.RootNode)
				.ThrowExceptions<ModelNodeOpenGL, ModelOpenGLStorageHandle>(context.Logger);

			var model = new ModelOpenGL(
				meshes,
				geometries,
				materials,
				textures,
				rootNode);

			progress?.Report(1f);

			return model;
		}

		private async Task Load<TResource>(
			TResource[] resourceCollection,
			string[] sourceCollection,
			string variantID,
			float totalProgressStart,
			float totalProgressEnd,
			IProgress<float> progress)
		{
			for (int i = 0; i < resourceCollection.Length; i++)
			{
				var resourceStorageHandle = context
					.RuntimeResourceManager
					.GetResource(
						sourceCollection[i].SplitAddressBySeparator())
					.GetVariant(
						variantID)
					.StorageHandle;

				if (!resourceStorageHandle.Allocated)
				{
					IProgress<float> localProgress = progress.CreateLocalProgress(
						totalProgressStart,
						totalProgressEnd,
						i,
						resourceCollection.Length);

					await resourceStorageHandle
						.Allocate(
							localProgress)
						.ThrowExceptions<ModelOpenGLStorageHandle>(context.Logger);
				}

				resourceCollection[i] = resourceStorageHandle.GetResource<TResource>();
			}
		}

		private void ScheduleLoading<TResource>(
			TResource[] resourceCollection,
			string[] sourceCollection,
			string variantID,

			List<Task> loadDependencyTasks,
			List<float> loadDependencyProgresses,

			float totalProgressStart,
			float totalProgressEnd,
			IProgress<float> progress)
		{
			for (int i = 0; i < resourceCollection.Length; i++)
			{
				int iClosure = i;

				Func<Task> loadResource = async () =>
				{
					IProgress<float> localProgress = progress.CreateLocalProgress(
						totalProgressStart,
						totalProgressEnd,
						loadDependencyProgresses,
						loadDependencyProgresses.Count);

					var resourceStorageHandle = await ((IContainsDependencyResources)context.RuntimeResourceManager)
						.LoadDependency(
							sourceCollection[iClosure],
							variantID,
							localProgress)
						.ThrowExceptions<IReadOnlyResourceStorageHandle, ModelOpenGLStorageHandle>(
							context.Logger);

					resourceCollection[iClosure] = resourceStorageHandle.GetResource<TResource>();

					//context.Logger?.Log<ConcurrentModelOpenGLStorageHandle>(
					//	$"LOADED {sourceCollection[iClosure]} VARIANT {variantID}");
				};

				loadDependencyTasks.Add(loadResource());
			}
		}

		private async Task<ModelNodeOpenGL> BuildModelNodeOpenGL(
			ModelNodeDescriptor dto)
		{
			ModelNodeOpenGL[] children = new ModelNodeOpenGL[dto.Children.Length];

			for (int i = 0; i < children.Length; i++)
			{
				children[i] = await BuildModelNodeOpenGL(
					dto.Children[i])
					.ThrowExceptions<ModelNodeOpenGL, ModelOpenGLStorageHandle>(context.Logger);
			}

			Transform transform = new Transform
			{
				Position = dto.Transform.Position.ToSilkNetVector3D(),
				Rotation = dto.Transform.Rotation.ToSilkNetQuaternion(),
				Scale = dto.Transform.Scale.ToSilkNetVector3D()
			};

			transform.RecalculateTRSMatrix();

			MeshOpenGL[] meshes = new MeshOpenGL[dto.MeshResourcePaths.Length];

			await Load<MeshOpenGL>(
				meshes,
				dto.MeshResourcePaths,
				MeshOpenGLAssetImporter.MESH_OPENGL_VARIANT_ID,
				-1f,
				-1f,
				null)
				.ThrowExceptions<ModelOpenGLStorageHandle>(context.Logger);

			var result = new ModelNodeOpenGL(
				dto.Name,
				children,
				transform,
				meshes);

			for (int i = 0; i < children.Length; i++)
			{
				children[i].SetParent(result);
			}

			return result;
		}

		protected override async Task FreeResource(
			ModelOpenGL resource,
			IProgress<float> progress = null)
		{
			progress?.Report(1f);
		}
	}
}