using System;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Scenes;
using HereticalSolutions.HereticalEngine.Math;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ConcurrentModelOpenGLStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		private readonly IRuntimeResourceManager resourceManager = null;

		private readonly IReadOnlyResourceStorageHandle modelRAMStorageHandle = null;

		private readonly SemaphoreSlim semaphore;

		private readonly IFormatLogger logger;


		private bool allocated = false;

		private ModelOpenGL model = null;

		public ConcurrentModelOpenGLStorageHandle(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle modelRAMStorageHandle,
			SemaphoreSlim semaphore,
			IFormatLogger logger)
		{
			this.resourceManager = resourceManager;

			this.modelRAMStorageHandle = modelRAMStorageHandle;

			this.semaphore = semaphore;

			this.logger = logger;


			model = null;

			allocated = false;
		}

		#region IReadOnlyResourceStorageHandle

		#region IAllocatable

		public bool Allocated
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					return allocated;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		public virtual async Task Allocate(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				if (allocated)
				{
					progress?.Report(1f);

					return;
				}

				logger?.Log<ConcurrentModelOpenGLStorageHandle>(
					$"ALLOCATING");

				bool result = await LoadModel(
					resourceManager,
					modelRAMStorageHandle,
					progress)
					.ThrowExceptions<bool, ModelOpenGLStorageHandle>(logger);

				if (!result)
				{
					progress?.Report(1f);

					return;
				}

				allocated = true;

				logger?.Log<ConcurrentModelOpenGLStorageHandle>(
					$"ALLOCATED");
			}
			finally
			{
				semaphore.Release(); // Release the semaphore

				progress?.Report(1f);
			}
		}

		private async Task<bool> LoadModel(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle modelRAMStorageHandle,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			if (!modelRAMStorageHandle.Allocated)
			{
				IProgress<float> localProgress = progress.CreateLocalProgress(
					0f,
					(1f / 6f));

				await modelRAMStorageHandle
					.Allocate(
						localProgress)
					.ThrowExceptions<ModelOpenGLStorageHandle>(logger);
			}

			var modelDTO = modelRAMStorageHandle.GetResource<ModelDTO>();

			progress?.Report(1f / 6f);

			MeshOpenGL[] meshes = new MeshOpenGL[modelDTO.MeshResourceIDs.Length];

			await Load<MeshOpenGL>(
				meshes,
				modelDTO.MeshResourceIDs,
				MeshOpenGLAssetImporter.MESH_OPENGL_VARIANT_ID,
				(1f / 6f),
				(2f / 6f),
				progress)
				.ThrowExceptions<ModelOpenGLStorageHandle>(logger);

			progress?.Report(2f / 6f);

			GeometryOpenGL[] geometries = new GeometryOpenGL[modelDTO.GeometryResourceIDs.Length];

			await Load<GeometryOpenGL>(
				geometries,
				modelDTO.GeometryResourceIDs,
				GeometryOpenGLAssetImporter.GEOMETRY_OPENGL_VARIANT_ID,
				(2f / 6f),
				(3f / 6f),
				progress)
				.ThrowExceptions<ModelOpenGLStorageHandle>(logger);

			progress?.Report(3f / 6f);

			MaterialOpenGL[] materials = new MaterialOpenGL[modelDTO.MaterialResourceIDs.Length];

			await Load<MaterialOpenGL>(
				materials,
				modelDTO.MaterialResourceIDs,
				MaterialOpenGLAssetImporter.MATERIAL_OPENGL_VARIANT_ID,
				(3f / 6f),
				(4f / 6f),
				progress)
				.ThrowExceptions<ModelOpenGLStorageHandle>(logger);

			progress?.Report(4f / 6f);

			TextureOpenGL[] textures = new TextureOpenGL[modelDTO.TextureResourceIDs.Length];

			await Load<TextureOpenGL>(
				textures,
				modelDTO.TextureResourceIDs,
				TextureOpenGLAssetImporter.TEXTURE_OPENGL_VARIANT_ID,
				(4f / 6f),
				(5f / 6f),
				progress)
				.ThrowExceptions<ModelOpenGLStorageHandle>(logger);

			progress?.Report(5f / 6f);

			ModelNodeOpenGL rootNode = await BuildModelNodeOpenGL(
				modelDTO.RootNode)
				.ThrowExceptions<ModelNodeOpenGL, ModelOpenGLStorageHandle>(logger);

			model = new ModelOpenGL(
				meshes,
				geometries,
				materials,
				textures,
				rootNode);

			progress?.Report(1f);

			return true;
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
				var resourceStorageHandle = resourceManager
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
						.ThrowExceptions<ModelOpenGLStorageHandle>(logger);
				}

				resourceCollection[i] = resourceStorageHandle.GetResource<TResource>();
			}
		}

		private async Task<ModelNodeOpenGL> BuildModelNodeOpenGL(
			ModelNodeDTO dto)
		{
			ModelNodeOpenGL[] children = new ModelNodeOpenGL[dto.Children.Length];

			for (int i = 0; i < children.Length; i++)
			{
				children[i] = await BuildModelNodeOpenGL(
					dto.Children[i])
					.ThrowExceptions<ModelNodeOpenGL, ModelOpenGLStorageHandle>(logger);
			}

			Transform transform = new Transform
			{
				Position = dto.Transform.Position.ToSilkNetVector3D(),
				Rotation = dto.Transform.Rotation.ToSilkNetQuaternion(),
				Scale = dto.Transform.Scale.ToSilkNetVector3D()
			};

			transform.RecalculateTRSMatrix();

			MeshOpenGL[] meshes = new MeshOpenGL[dto.MeshResourceIDs.Length];

			await Load<MeshOpenGL>(
				meshes,
				dto.MeshResourceIDs,
				MeshOpenGLAssetImporter.MESH_OPENGL_VARIANT_ID,
				-1f,
				-1f,
				null)
				.ThrowExceptions<ModelOpenGLStorageHandle>(logger);

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

		public virtual async Task Free(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				if (!allocated)
				{
					progress?.Report(1f);

					return;
				}

				logger?.Log<ConcurrentModelOpenGLStorageHandle>(
					$"FREEING");

				model = null;


				allocated = false;

				logger?.Log<ConcurrentModelOpenGLStorageHandle>(
					$"FREE");
			}
			finally
			{
				semaphore.Release(); // Release the semaphore

				progress?.Report(1f);
			}
		}

		#endregion

		public object RawResource
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					if (!allocated)
						throw new InvalidOperationException("Resource is not allocated");

					return model;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		public TValue GetResource<TValue>()
		{
			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!allocated)
					throw new InvalidOperationException("Resource is not allocated");

				return (TValue)(object)model;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		#endregion
	}
}