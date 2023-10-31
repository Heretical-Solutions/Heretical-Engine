namespace HereticalSolutions.HereticalEngine.Messaging
{
	public class MainThreadCommand
	{
		public ECommandStatus Status
		{
			get
			{
				ECommandStatus result;

				lock (lockObject)
				{
					result = status;
				}

				return result;
			}
		}

		public Action DelegateToPerform { get; private set; }

		private ECommandStatus status = ECommandStatus.QUEUED;

		private object lockObject = new object();

		public MainThreadCommand(
			Action delegateToPerform)
		{
			status = ECommandStatus.QUEUED;

			DelegateToPerform = delegateToPerform;
		}

		public void Execute()
		{
			lock (lockObject)
			{
				status = ECommandStatus.IN_PROGRESS;
			}

			DelegateToPerform?.Invoke();

			lock (lockObject)
			{
				status = ECommandStatus.DONE;
			}
		}
	}
}