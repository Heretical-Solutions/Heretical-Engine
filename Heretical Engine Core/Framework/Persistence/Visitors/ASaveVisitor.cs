using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public abstract class ASaveVisitor<TValue, TDTO> :
		ISaveVisitorGeneric<TValue, TDTO>,
		ISaveVisitor
	{
		protected readonly IFormatLogger logger;

		public ASaveVisitor(IFormatLogger logger)
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

					logger?.ThrowException(
						GetType(),
						$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).GetType().Name}\"");

					break;
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

					logger?.ThrowException(
						GetType(),
						$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).GetType().Name}\"");

					break;
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

					logger?.ThrowException(
						GetType(),
						$"CANNOT CAST \"{typeof(TDTO).Name}\" TO \"{typeof(TDTO).GetType().Name}\"");

					return false;
			}
		}

		#endregion
	}
}