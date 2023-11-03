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

					progress.Report(scale * (totalProgress / (float)count) + totalProgressStart);
				};

				localProgress = localProgressInstance;
			}
			
			return localProgress;
		}
	}
}