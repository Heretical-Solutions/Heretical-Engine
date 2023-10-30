using System;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureRAMStorageHandle
		: IReadOnlyResourceStorageHandle
	{
		private readonly FilePathSettings filePathSettings;


		private bool allocated = false;

		private Image<Rgba32> texture = null;

		public TextureRAMStorageHandle(
			FilePathSettings filePathSettings)
		{
			this.filePathSettings = filePathSettings;


			texture = null;

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

			texture = await Image.LoadAsync<Rgba32>(filePathSettings.FullPath);

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

			texture.Dispose();

			texture = null;


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

				return texture;
			}
		}

		public TValue GetResource<TValue>()
		{
			if (!allocated)
				throw new InvalidOperationException("Resource is not allocated.");

			return (TValue)(object)texture; //DO NOT REPEAT
		}

		#endregion
	}
}