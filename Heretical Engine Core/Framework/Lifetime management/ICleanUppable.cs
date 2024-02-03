namespace HereticalSolutions.LifetimeManagement
{
	//TODO: ensure that all classes that have (or should have) a Cleanup method (like IRepository) inherit from this interface
	//
	//Also note that users of IRepository and other Cleanuppable classes should be Cleanuppable as well
	//
	//Also to mention, this is NOT the IDisposable. The cleanup may leave the object in a pristine YET properly self-initialized
	//state ready to be used while IDisposable is meant like "in the garbage you go"
	//
	//Also note that Cleanup is somewhat close to Clear but still there is a difference: Clear erases the data from some repo/database
	//while Cleanup is a more kind of thorough process of removing dependencies and nulling the references so make sure its either
	//one of the two or the latter is used in the former
	public interface ICleanUppable
	{
		void Cleanup();
	}
}