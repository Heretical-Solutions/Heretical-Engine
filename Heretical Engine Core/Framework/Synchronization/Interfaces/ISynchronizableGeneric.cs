namespace HereticalSolutions.Synchronization
{
	public interface ISynchronizableGeneric<TDelta>
	{
		SynchronizationDescriptor Descriptor { get; }

		void Toggle(bool active);

		void SetScale(float scale);

		void Synchronize(TDelta delta);
	}
}