using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public interface IModule
		: ILifetimeable
	{
		string Name { get; }

		void Load(
			ApplicationContext context);

		void Unload(
			ApplicationContext context);
	}
}