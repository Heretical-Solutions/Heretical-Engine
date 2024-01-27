# ILoadVisitor

Represents a visitor interface for loading values from Data Transfer Objects (DTOs)

## Methods

Method | Description
--- | ---
`bool Load<TValue>(object DTO, out TValue value)` | Loads the value of type TValue from the given DTO object
`bool Load<TValue, TDTO>(TDTO DTO, out TValue value)` | Loads the value of type TValue from the given DTO object
`bool Load<TValue>(object DTO, TValue valueToPopulate)` | Loads the value of type TValue from the given DTO object and populates an existing value
`bool Load<TValue, TDTO>(TDTO DTO, TValue valueToPopulate)` | Loads the value of type TValue from the given DTO object and populates an existing value

## Using ILoadVisitor

```csharp
ILoadVisitor foo;

object bar;

if (!foo.Load<TValue>(
    bar,
    out TValue value))
{
	throw new Exception("Loading botched");
}
```

## Implementing ILoadVisitor

```csharp
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

			throw new Exception(
				$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TDTO).Name}\" RECEIVED: \"{DTO.GetType().Name}\"");
	}

	if (!result)
	{
		return false;
	}

	switch (returnValue)
	{
		case TArgument tArgumentReturnValue:

			value = tArgumentReturnValue;

			return true;

		default:

			throw new Exception(
				$"CANNOT CAST RETURN VALUE TYPE \"{typeof(TValue).Name}\" TO TYPE \"{typeof(TArgument).GetType().Name}\"");
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

			throw new Exception(
				$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TDTO).Name}\" RECEIVED: \"{typeof(TDTO).Name}\"");
	}

	if (!result)
	{
		return false;
	}

	switch (returnValue)
	{
		case TArgument tArgumentReturnValue:

			value = tArgumentReturnValue;

			break;

		default:

			throw new Exception(
				$"CANNOT CAST RETURN VALUE TYPE \"{typeof(TValue).Name}\" TO TYPE \"{typeof(TArgument).Name}\"");
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

			throw new Exception(
				$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TDTO).Name}\" RECEIVED: \"{DTO.GetType().Name}\"");
	}

	switch (valueToPopulate)
	{
		case TValue world:

			return Load(
				dtoToLoad,
				world);

		default:

			throw new Exception(
				$"CANNOT CAST RETURN VALUE TYPE \"{typeof(TValue).Name}\" TO TYPE \"{typeof(TArgument).Name}\"");
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

			throw new Exception(
				$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TDTO).Name}\" RECEIVED: \"{typeof(TDTO).Name}\"");
	}

	switch (valueToPopulate)
	{
		case TValue world:

			return Load(
				dtoToLoad,
				world);

		default:

			throw new Exception(
				$"CANNOT CAST RETURN VALUE TYPE \"{typeof(TValue).Name}\" TO TYPE \"{typeof(TArgument).Name}\"");
	}
}

#endregion
```
