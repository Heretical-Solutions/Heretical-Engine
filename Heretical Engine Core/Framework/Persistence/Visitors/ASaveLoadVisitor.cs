using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public abstract class ASaveLoadVisitor<TValue, TDTO> :
		ILoadVisitorGeneric<TValue, TDTO>,
		ILoadVisitor,
		ISaveVisitorGeneric<TValue, TDTO>,
		ISaveVisitor
	{
		protected readonly IFormatLogger logger; 

		public ASaveLoadVisitor(IFormatLogger logger)
		{
			this.logger = logger;
		}

		#region ILoadVisitorGeneric

		public abstract bool Load(
			TDTO DTO,
			out TValue value);

		public abstract bool Load(
			TDTO DTO,
			TValue valueToPopulate);

		#endregion

		#region ILoadVisitor

		public bool Load<TArgument>(
			object DTO,
			out TArgument value)
		{
			bool result = false;

			TValue returnValue = default;

			value = default;

			switch (DTO)
			{
				case TDTO targetTypeDTO:

					result = Load(
						targetTypeDTO,
						out returnValue);

					break;

				default:

					logger?.ThrowException(
						GetType(),
						$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TDTO).Name}\" RECEIVED: \"{DTO.GetType().Name}\"");

					break;
			}

			if (!result)
			{
				return false;
			}

			//LOL, pattern matching to the rescue of converting TArgument to TValue
			switch (returnValue)
			{
				case TArgument tArgumentReturnValue:

					value = tArgumentReturnValue;

					return true;

				default:

					logger?.ThrowException(
						GetType(),
						$"CANNOT CAST RETURN VALUE TYPE \"{typeof(TValue).Name}\" TO TYPE \"{typeof(TArgument).GetType().Name}\"");

					return false;
			}
		}

		public bool Load<TArgument, TDTO>(
			TDTO DTO,
			out TArgument value)
		{
			bool result = false;

			TValue returnValue = default;

			value = default;

			switch (DTO)
			{
				case TDTO targetTypeDTO:

					result = Load(
						targetTypeDTO,
						out returnValue);

					break;

				default:

					logger?.ThrowException(
						GetType(),
						$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TDTO).Name}\" RECEIVED: \"{typeof(TDTO).Name}\"");

					break;
			}

			if (!result)
			{
				return false;
			}

			//LOL, pattern matching to the rescue of converting TArgument to TValue
			switch (returnValue)
			{
				case TArgument tArgumentReturnValue:

					value = tArgumentReturnValue;

					break;

				default:

					logger?.ThrowException(
						GetType(),
						$"CANNOT CAST RETURN VALUE TYPE \"{typeof(TValue).Name}\" TO TYPE \"{typeof(TArgument).Name}\"");

					break;
			}

			return result;
		}

		public bool Load<TArgument>(
			object DTO,
			TArgument valueToPopulate)
		{
			TDTO dtoToLoad = default;

			switch (DTO)
			{
				case TDTO targetTypeDTO:

					dtoToLoad = targetTypeDTO;

					break;

				default:

					logger?.ThrowException(
						GetType(),
						$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TDTO).Name}\" RECEIVED: \"{DTO.GetType().Name}\"");

					break;
			}

			//LOL, pattern matching to the rescue of converting TArgument to TValue
			switch (valueToPopulate)
			{
				case TValue world:

					return Load(
						dtoToLoad,
						world);

				default:

					logger?.ThrowException(
						GetType(),
						$"CANNOT CAST RETURN VALUE TYPE \"{typeof(TValue).Name}\" TO TYPE \"{typeof(TArgument).Name}\"");

					return false;
			}
		}

		public bool Load<TArgument, TDTO>(
			TDTO DTO,
			TArgument valueToPopulate)
		{
			TDTO dtoToLoad = default;

			switch (DTO)
			{
				case TDTO targetTypeDTO:

					dtoToLoad = targetTypeDTO;

					break;

				default:

					logger?.ThrowException(
						GetType(),
						$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TDTO).Name}\" RECEIVED: \"{typeof(TDTO).Name}\"");

					break;
			}

			//LOL, pattern matching to the rescue of converting TArgument to TValue
			switch (valueToPopulate)
			{
				case TValue world:

					return Load(
						dtoToLoad,
						world);

				default:

					logger?.ThrowException(
						GetType(),
						$"CANNOT CAST RETURN VALUE TYPE \"{typeof(TValue).Name}\" TO TYPE \"{typeof(TArgument).Name}\"");

					return false;
			}
		}

		#endregion

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