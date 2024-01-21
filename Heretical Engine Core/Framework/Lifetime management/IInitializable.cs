namespace HereticalSolutions.LifetimeManagement
{
	public interface IInitializable
	{
		//Screw this, I am sick and tired of having a mostly unused bogus Initialize() method in every single class
		//that has its own initialization method with their own parameters. From now on if any class to wishes to call
		//Initialize from the interface, they can do it as long as the target downcasts arguments to their respective types
		//and performs an inner Initialize call
		//void Initialize();
		void Initialize(object[] args = null);
	}
}