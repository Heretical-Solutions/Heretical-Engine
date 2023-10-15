using System;

using DefaultEcs;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.GameEntities
{
    public interface IEntityManagerInternal
    {
        IRepository<string, Entity> PrototypesRepository { get; }

        IRepository<EWorld, World> WorldsRepository { get; }

        IRepository<EWorld, IWorldController> WorldControllersRepository { get; }
    }
}