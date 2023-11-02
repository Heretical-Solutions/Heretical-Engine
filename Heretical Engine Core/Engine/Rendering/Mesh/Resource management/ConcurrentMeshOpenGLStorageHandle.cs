using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class ConcurrentMeshOpenGLStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		private readonly IRuntimeResourceManager resourceManager = null;

		private readonly IReadOnlyResourceStorageHandle meshRAMStorageHandle = null;

		private readonly ReaderWriterLockSlim readWriteLock;

		private readonly IFormatLogger logger;


		private bool allocated = false;

		private MeshOpenGL mesh = null;

		public ConcurrentMeshOpenGLStorageHandle(
			IRuntimeResourceManager resourceManager,
			IReadOnlyResourceStorageHandle meshRAMStorageHandle,
			ReaderWriterLockSlim readWriteLock,
			IFormatLogger logger)
		{
			this.resourceManager = resourceManager;

			this.meshRAMStorageHandle = meshRAMStorageHandle;

			this.readWriteLock = readWriteLock;

			this.logger = logger;


			mesh = null;

			allocated = false;
		}

		#region IReadOnlyResourceStorageHandle

		#region IAllocatable

		public bool Allocated
		{
			get
			{
				readWriteLock.EnterReadLock();

				try
				{
					return allocated;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
		}

		public virtual async Task Allocate(
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			readWriteLock.EnterWriteLock();

			try
			{
				if (allocated)
				{
					progress?.Report(1f);

					return;
				}

				bool result = await LoadMesh(
					resourceManager,
					meshRAMStorageHandle,
					progress)
					.ThrowExceptions<bool, ConcurrentMeshOpenGLStorageHandle>(logger);

				if (!result)
				{
					progress?.Report(1f);

					return;
				}

				allocated = true;
			}
			finally
			{
				readWriteLock.ExitWriteLock();

				progress?.Report(1f);
			}
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

				await meshRAMStorageHandle
					.Allocate(
						localProgress)
					.ThrowExceptions<ConcurrentMeshOpenGLStorageHandle>(logger);
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

				await geometryStorageHandle
					.Allocate(
						localProgress)
					.ThrowExceptions<ConcurrentMeshOpenGLStorageHandle>(logger);
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

				await materialOpenGLStorageHandle
					.Allocate(
						localProgress)
					.ThrowExceptions<ConcurrentMeshOpenGLStorageHandle>(logger);
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

			readWriteLock.EnterWriteLock();

			try
			{
				if (!allocated)
				{
					progress?.Report(1f);
					return;
				}

				mesh = null;

				allocated = false;
			}
			finally
			{
				readWriteLock.ExitWriteLock();

				progress?.Report(1f);
			}
		}

		#endregion

		public object RawResource
		{
			get
			{
				readWriteLock.EnterReadLock();

				try
				{
					if (!allocated)
						throw new InvalidOperationException("Resource is not allocated");

					return mesh;
				}
				finally
				{
					readWriteLock.ExitReadLock();
				}
			}
		}

		public TValue GetResource<TValue>()
		{
			readWriteLock.EnterReadLock();

			try
			{
				if (!allocated)
					throw new InvalidOperationException("Resource is not allocated");

				return (TValue)(object)mesh;
			}
			finally
			{
				readWriteLock.ExitReadLock();
			}
		}

		#endregion
	}
}