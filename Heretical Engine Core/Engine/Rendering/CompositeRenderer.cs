using HereticalSolutions.Repositories;

namespace HereticalSolutions.HereticalEngine.Rendering
{
	public class CompositeRenderer
		: IRenderer,
		  ICompositeRenderer
	{
		private readonly IRepository<string, IRenderer> renderersRepository;

		public CompositeRenderer(
			IRepository<string, IRenderer> renderersRepository)
		{
			this.renderersRepository = renderersRepository;
		}

		#region IRenderer

		public void Render()
		{
			foreach (var renderer in renderersRepository.Values)
			{
				renderer.Render();
			}
		}

		#endregion

		#region ICompositeRenderer

		public IEnumerable<string> ActiveRenderers { get => renderersRepository.Keys; }

		public bool RendererActive(string rendererID)
		{
			return renderersRepository.Has(rendererID);
		}

		public void AddRender(
			string rendererID,
			IRenderer renderer)
		{
			renderersRepository.TryAdd(
				rendererID,
				renderer);
		}

		public void RemoveRender(
			string rendererID)
		{
			renderersRepository.TryRemove(rendererID);
		}

		public void RemoveAllRenderers()
		{
			renderersRepository.Clear();
		}

		#endregion
	}
}