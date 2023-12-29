using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.HereticalEngine.Application;

namespace HereticalSolutions.HereticalEngine.Modules
{
	public interface IModule
		: IGenericLifetimeable<ApplicationContext>
	{
		string Name { get; }

		void Update(
			ApplicationContext context,
			float timeDelta);

		void Draw(
			ApplicationContext context,
			float timeDelta);
	}
}