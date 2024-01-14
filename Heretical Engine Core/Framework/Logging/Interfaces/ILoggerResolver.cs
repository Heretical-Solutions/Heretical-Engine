namespace HereticalSolutions.Logging
{
	public interface ILoggerResolver
	{
		IFormatLogger GetLogger<TLogSource>();

		IFormatLogger GetLogger(Type logSourceType);
	}
}