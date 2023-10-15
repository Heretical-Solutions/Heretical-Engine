using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents a repository for accessing worlds.
    /// </summary>
    public interface IWorldRepository
    {
        /// <summary>
        /// Gets the world with the specified ID.
        /// </summary>
        /// <param name="worldID">The ID of the world.</param>
        /// <returns>The world with the specified ID.</returns>
        World GetWorld(EWorld worldID);

        IWorldController GetWorldController(EWorld worldID);
    }
}