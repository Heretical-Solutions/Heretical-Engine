using System;
using System.Collections.Generic;
using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Represents an interface for an entity repository.
    /// </summary>
    public interface IEntityRepository
    {
        /// <summary>
        /// Determines whether the repository contains an entity with the specified ID.
        /// </summary>
        /// <param name="guid">The ID to check.</param>
        /// <returns>True if the entity exists, otherwise false.</returns>
        bool HasEntity(Guid guid);
        
        /// <summary>
        /// Gets the entity with the specified ID.
        /// </summary>
        /// <param name="guid">The ID of the entity to get.</param>
        /// <returns>The entity with the specified ID.</returns>
        Entity GetEntity(Guid guid);

        /// <summary>
        /// Gets an array of all entities in the repository along with their respective prototype IDs.
        /// </summary>
        GuidPrototypeIDPair[] AllEntities { get; }
        
        /// <summary>
        /// Gets an IEnumerable of all entities GUIDs.
        /// </summary>
        public IEnumerable<Guid> GetAllGUIDs { get; }
    }
}