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
			string text = string.Empty;

			if (typeof(TValue) == typeof(string))
			{
				text = (string)(object)DTO;
			}
			else if (typeof(TValue) == typeof(string[]))
			{
				var array = (string[])(object)DTO;

				text = string.Join("\n", array);
			}
			else
			{
				var containsPlainText = DTO as IContainsPlainText;

				if (containsPlainText == null)
					throw new Exception($"[PlainTextSerializer] DTO OF TYPE {typeof(TValue).ToString()} DOES NOT IMPLEMENT IContainsPlainText");

				text = containsPlainText.Text;
			}

			if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
				throw new Exception($"[PlainTextSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

			var concreteStrategy = (IPlainTextSerializationStrategy)strategyObject;

			return concreteStrategy.Serialize(argument, text);
		}

		public bool Serialize(ISerializationArgument argument, Type DTOType, object DTO)
		{
			string text = string.Empty;

			if (DTOType == typeof(string))
			{
				text = (string)DTO;
			}
			else if (DTOType == typeof(string[]))
			{
				var array = (string[])DTO;

				text = string.Join("\n", array);
			}
			else
			{
				var containsPlainText = DTO as IContainsPlainText;

				if (containsPlainText == null)
					throw new Exception($"[PlainTextSerializer] DTO OF TYPE {DTOType.ToString()} DOES NOT IMPLEMENT IContainsPlainText");

				text = containsPlainText.Text;
			}

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

			if (typeof(TValue) == typeof(string))
			{
				DTO = (TValue)(object)text;
			}
			else if (typeof(TValue) == typeof(string[]))
			{
				var array = text.Split("\n");

				DTO = (TValue)(object)array;
			}
			else
			{
				((IContainsPlainText)DTO).Text = text;
			}

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

			if (DTOType == typeof(string))
			{
				DTO = text;
			}
			else if (DTOType == typeof(string[]))
			{
				var array = text.Split("\n");

				DTO = array;
			}
			else
			{
				((IContainsPlainText)DTO).Text = text;
			}

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