namespace HereticalSolutions.HereticalEngine.Rendering
{
	public interface ICompositeRenderer
	{
		IEnumerable<string> ActiveRenderers { get; }

		bool RendererActive(string rendererID);

		void AddRender(
			string rendererID,
			IRenderer renderer);

		void RemoveRender(
			string rendererID);

		void RemoveAllRenderers();
	}
}