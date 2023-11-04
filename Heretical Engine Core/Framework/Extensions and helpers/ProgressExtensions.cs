namespace HereticalSolutions
{
	public static class ProgressExtensions
	{
		public static IProgress<float> CreateLocalProgress(
			this IProgress<float> progress)
		{
			IProgress<float> localProgress = null;

			if (progress != null)
			{
				var localProgressInstance = new Progress<float>();

				localProgressInstance.ProgressChanged += (sender, value) =>
				{
					//Console.WriteLine($"PROGRESS: {String.Format("{0:0.00}", value)} REPORTING: {String.Format("{0:0.00}", value)}");

					progress.Report(value);
				};

				localProgress = localProgressInstance;
			}

			return localProgress;
		}

		public static IProgress<float> CreateLocalProgress(
			this IProgress<float> progress,
			EventHandler<float> handler)
		{
			IProgress<float> localProgress = null;

			if (progress != null)
			{
				var localProgressInstance = new Progress<float>();

				localProgressInstance.ProgressChanged += handler;

				localProgress = localProgressInstance;
			}

			return localProgress;
		}

		public static IProgress<float> CreateLocalProgress(
			this IProgress<float> progress,
			Func<float, float> totalProgressCalculationDelegate)
		{
			IProgress<float> localProgress = null;

			if (progress != null)
			{
				var localProgressInstance = new Progress<float>();

				localProgressInstance.ProgressChanged += (sender, value) =>
				{
					/*
					Console.WriteLine($"PROGRESS: {String.Format("{0:0.00}", value)} REPORTING: {String.Format("{0:0.00}", totalProgressCalculationDelegate.Invoke(value))}");
					*/

					progress.Report(totalProgressCalculationDelegate.Invoke(value));
				};

				localProgress = localProgressInstance;
			}

			return localProgress;
		}

		public static IProgress<float> CreateLocalProgress(
			this IProgress<float> progress,
			float totalProgressStart,
			float totalProgressFinish)
		{
			IProgress<float> localProgress = null;

			if (progress != null)
			{
				float scale = totalProgressFinish - totalProgressStart;

				var localProgressInstance = new Progress<float>();

				localProgressInstance.ProgressChanged += (sender, value) =>
				{
					/*
					Console.WriteLine($"PROGRESS: {String.Format("{0:0.00}", value)} totalProgressStart: {String.Format("{0:0.00}", totalProgressStart)} totalProgressFinish: {String.Format("{0:0.00}", totalProgressFinish)} scale: {String.Format("{0:0.00}", scale)} REPORTING: {String.Format("{0:0.00}", (scale * value + totalProgressStart))}");
					*/

					progress.Report(scale * value + totalProgressStart);
				};

				localProgress = localProgressInstance;
			}

			return localProgress;
		}

		public static IProgress<float> CreateLocalProgress(
			this IProgress<float> progress,
			float totalProgressStart,
			float totalProgressFinish,
			Func<float, float> localProgressCalculationDelegate)
		{
			IProgress<float> localProgress = null;

			if (progress != null)
			{
				float scale = totalProgressFinish - totalProgressStart;

				var localProgressInstance = new Progress<float>();

				localProgressInstance.ProgressChanged += (sender, value) =>
				{
					/*
					Console.WriteLine($"PROGRESS: {String.Format("{0:0.00}", value)} totalProgressStart: {String.Format("{0:0.00}", totalProgressStart)} totalProgressFinish: {String.Format("{0:0.00}", totalProgressFinish)} scale: {String.Format("{0:0.00}", scale)} REPORTING: {String.Format("{0:0.00}", (scale * localProgressCalculationDelegate.Invoke(value) + totalProgressStart))}");
					*/

					progress.Report(scale * localProgressCalculationDelegate.Invoke(value) + totalProgressStart);
				};

				localProgress = localProgressInstance;
			}

			return localProgress;
		}

		public static IProgress<float> CreateLocalProgress(
			this IProgress<float> progress,
			float totalProgressStart,
			float totalProgressFinish,
			int index,
			int count)
		{
			IProgress<float> localProgress = null;

			if (progress != null)
			{
				float scale = totalProgressFinish - totalProgressStart;

				var localProgressInstance = new Progress<float>();

				localProgressInstance.ProgressChanged += (sender, value) =>
				{
					/*
					Console.WriteLine($"PROGRESS: {String.Format("{0:0.00}", value)} totalProgressStart: {String.Format("{0:0.00}", totalProgressStart)} totalProgressFinish: {String.Format("{0:0.00}", totalProgressFinish)} index: {String.Format("{0:0.00}", index)} count: {String.Format("{0:0.00}", count)} scale: {String.Format("{0:0.00}", scale)} REPORTING: {String.Format("{0:0.00}", (scale * ((value + (float)index) / count) + totalProgressStart))}");
					*/

					progress.Report(scale * ((value + (float)index) / count) + totalProgressStart);
				};

				localProgress = localProgressInstance;
			}

			return localProgress;
		}

		public static IProgress<float> CreateLocalProgress(
			this IProgress<float> progress,
			float totalProgressStart,
			float totalProgressFinish,
			List<float> localProgressValues,
			int count)
		{
			IProgress<float> localProgress = null;

			if (progress != null)
			{
				float scale = totalProgressFinish - totalProgressStart;


				int currentProgressIndex = localProgressValues.Count;

				localProgressValues.Add(0f);


				var localProgressInstance = new Progress<float>();

				localProgressInstance.ProgressChanged += (sender, value) =>
				{
					localProgressValues[currentProgressIndex] = value;


					float totalProgress = 0f;

					foreach (var assetImportProgress in localProgressValues)
					{
						totalProgress += assetImportProgress;
					}

					/*
					Console.WriteLine($"PROGRESS: {String.Format("{0:0.00}", value)} totalProgressStart: {String.Format("{0:0.00}", totalProgressStart)} totalProgressFinish: {String.Format("{0:0.00}", totalProgressFinish)} currentProgressIndex: {String.Format("{0:0.00}", currentProgressIndex)} totalProgress: {String.Format("{0:0.00}", totalProgress)} count: {String.Format("{0:0.00}", count)} scale: {String.Format("{0:0.00}", scale)} REPORTING: {String.Format("{0:0.00}", (scale * (totalProgress / (float)count) + totalProgressStart))}");
					*/

					progress.Report(scale * (totalProgress / (float)count) + totalProgressStart);
				};

				localProgress = localProgressInstance;
			}
			
			return localProgress;
		}
	}
}