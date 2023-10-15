using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Interface for a prototype repository that stores and manages entities as prototypes.
    /// </summary>
    public interface IPrototypeRepository
    {
        /// <summary>
        /// Checks if the specified prototype ID exists in the repository.
        /// </summary>
        /// <param name="prototypeID">The ID of the prototype to check.</param>
        /// <returns><c>true</c> if the prototype exists; otherwise, <c>false</c>.</returns>
        bool HasPrototype(string prototypeID);

        /// <summary>
        /// Retrieves the entity prototype with the specified ID from the repository.
        /// </summary>
        /// <param name="prototypeID">The ID of the prototype to retrieve.</param>
        /// <returns>The entity prototype with the specified ID.</returns>
        Entity GetPrototype(string prototypeID);

        /// <summary>
        /// Adds a new entity as a prototype to the repository.
        /// </summary>
        /// <param name="prototypeID">The ID to assign to the new prototype.</param>
        /// <param name="prototypeEntity">The entity to be added as a prototype.</param>
        void AddPrototype(
            string prototypeID,
            Entity prototypeEntity);

        /// <summary>
        /// Removes the prototype with the specified ID from the repository.
        /// </summary>
        /// <param name="prototypeID">The ID of the prototype to remove.</param>
        void RemovePrototype(string prototypeID);
    }
}