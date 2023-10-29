using System;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;

using Silk.NET.OpenGL;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class MeshOpenGLStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		private readonly IReadOnlyResourceStorageHandle meshRAMStorageHandle = null;

		private readonly GL cachedGL = default;


		private bool allocated = false;

		private MeshOpenGL mesh = null;

		public MeshOpenGLStorageHandle(
			IReadOnlyResourceStorageHandle meshRAMStorageHandle,
			GL gl)
		{
			this.meshRAMStorageHandle = meshRAMStorageHandle;

			cachedGL = gl;


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

			if (!meshRAMStorageHandle.Allocated)
			{
				IProgress<float> localProgress = null;

				if (progress != null)
				{
					var localProgressInstance = new Progress<float>();

					localProgressInstance.ProgressChanged += (sender, value) =>
					{
						progress.Report(value);
					};

					localProgress = localProgressInstance;
				}

				await meshRAMStorageHandle.Allocate(
					localProgress);
			}

			mesh = MeshFactory.BuildMeshOpenGL(
				cachedGL,
				meshRAMStorageHandle.GetResource<Mesh>());


			allocated = true;

			progress?.Report(1f);
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

			mesh.Dispose(cachedGL);

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