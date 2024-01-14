namespace HereticalSolutions.Logging
{
	public interface ILoggerBuilder
	{
		bool AllowedByDefault { get; set; }

		void Toggle<TLogSource>(
			bool allowed);

		void Toggle(
			Type logSourceType,
			bool allowed);
	}
}