# ISaveVisitor

Represents an interface for saving objects and converting them to corresponding data transfer objects (DTOs)

## Methods

Method | Description
--- | ---
`bool Save<TValue>(TValue value, out object DTO)` | Saves the specified value and converts it to a data transfer object
`bool Save<TValue, TDTO>(TValue value, out TDTO DTO)` | Saves the specified value and converts it to a data transfer object

## Using ISaveVisitor

```csharp
ISaveVisitor foo;

TValue bar;

if (!foo.Save<TValue>(
	bar,
	out object DTO))
{
	throw new Exception("Saving botched");
}
```

## Implementing ISaveVisitor

```csharp
#region ISaveVisitor

public bool Save<TArgument>(
	TArgument value,
	out object DTO)
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
				$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).etType().Name}\"");
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
				$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).GetType().Name}\"");
	}

	if (!result)
	{
		return false;
	}

	switch (returnDTO)
	{
		case TDTO targetTypeDTO:

			DTO = targetTypeDTO;

			return true;

		default:

			throw new Exception(
				$"CANNOT CAST \"{typeof(TDTO).Name}\" TO \"{typeof(TDTO).GetType().Name}\"");
	}
}

#endregion

```
