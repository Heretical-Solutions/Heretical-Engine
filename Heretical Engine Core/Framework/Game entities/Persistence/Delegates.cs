using System;
using System.Collections.Generic;

using DefaultEcs;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.GameEntities
{
    /// <summary>
    /// Delegate for visiting an entity to read its components.
    /// </summary>
    /// <param name="entity">The entity to visit.</param>
    /// <param name="typeToHash">The repository of types to hash codes.</param>
    /// <param name="componentDeltas">The list of component deltas.</param>
    public delegate void VisitorReadComponentDelegate(
        Entity entity,
        IReadOnlyRepository<Type, int> typeToHash,
        List<ECSComponentDTO> componentDTOs);

    /// <summary>
    /// Delegate for visiting an entity to write its components.
    /// </summary>
    /// <param name="entity">The entity to visit.</param>
    /// <param name="component">The component delta to write.</param>
    public delegate void VisitorWriteComponentDelegate(
        Entity entity,
        ECSComponentDTO componentDTO);
}