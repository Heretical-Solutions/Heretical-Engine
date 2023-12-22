namespace HereticalSolutions.GameEntities
{
    public interface IEntityManager<TWorld, TEntity>
        : IReadOnlyEntityRepository<TEntity>
    {
        #region Spawn entity

        Guid SpawnEntity(
            string prototypeID,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT);

        bool SpawnEntity(
            Guid guid,
            string prototypeID,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT);

        TEntity SpawnWorldLocalEntity(
            string prototypeID,
            string worldID);

        TEntity SpawnWorldLocalEntity(
            string prototypeID,
            TWorld world);

        #endregion

        #region Resolve entity

        Guid ResolveEntity(
            object source,
            string prototypeID,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT);

        bool ResolveEntity(
            Guid guid,
            object source,
            string prototypeID,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT);

        TEntity ResolveWorldLocalEntity(
            string prototypeID,
            object source,
            string worldID);

        TEntity ResolveWorldLocalEntity(
            string prototypeID,
            object source,
            TWorld world);

        #endregion

        #region Despawn entity

        void DespawnEntity(Guid guid);

        void DespawnWorldLocalEntity(TEntity entity);

        #endregion
    }
}