using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.HereticalEngine.Rendering.Factories
{
	public static class RendererFactory
	{
		public static CompositeRenderer BuildCompositeRenderer()
		{
			return new CompositeRenderer(
				RepositoriesFactory.BuildDictionaryRepository<string, IRenderer>());
		}
	}
}