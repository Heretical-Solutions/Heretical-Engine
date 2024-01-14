namespace HereticalSolutions.Collections.Managed
{
	public interface IGenericCircularBuffer<TValue>
		where TValue : class
	{
		int ProducerEnd { get; }

		int ConsumerEnd { get; }

		EBufferElementState GetState(int index);

		TValue GetValue(int index);

		bool TryProduce(TValue value);

		bool TryConsume(out TValue value);
	}
}