using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public interface IModule
	{
		string Name { get; }

		void Load(
			IApplicationContext context,
			ILifetimeModule parentLifetime);

		void Unload(IApplicationContext context);
	}
}