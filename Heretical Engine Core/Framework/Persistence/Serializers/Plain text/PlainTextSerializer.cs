using HereticalSolutions.Repositories;

namespace HereticalSolutions.Persistence.Serializers
{
	public class PlainTextSerializer : ISerializer
	{
		private readonly IReadOnlyObjectRepository strategyRepository;

		public PlainTextSerializer(IReadOnlyObjectRepository strategyRepository)
		{
			this.strategyRepository = strategyRepository;
		}

		#region ISerializer

		public bool Serialize<TValue>(ISerializationArgument argument, TValue DTO)
		{
			string text = ((IContainsPlainText)DTO).Text;

			if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
				throw new Exception($"[PlainTextSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

			var concreteStrategy = (IPlainTextSerializationStrategy)strategyObject;

			return concreteStrategy.Serialize(argument, text);
		}

		public bool Serialize(ISerializationArgument argument, Type DTOType, object DTO)
		{
			string text = ((IContainsPlainText)DTO).Text;

			if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
				throw new Exception($"[PlainTextSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

			var concreteStrategy = (IPlainTextSerializationStrategy)strategyObject;

			return concreteStrategy.Serialize(argument, text);
		}

		public bool Deserialize<TValue>(ISerializationArgument argument, out TValue DTO)
		{
			DTO = (TValue)Activator.CreateInstance(typeof(TValue));

			if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
				throw new Exception($"[PlainTextSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

			var concreteStrategy = (IPlainTextSerializationStrategy)strategyObject;

			if (!concreteStrategy.Deserialize(argument, out var text))
				return false;

			((IContainsPlainText)DTO).Text = text;

			return true;
		}

		public bool Deserialize(ISerializationArgument argument, Type DTOType, out object DTO)
		{
			DTO = Activator.CreateInstance(DTOType);

			if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
				throw new Exception($"[PlainTextSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

			var concreteStrategy = (IPlainTextSerializationStrategy)strategyObject;

			if (!concreteStrategy.Deserialize(argument, out var text))
				return false;

			((IContainsPlainText)DTO).Text = text;

			return true;
		}

		public void Erase(ISerializationArgument argument)
		{
			if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
				throw new Exception($"[PlainTextSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

			var concreteStrategy = (IPlainTextSerializationStrategy)strategyObject;

			concreteStrategy.Erase(argument);
		}

		#endregion
	}
}