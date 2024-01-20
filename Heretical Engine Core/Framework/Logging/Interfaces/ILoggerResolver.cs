namespace HereticalSolutions.Logging
{
	public interface ILoggerResolver
	{
		ILogger GetLogger<TLogSource>();

		ILogger GetLogger(Type logSourceType);
	}
}