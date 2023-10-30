using System;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;

using Silk.NET.OpenGL;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class GeometryOpenGLStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		private readonly IReadOnlyResourceStorageHandle geometryRAMStorageHandle = null;

		private readonly GL cachedGL = default;


		private bool allocated = false;

		private GeometryOpenGL geometry = null;

		public GeometryOpenGLStorageHandle(
			IReadOnlyResourceStorageHandle geometryRAMStorageHandle,
			GL gl)
		{
			this.geometryRAMStorageHandle = geometryRAMStorageHandle;

			cachedGL = gl;


			geometry = null;

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

			if (!geometryRAMStorageHandle.Allocated)
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

				await geometryRAMStorageHandle.Allocate(
					localProgress);
			}

			geometry = GeometryFactory.BuildGeometryOpenGL(
				cachedGL,
				geometryRAMStorageHandle.GetResource<Geometry>());


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

			geometry.Dispose(cachedGL);

			geometry = null;


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

				return geometry;
			}
		}

		public TValue GetResource<TValue>()
		{
			if (!allocated)
				throw new InvalidOperationException("Resource is not allocated.");

			return (TValue)(object)geometry; //DO NOT REPEAT
		}

		#endregion
	}
}