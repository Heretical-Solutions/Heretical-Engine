using System;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class TextureRAMStorageHandle
		: IResourceStorageHandle
	{
		private readonly FileSystemSettings fsSettings;


		private bool allocated = false;

		private Image<Rgba32> texture = null;

		public TextureRAMStorageHandle(
			FileSystemSettings fsSettings)
		{
			this.fsSettings = fsSettings;


			texture = null;

			allocated = false;
		}

		#region IResourceStorageHandle

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

			texture = await Image.LoadAsync<Rgba32>(fsSettings.FullPath);

			allocated = true;

			progress?.Report(1f);
		}

		public object RawResource
		{
			get => texture;
		}

		public TValue GetResource<TValue>()
		{
			return (TValue)(object)texture;
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
	}
}