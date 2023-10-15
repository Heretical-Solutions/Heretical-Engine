using HereticalSolutions.Persistence.Visitors;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Persistence.Factories
{
    /// <summary>
    /// Provides a static method to build instances of the CompositeVisitor class.
    /// </summary>
    public static partial class PersistenceFactory
    {
        /// <summary>
        /// Builds an instance of the CompositeVisitor class.
        /// </summary>
        /// <param name="loadVisitorsRepository">An instance of the IReadOnlyObjectRepository class.</param>
        /// <param name="saveVisitorsRepository">An instance of the IReadOnlyObjectRepository class.</param>
        /// <returns>An instance of the CompositeVisitor class.</returns>
        public static CompositeVisitor BuildCompositeVisitor(IReadOnlyObjectRepository loadVisitorsRepository, IReadOnlyObjectRepository saveVisitorsRepository)
        {
            return new CompositeVisitor(loadVisitorsRepository, saveVisitorsRepository);
        }
    }
}