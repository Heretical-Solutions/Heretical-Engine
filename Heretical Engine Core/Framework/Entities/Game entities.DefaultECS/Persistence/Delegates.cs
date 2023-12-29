using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
	public delegate void EntityFactoryWriteComponentDelegate(
		Entity entity,
		object componentValue);

	public delegate void EntityFactoryAddComponentDelegate(
		Entity entity,
		object component);
}