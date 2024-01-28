using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.HereticalEngine.Application
{
	public interface ILifetimeComposer
	{
		ILifetimeable RootLifetime { get; }

		ILifetimeable CurrentLifetime { get; }

		void NestLifetime(ILifetimeable lifetime);

		void SetLifetimeAsCurrent(
			ILifetimeable lifetime,
			bool root = false);
	}
}