using System;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Interface for managing game entities.
    /// </summary>
    public interface IEntityManager
        : IWorldRepository, //TODO: EXTRACT
          IPrototypeRepository,
          IEntityRepository,
          IContainsEventEntityBuilder,
          IContainsMultiWorldSetter
    {
        #region Spawn entity

        /// <summary>
        /// Spawns a new entity based on the specified prototype ID and authoring type.
        /// </summary>
        /// <param name="prototypeID">The ID of the entity's prototype.</param>
        /// <param name="authoring">The authoring type of the entity.</param>
        /// <returns>The GUID of the spawned entity.</returns>
        Guid SpawnEntity(
            string prototypeID,
            EEntityAuthoring authoring = EEntityAuthoring.OFFLINE);

        /// <summary>
        /// Spawns a new entity from the server with the specified GUID and prototype ID.
        /// </summary>
        /// <param name="guid">The GUID of the entity.</param>
        /// <param name="prototypeID">The ID of the entity's prototype.</param>
        void SpawnEntityFromServer(
            Guid guid,
            string prototypeID);

        #endregion

        #region Resolve entity

        /// <summary>
        /// Resolves an entity based on the specified target, prototype ID, and authoring type.
        /// </summary>
        /// <param name="target">The target object for resolving the entity.</param>
        /// <param name="prototypeID">The ID of the entity's prototype.</param>
        /// <param name="authoring">The authoring type of the entity.</param>
        void ResolveEntity(
            object target,
            string prototypeID,
            EEntityAuthoring authoring = EEntityAuthoring.OFFLINE);

        /// <summary>
        /// Resolves an entity based on the specified GUID, target, prototype ID, and authoring type.
        /// </summary>
        /// <param name="guid">The GUID of the entity.</param>
        /// <param name="target">The target object for resolving the entity.</param>
        /// <param name="prototypeID">The ID of the entity's prototype.</param>
        /// <param name="authoring">The authoring type of the entity.</param>
        void ResolveEntity(
            Guid guid,
            object target,
            string prototypeID,
            EEntityAuthoring authoring = EEntityAuthoring.OFFLINE);

        #endregion

        #region Despawn entity

        /// <summary>
        /// Despawns the entity with the specified GUID.
        /// </summary>
        /// <param name="guid">The GUID of the entity to despawn.</param>
        void DespawnEntity(Guid guid);

        #endregion
    }
}