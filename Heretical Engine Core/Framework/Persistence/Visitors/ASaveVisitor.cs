using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public abstract class ASaveVisitor<TValue, TDTO> :
		ISaveVisitorGeneric<TValue, TDTO>,
		ISaveVisitor
	{
		protected readonly ILogger logger;

		public ASaveVisitor(
			ILogger logger = null)
		{
			this.logger = logger;
		}

		#region ISaveVisitorGeneric

		public abstract bool Save(
			TValue value,
			out TDTO DTO);

		#endregion

		#region ISaveVisitor

		public bool Save<TArgument>(
			TArgument value,
			out object DTO)
		{
			bool result = false;

			TDTO returnDTO = default;

			DTO = default;

			//LOL, pattern matching to the rescue of converting TArgument to TValue
			switch (value)
			{
				case TValue worldValue:

					result = Save(
						worldValue,
						out returnDTO);

					break;

				default:

					throw new Exception(
						logger.TryFormat(
							GetType(),
							$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).GetType().Name}\""));
			}

			if (!result)
			{
				return false;
			}

			DTO = returnDTO;

			return true;
		}

		public bool Save<TArgument, TDTO>(
			TArgument value,
			out TDTO DTO)
		{
			bool result = false;

			TDTO returnDTO = default;

			DTO = default;

			switch (value)
			{
				case TValue worldValue:

					result = Save(
						worldValue,
						out returnDTO);

					break;

				default:

					throw new Exception(
						logger.TryFormat(
							GetType(),
							$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).GetType().Name}\""));
			}

			if (!result)
			{
				return false;
			}

			//LOL, pattern matching to the rescue of converting TArgument to TValue
			switch (returnDTO)
			{
				case TDTO targetTypeDTO:

					DTO = targetTypeDTO;

					return true;

				default:

					throw new Exception(
						logger.TryFormat(
							GetType(),
							$"CANNOT CAST \"{typeof(TDTO).Name}\" TO \"{typeof(TDTO).GetType().Name}\""));
			}
		}

		#endregion
	}
}