using System;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ModelOpenGLStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		private readonly IRuntimeResourceManager resourceManager = null;

		private readonly IReadOnlyResourceStorageHandle modelRAMStorageHandle = null;

		private readonly IFormatLogger logger;


		private bool allocated = false;

		private ModelOpenGL model = null;

		public ModelOpenGLStorageHandle(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle modelRAMStorageHandle,
			IFormatLogger logger)
		{
			this.resourceManager = resourceManager;

			this.modelRAMStorageHandle = modelRAMStorageHandle;

			this.logger = logger;


			model = null;

			allocated = false;
		}

		#region IReadOnlyResourceStorageHandle

		#region IAllocatable

		public bool Allocated
		{
			get => allocated;
		}

		public virtual async Task Allocate(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			if (allocated)
			{
				progress?.Report(1f);

				return;
			}

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

			progress?.Report(1f);
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
					0.333f);

				await modelRAMStorageHandle
					.Allocate(
						localProgress)
					.ThrowExceptions<ModelOpenGLStorageHandle>(logger);
			}

			var modelDTO = modelRAMStorageHandle.GetResource<ModelDTO>();

			progress?.Report(0.333f);
			
			/*
			var geometryStorageHandle = resourceManager
				.GetResource(
					modelDTO.GeometryResourceID.SplitAddressBySeparator())
				.GetVariant(
					GeometryOpenGLAssetImporter.GEOMETRY_OPENGL_VARIANT_ID)
				.StorageHandle;

			if (!geometryStorageHandle.Allocated)
			{
				IProgress<float> localProgress = progress.CreateLocalProgress(
					0.333f,
					0.666f);

				await geometryStorageHandle
					.Allocate(
						localProgress)
					.ThrowExceptions<MeshOpenGLStorageHandle>(logger);
			}

			var geometry = geometryStorageHandle.GetResource<GeometryOpenGL>();

			progress?.Report(0.666f);


			var materialOpenGLStorageHandle = resourceManager
				.GetResource(
					modelDTO.MaterialResourceID.SplitAddressBySeparator())
				.GetVariant(
					MaterialOpenGLAssetImporter.MATERIAL_OPENGL_VARIANT_ID)
				.StorageHandle;

			if (!materialOpenGLStorageHandle.Allocated)
			{
				IProgress<float> localProgress = progress.CreateLocalProgress(
					0.666f,
					1f);

				await materialOpenGLStorageHandle
					.Allocate(
						localProgress)
					.ThrowExceptions<MeshOpenGLStorageHandle>(logger);
			}

			var material = materialOpenGLStorageHandle.GetResource<MaterialOpenGL>();

			model = new MeshOpenGL(
				geometry,
				material);
			*/
			
			progress?.Report(1f);

			return true;
		}

		public virtual async Task Free(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			if (!allocated)
			{
				progress?.Report(1f);

				return;
			}

			model = null;


			allocated = false;

			progress?.Report(1f);
		}

		#endregion

		public object RawResource
		{
			get
			{
				if (!allocated)
					throw new InvalidOperationException("Resource is not allocated.");

				return model;
			}
		}

		public TValue GetResource<TValue>()
		{
			if (!allocated)
				throw new InvalidOperationException("Resource is not allocated.");

			return (TValue)(object)model; //DO NOT REPEAT
		}

		#endregion
	}
}