# IAsyncNotifierSingleArgGeneric\<TArgument, TValue\>

Represents an interface for an asynchronous notifier with a single argument and a generic value. The argument represents the key to identify the value that is being seeked among all the values that are added or stored inside the source class. `TArgument` should represent the type of the key and `TValue` should represent the type of the value

## Methods

Method | Description
--- | ---
`Task<TValue> GetValueWhenNotified(TArgument argument = default, bool ignoreKey = false)` | Gets the value when notified
`Task<Task<TValue>> GetWaitForNotificationTask(TArgument argument = default, bool ignoreKey = false)` | Gets the task that waits for notification
`Task Notify(TArgument argument, TValue value)` | Notifies with the specified argument and value

## Using IAsyncNotifierSingleArgGeneric\<TArgument, TValue\>

### Await for a result

```csharp
IAsyncNotifierSingleArgGeneric<TArgument, TValue> valueAddedNotifier;

TArgument key;

var awaitForNotificationTask = await valueAddedNotifier
	.GetWaitForNotificationTask(key);
			
TValue value = await waitForNotificationTask;
```

### Notify of the value added

```csharp
IAsyncNotifierSingleArgGeneric<TArgument, TValue> valueAddedNotifier;

TArgument key;

TValue value;

await valueAddedNotifier
	.Notify(
		key,
		value);
```

## Creating IAsyncNotifierSingleArgGeneric\<TArgument, TValue\>

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

IAsyncNotifierSingleArgGeneric<TArgument, TValue> foo = NotifiersFactory.BuildAsyncNotifierSingleArgGeneric<TArgument, TValue>(loggerResolver);
```

## Implementing IAsyncNotifierSingleArgGeneric\<TArgument, TValue\>

```csharp
#region IAsyncNotifierSingleArgGeneric

public async Task<TValue> GetValueWhenNotified(
	TArgument argument = default,
	bool ignoreKey = false)
{
	TaskCompletionSource<TValue> completionSource = new TaskCompletionSource<TValue>();

	var request = new NotifyRequestSingleArgGeneric<TArgument, TValue>(
		argument,
		ignoreKey,
		completionSource);


	await semaphore.WaitAsync();

	requests.Add(request);

	semaphore.Release();


	await completionSource
		.Task
		.ThrowExceptions<TValue, AsyncNotifierSingleArgGeneric<TArgument, TValue>>(logger);

	return completionSource.Task.Result;
}

public async Task<Task<TValue>> GetWaitForNotificationTask(
	TArgument argument = default,
	bool ignoreKey = false)
{
	TaskCompletionSource<TValue> completionSource = new TaskCompletionSource<TValue>();

	var request = new NotifyRequestSingleArgGeneric<TArgument, TValue>(
		argument,
		ignoreKey,
		completionSource);


	await semaphore.WaitAsync();

	requests.Add(request);

	semaphore.Release();


	return GetValueFromCompletionSource(completionSource);
}

private async Task<TValue> GetValueFromCompletionSource(
	TaskCompletionSource<TValue> completionSource)
{
	await completionSource
		.Task
		.ThrowExceptions<TValue, AsyncNotifierSingleArgGeneric<TArgument, TValue>>(logger);

	return completionSource.Task.Result;
}

public async Task Notify(
	TArgument argument,
	TValue value)
{
	await semaphore.WaitAsync();

	for (int i = requests.Count - 1; i >= 0; i--)
	{
		var request = requests[i];

		if (request.IgnoreKey
			|| EqualityComparer<TArgument>.Default.Equals(request.Key, argument))
		{
			requests.RemoveAt(i);

			request.TaskCompletionSource.TrySetResult(value);					
		}
	}

	semaphore.Release();
}

#endregion
```
