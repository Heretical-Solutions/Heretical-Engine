namespace HereticalSolutions.Logging
{
	public interface ILoggerWrapper
	{
		ILogger InnerLogger { get; }
	}
}