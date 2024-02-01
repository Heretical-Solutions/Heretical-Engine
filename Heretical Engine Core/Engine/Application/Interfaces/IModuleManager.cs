using HereticalSolutions.HereticalEngine.Modules;

namespace HereticalSolutions.HereticalEngine.Application
{
	public interface IModuleManager
	{
		void LoadModule(
			IModule module,
			ILifetimeModule parentLifetime);

		void UnloadModule(
			IModule module);
	}
}