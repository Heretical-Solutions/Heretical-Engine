using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.HereticalEngine.Application
{
	public interface ILifetimeComposer
	{
		void NestLifetime(ILifetimeable lifetime);

		void SetLifetimeAsCurrent(
			ILifetimeable lifetime,
			bool root = false);
	}
}