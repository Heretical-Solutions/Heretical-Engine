using System;
using System.Threading.Tasks;

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.HereticalEngine.Rendering.Factories;

using HereticalSolutions.HereticalEngine.Messaging;

using HereticalSolutions.Logging;

using Silk.NET.OpenGL;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class GeometryOpenGLStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		private readonly IReadOnlyResourceStorageHandle geometryRAMStorageHandle = null;

		private readonly IReadOnlyResourceStorageHandle shaderStorageHandle = null;

		private readonly GL cachedGL = default;

		private readonly ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer;

		private readonly IFormatLogger logger;


		private bool allocated = false;

		private GeometryOpenGL geometry = null;

		public GeometryOpenGLStorageHandle(
			IReadOnlyResourceStorageHandle geometryRAMStorageHandle,
			IReadOnlyResourceStorageHandle shaderStorageHandle,
			GL gl,
			ConcurrentGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,
			IFormatLogger logger)
		{
			this.geometryRAMStorageHandle = geometryRAMStorageHandle;

			this.shaderStorageHandle = shaderStorageHandle;

			cachedGL = gl;

			this.mainThreadCommandBuffer = mainThreadCommandBuffer;

			this.logger = logger;


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

			logger.Log<GeometryOpenGLStorageHandle>(
				$"ALLOCATING");

			if (!geometryRAMStorageHandle.Allocated)
			{
				IProgress<float> localProgress = progress.CreateLocalProgress();

				await geometryRAMStorageHandle
					.Allocate(
						localProgress)
					.ThrowExceptions<GeometryOpenGLStorageHandle>(logger);
			}

			if (!shaderStorageHandle.Allocated)
			{
				IProgress<float> localProgress = progress.CreateLocalProgress();

				await shaderStorageHandle
					.Allocate(
						localProgress)
					.ThrowExceptions<GeometryOpenGLStorageHandle>(logger);
			}

			Action buildShaderDelegate = () =>
			{
				geometry = GeometryFactory.BuildGeometryOpenGL(
					cachedGL,
					geometryRAMStorageHandle.GetResource<Geometry>(),
					shaderStorageHandle.GetResource<ShaderOpenGL>().Descriptor,
					logger);
			};

			var command = new MainThreadCommand(
				buildShaderDelegate);

			while (!mainThreadCommandBuffer.TryProduce(
				command))
			{
				await Task.Yield();
			}

			while (command.Status != ECommandStatus.DONE)
			{
				await Task.Yield();
			}

			allocated = true;

			logger.Log<GeometryOpenGLStorageHandle>(
				$"ALLOCATED");

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

			logger.Log<GeometryOpenGLStorageHandle>(
				$"FREEING");

			//geometry.Dispose(cachedGL);

			Action deleteShaderDelegate = () =>
			{
				geometry.Dispose(cachedGL);
			};

			var command = new MainThreadCommand(
				deleteShaderDelegate);

			while (!mainThreadCommandBuffer.TryProduce(
				command))
			{
				await Task.Yield();
			}

			while (command.Status != ECommandStatus.DONE)
			{
				await Task.Yield();
			}

			geometry = null;


			allocated = false;

			logger.Log<GeometryOpenGLStorageHandle>(
				$"FREE");

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