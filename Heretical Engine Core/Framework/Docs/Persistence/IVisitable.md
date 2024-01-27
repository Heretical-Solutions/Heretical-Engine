# IVisitable

Represents an interface that allows an object to be visited by a visitor

## Methods

Method | Description
--- | ---
`Type DTOType { get; }` | Gets the type of the data transfer object (DTO) associated with the visitable object
`bool Accept<TDTO>(ISaveVisitor visitor, out TDTO DTO)` | Accepts a save visitor and returns the corresponding DTO of type `TDTO`
`bool Accept(ISaveVisitor visitor, out object DTO)` | Accepts a save visitor and returns the corresponding DTO of type `object`
`bool Accept<TDTO>(ILoadVisitor visitor, TDTO DTO)` | Accepts a load visitor and performs the visitation using the provided DTO of type `TDTO`
`bool Accept(ILoadVisitor visitor, object DTO)` | Accepts a load visitor and performs the visitation using the provided DTO of type `object`

## Using IVisitable

### Save to file

```csharp
IVisitable foo;

ISaveVisitor bar;

if (!foo.Accept<TDTO>(
	bar,
	out TDTO DTO))
{
	throw new Exception("Saving botched");
}
```

### Read from file

```csharp
IVisitable foo;

ILoadVisitor bar;

if (!foo.Accept<TDTO>(
	bar,
	out TDTO DTO))
{
	throw new Exception("Loading botched");
}
```

### Clone

```csharp
IVisitable foo;

ISaveVisitor barSave;

ILoadVisitor barLoad;

IVisitable fooClone;

if (!foo.Accept<TDTO>(
	barSave,
	out TDTO DTO))
{
	throw new Exception("Saving botched");
}

if (!fooClone.Accept<TDTO>(
	barLoad,
	DTO))
{
 throw new Exception("Loading botched");
}
```

## Implementing IVisitable

```csharp
#region IVisitable

public Type DTOType => typeof(RuntimeTimerDTO);

public bool Accept<TDTO>(
	ISaveVisitor visitor,
	out TDTO DTO)
{
	var result = visitor
		.Save<IRuntimeTimer, RuntimeTimerDTO>(
			this,
			out RuntimeTimerDTO runtimeTimerDTO);

	DTO = default;

	switch (runtimeTimerDTO)
	{
		case TDTO targetTypeDTO:

			DTO = targetTypeDTO;

			break;

		default:

			throw new Exception(
				$"CANNOT CAST RETURN VALUE TYPE \"{typeof(RuntimeTimerDTO).Name}\" TO TYPE \"{typeof(TDTO).GetType().Name}\"");
	}

	return result;
}

public bool Accept(ISaveVisitor visitor, out object DTO)
{
	var result = visitor.Save<IRuntimeTimer, RuntimeTimerDTO>(
		this,
		out RuntimeTimerDTO runtimeTimerDTO);

	DTO = runtimeTimerDTO;

	return result;
}

public bool Accept<TDTO>(
	ILoadVisitor visitor,
	TDTO DTO)
{
	switch (DTO)
	{
		case RuntimeTimerDTO targetTypeDTO:

			return visitor
				.Load<IRuntimeTimer, RuntimeTimerDTO>(
					targetTypeDTO,
					this);

		default:

			throw new Exception(
				$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(RuntimeTimerDTO).Name}\" RECEIVED: \"{typeof(TDTO).GetType().Name}\"");
	}
}

public bool Accept(ILoadVisitor visitor, object DTO)
{
	return visitor.Load<IRuntimeTimer, RuntimeTimerDTO>(
		(RuntimeTimerDTO)DTO,
		this);
}

#endregion
```
