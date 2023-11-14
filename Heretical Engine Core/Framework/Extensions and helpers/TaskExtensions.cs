using System.Runtime.CompilerServices;

using System.Text;

using HereticalSolutions.Logging;

namespace HereticalSolutions
{
	public static class TaskExtensions
	{
		//For some reason the previous version I used (that utilized ContinueWith(..., TaskContinuationOptions.OnlyOnFaulted) method chain) would throw a TaskCanceledException in random places in the code
		//As stated over here, it was because TaskContinuationOptions.OnlyOnFaulted is not valid for multi-task continuations
		//https://stackoverflow.com/questions/28633871/taskcanceledexception-with-continuewith
		//So I've changed it to the option provided here:
		//https://stackoverflow.com/a/58469206

		#region Task

		public static ConfiguredTaskAwaitable LogExceptions(this Task task)
		{
			return task
				.ContinueWith(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						Console.WriteLine($"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");
					})
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable ThrowExceptions(this Task task)
		{
			return task
				.ContinueWith(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						throw new Exception($"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");
					})
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable LogExceptions<TSource>(
			this Task task,
			IFormatLogger logger)
		{
			return task
				.ContinueWith(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger?.LogError<TSource>(
							$"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");
					})
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable ThrowExceptions<TSource>(
			this Task task,
			IFormatLogger logger)
		{
			return task
				.ContinueWith(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger?.ThrowException<TSource>(
							$"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");
					})
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable LogExceptions(
			this Task task,
			Type logSource,
			IFormatLogger logger)
		{
			return task
				.ContinueWith(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger?.LogError(
							logSource,
							$"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");
					})
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable ThrowExceptions(
			this Task task,
			Type logSource,
			IFormatLogger logger)
		{
			return task
				.ContinueWith(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger?.ThrowException(
							logSource,
							$"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");
					})
				.ConfigureAwait(false);
		}

		#endregion

		#region Task<T>

		public static ConfiguredTaskAwaitable<T> LogExceptions<T>(this Task<T> task)
		{
			return task
				.ContinueWith<T>(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return targetTask.Result;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						Console.WriteLine($"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");

						return default;
					})
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable<T> ThrowExceptions<T>(this Task<T> task)
		{
			return task
				.ContinueWith<T>(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return targetTask.Result;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						throw new Exception($"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");
					})
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable<T> LogExceptions<T, TSource>(
			this Task<T> task,
			IFormatLogger logger)
		{
			return task
				.ContinueWith<T>(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return targetTask.Result;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger?.LogError<TSource>(
							$"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");

						return default;
					})
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable<T> ThrowExceptions<T, TSource>(
			this Task<T> task,
			IFormatLogger logger)
		{
			return task
				.ContinueWith<T>(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return targetTask.Result;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger?.ThrowException<TSource>(
							$"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");

						return default;
					})
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable<T> LogExceptions<T>(
			this Task<T> task,
			Type logSource,
			IFormatLogger logger)
		{
			return task
				.ContinueWith<T>(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return targetTask.Result;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger?.LogError(
							logSource,
							$"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");

						return default;
					})
				.ConfigureAwait(false);
		}

		public static ConfiguredTaskAwaitable<T> ThrowExceptions<T>(
			this Task<T> task,
			Type logSource,
			IFormatLogger logger)
		{
			return task
				.ContinueWith<T>(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return targetTask.Result;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger?.ThrowException(
							logSource,
							$"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");

						return default;
					})
				.ConfigureAwait(false);
		}

		#endregion
	}
}