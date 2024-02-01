using HereticalSolutions.HereticalEngine.Modules;

namespace HereticalSolutions.HereticalEngine.Application
{
	public interface IRootLifetimeManager
	{
		void SetRootLifetime(
			ILifetimeModule module);
	}
}