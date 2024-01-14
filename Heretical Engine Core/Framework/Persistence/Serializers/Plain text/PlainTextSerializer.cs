using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Serializers
{
	public class PlainTextSerializer : ISerializer
	{
		private readonly IReadOnlyObjectRepository strategyRepository;

		private readonly IFormatLogger logger;

		public PlainTextSerializer(
			IReadOnlyObjectRepository strategyRepository,
			IFormatLogger logger = null)
		{
			this.strategyRepository = strategyRepository;

			this.logger = logger;
		}

		#region ISerializer

		public bool Serialize<TValue>(
			ISerializationArgument argument,
			TValue DTO)
		{
			string text = string.Empty;

			switch (DTO)
			{
				case string s:

					text = s;

					break;

				case string[] array:

					text = string.Join("\n", array);
					
					break;

				default:
					var containsPlainText = DTO as IContainsPlainText;

					if (containsPlainText == null)
						logger?.ThrowException<PlainTextSerializer>(
							$"DTO OF TYPE {typeof(TValue).Name} DOES NOT IMPLEMENT IContainsPlainText");

					text = containsPlainText.Text;

					break;
			}

			if (!strategyRepository.TryGet(
				argument.GetType(),
				out var strategyObject))
				logger?.ThrowException<PlainTextSerializer>(
					$"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}");

			var concreteStrategy = (IPlainTextSerializationStrategy)strategyObject;

			return concreteStrategy.Serialize(argument, text);
		}

		public bool Serialize(
			ISerializationArgument argument,
			Type DTOType,
			object DTO)
		{
			string text = string.Empty;

			switch (DTO)
			{
				case string s:

					text = s;

					break;

				case string[] array:

					text = string.Join("\n", array);

					break;

				default:
					var containsPlainText = DTO as IContainsPlainText;

					if (containsPlainText == null)
						logger?.ThrowException<PlainTextSerializer>(
							$"DTO OF TYPE {DTOType.Name} DOES NOT IMPLEMENT IContainsPlainText");

					text = containsPlainText.Text;

					break;
			}

			if (!strategyRepository.TryGet(
				argument.GetType(),
				out var strategyObject))
				logger?.ThrowException<PlainTextSerializer>(
					$"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}");

			var concreteStrategy = (IPlainTextSerializationStrategy)strategyObject;

			return concreteStrategy.Serialize(argument, text);
		}

		public bool Deserialize<TValue>(
			ISerializationArgument argument,
			out TValue DTO)
		{
			DTO = (TValue)Activator.CreateInstance(typeof(TValue));

			if (!strategyRepository.TryGet(
				argument.GetType(),
				out var strategyObject))
				logger?.ThrowException<PlainTextSerializer>(
					$"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}");

			var concreteStrategy = (IPlainTextSerializationStrategy)strategyObject;

			if (!concreteStrategy.Deserialize(
				argument,
				out var text))
				return false;

			switch (DTO)
			{
				case string s:

					//DTO = (TValue)(object)text;
					DTO = (TValue)Convert.ChangeType(text, typeof(TValue));

					break;

				case string[] array:

					var resultArray = text.Split("\n");

					//DTO = (TValue)(object)resultArray;
					DTO = (TValue)Convert.ChangeType(resultArray, typeof(TValue));

					break;

				default:
					((IContainsPlainText)DTO).Text = text;

					break;
			}

			return true;
		}

		public bool Deserialize(
			ISerializationArgument argument,
			Type DTOType,
			out object DTO)
		{
			DTO = Activator.CreateInstance(DTOType);

			if (!strategyRepository.TryGet(
				argument.GetType(),
				out var strategyObject))
				logger?.ThrowException<PlainTextSerializer>(
					$"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}");

			var concreteStrategy = (IPlainTextSerializationStrategy)strategyObject;

			if (!concreteStrategy.Deserialize(
				argument,
				out var text))
				return false;

			switch (DTO)
			{
				case string s:

					DTO = text;

					break;

				case string[] array:

					var resultArray = text.Split("\n");

					DTO = resultArray;

					break;

				default:
					((IContainsPlainText)DTO).Text = text;

					break;
			}

			return true;
		}

		public void Erase(ISerializationArgument argument)
		{
			if (!strategyRepository.TryGet(
				argument.GetType(),
				out var strategyObject))
				logger?.ThrowException<PlainTextSerializer>(
					$"COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().Name}");

			var concreteStrategy = (IPlainTextSerializationStrategy)strategyObject;

			concreteStrategy.Erase(argument);
		}

		#endregion
	}
}