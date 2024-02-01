using HereticalSolutions.HereticalEngine.Modules;

namespace HereticalSolutions.HereticalEngine.Application
{
	public interface IActiveModuleRegistry
	{
		void RegisterActiveModule(IModule module);

		void UnregisterActiveModule(IModule module);
	}
}