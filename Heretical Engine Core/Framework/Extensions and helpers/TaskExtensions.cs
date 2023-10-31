using System.Runtime.CompilerServices;

using System.Text;

using HereticalSolutions.Logging;

namespace HereticalSolutions
{
	public static class TaskExtensions
	{
		#region Task

		public static ConfiguredTaskAwaitable LogExceptions(this Task task)
		{
			return task
				.ContinueWith(
					failedTask =>
					{
						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in failedTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						Console.WriteLine($"{failedTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");
					},
					TaskContinuationOptions.OnlyOnFaulted)
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable ThrowExceptions(this Task task)
		{
			return task
				.ContinueWith(
					failedTask =>
					{
						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in failedTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						throw new Exception($"{failedTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");
					},
					TaskContinuationOptions.OnlyOnFaulted)
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable LogExceptions<TSource>(
			this Task task,
			IFormatLogger logger)
		{
			return task
				.ContinueWith(
					failedTask =>
					{
						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in failedTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger.LogError<TSource>(
							$"{failedTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");
					},
					TaskContinuationOptions.OnlyOnFaulted)
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable ThrowExceptions<TSource>(
			this Task task,
			IFormatLogger logger)
		{
			return task
				.ContinueWith(
					failedTask =>
					{
						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in failedTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger.ThrowException<TSource>(
							$"{failedTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");
					},
					TaskContinuationOptions.OnlyOnFaulted)
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable LogExceptions(
			this Task task,
			Type logSource,
			IFormatLogger logger)
		{
			return task
				.ContinueWith(
					failedTask =>
					{
						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in failedTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger.LogError(
							logSource,
							$"{failedTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");
					},
					TaskContinuationOptions.OnlyOnFaulted)
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable ThrowExceptions(
			this Task task,
			Type logSource,
			IFormatLogger logger)
		{
			return task
				.ContinueWith(
					failedTask =>
					{
						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in failedTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger.ThrowException(
							logSource,
							$"{failedTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");
					},
					TaskContinuationOptions.OnlyOnFaulted)
				.ConfigureAwait(false);
		}

		#endregion

		#region Task<T>

		public static ConfiguredTaskAwaitable<T> LogExceptions<T>(this Task<T> task)
		{
			return task
				.ContinueWith<T>(
					failedTask =>
					{
						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in failedTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						Console.WriteLine($"{failedTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");

						return default;
					},
					TaskContinuationOptions.OnlyOnFaulted)
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable<T> ThrowExceptions<T>(this Task<T> task)
		{
			return task
				.ContinueWith<T>(
					failedTask =>
					{
						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in failedTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						throw new Exception($"{failedTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");
					},
					TaskContinuationOptions.OnlyOnFaulted)
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable<T> LogExceptions<T, TSource>(
			this Task<T> task,
			IFormatLogger logger)
		{
			return task
				.ContinueWith<T>(
					failedTask =>
					{
						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in failedTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger.LogError<TSource>(
							$"{failedTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");

						return default;
					},
					TaskContinuationOptions.OnlyOnFaulted)
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable<T> ThrowExceptions<T, TSource>(
			this Task<T> task,
			IFormatLogger logger)
		{
			return task
				.ContinueWith<T>(
					failedTask =>
					{
						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in failedTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger.ThrowException<TSource>(
							$"{failedTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");

						return default;
					},
					TaskContinuationOptions.OnlyOnFaulted)
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable<T> LogExceptions<T>(
			this Task<T> task,
			Type logSource,
			IFormatLogger logger)
		{
			return task
				.ContinueWith<T>(
					failedTask =>
					{
						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in failedTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger.LogError(
							logSource,
							$"{failedTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");

						return default;
					},
					TaskContinuationOptions.OnlyOnFaulted)
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable<T> ThrowExceptions<T>(
			this Task<T> task,
			Type logSource,
			IFormatLogger logger)
		{
			return task
				.ContinueWith<T>(
					failedTask =>
					{
						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in failedTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger.ThrowException(
							logSource,
							$"{failedTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");

						return default;
					},
					TaskContinuationOptions.OnlyOnFaulted)
				.ConfigureAwait(false);
		}

		#endregion
	}
}