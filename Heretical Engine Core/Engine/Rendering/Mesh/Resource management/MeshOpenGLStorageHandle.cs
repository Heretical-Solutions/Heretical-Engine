using System;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MeshOpenGLStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		private readonly IRuntimeResourceManager resourceManager = null;

		private readonly IReadOnlyResourceStorageHandle meshRAMStorageHandle = null;


		private bool allocated = false;

		private MeshOpenGL mesh = null;

		public MeshOpenGLStorageHandle(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle meshRAMStorageHandle)
		{
			this.resourceManager = resourceManager;

			this.meshRAMStorageHandle = meshRAMStorageHandle;


			mesh = null;

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

			bool result = await LoadMesh(
				resourceManager,
				meshRAMStorageHandle,
				progress);

			if (!result)
			{
				progress?.Report(1f);

				return;
			}

			allocated = true;

			progress?.Report(1f);
		}

		private async Task<bool> LoadMesh(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle meshRAMStorageHandle,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			if (!meshRAMStorageHandle.Allocated)
			{
				IProgress<float> localProgress = null;

				if (progress != null)
				{
					var localProgressInstance = new Progress<float>();

					localProgressInstance.ProgressChanged += (sender, value) =>
					{
						progress.Report(value * 0.333f);
					};

					localProgress = localProgressInstance;
				}

				await meshRAMStorageHandle.Allocate(
					localProgress);
			}

			var meshDTO = meshRAMStorageHandle.GetResource<MeshDTO>();

			progress?.Report(0.333f);

			var geometryStorageHandle = resourceManager
				.GetResource(
					meshDTO.GeometryResourceID.SplitAddressBySeparator())
				.GetVariant(
					GeometryOpenGLAssetImporter.GEOMETRY_OPENGL_VARIANT_ID)
				.StorageHandle;

			if (!geometryStorageHandle.Allocated)
			{
				IProgress<float> localProgress = null;

				if (progress != null)
				{
					var localProgressInstance = new Progress<float>();

					localProgressInstance.ProgressChanged += (sender, value) =>
					{
						progress.Report(value * 0.333f + 0.333f);
					};

					localProgress = localProgressInstance;
				}

				await geometryStorageHandle.Allocate(
					localProgress);
			}

			var geometry = geometryStorageHandle.GetResource<GeometryOpenGL>();

			progress?.Report(0.666f);


			var materialOpenGLStorageHandle = resourceManager
				.GetResource(
					meshDTO.MaterialResourceID.SplitAddressBySeparator())
				.GetVariant(
					MaterialOpenGLAssetImporter.MATERIAL_OPENGL_VARIANT_ID)
				.StorageHandle;

			if (!materialOpenGLStorageHandle.Allocated)
			{
				IProgress<float> localProgress = null;

				if (progress != null)
				{
					var localProgressInstance = new Progress<float>();

					localProgressInstance.ProgressChanged += (sender, value) =>
					{
						progress.Report(value * 0.333f + 0.666f);
					};

					localProgress = localProgressInstance;
				}

				await materialOpenGLStorageHandle.Allocate(
					localProgress);
			}

			var material = materialOpenGLStorageHandle.GetResource<MaterialOpenGL>();

			mesh = new MeshOpenGL(
				geometry,
				material);

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

			mesh = null;


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

				return mesh;
			}
		}

		public TValue GetResource<TValue>()
		{
			if (!allocated)
				throw new InvalidOperationException("Resource is not allocated.");

			return (TValue)(object)mesh; //DO NOT REPEAT
		}

		#endregion
	}
}