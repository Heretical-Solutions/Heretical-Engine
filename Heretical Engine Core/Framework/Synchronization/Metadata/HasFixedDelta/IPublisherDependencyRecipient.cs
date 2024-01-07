using HereticalSolutions.Delegates;

namespace HereticalSolutions.Synchronization
{
	public interface IPublisherDependencyRecipient<TDelta>
	{
		IPublisherSingleArgGeneric<TDelta> BroadcasterAsPublisher { set; }
	}
}