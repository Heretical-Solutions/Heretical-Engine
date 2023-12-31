using HereticalSolutions.Persistence.Visitors;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Factories
{
    public static partial class PersistenceFactory
    {
        public static CompositeVisitor BuildCompositeVisitor(
            IReadOnlyObjectRepository loadVisitorsRepository,
            IReadOnlyObjectRepository saveVisitorsRepository,
            IFormatLogger logger)
        {
            return new CompositeVisitor(
                loadVisitorsRepository,
                saveVisitorsRepository,
                logger);
        }
    }
}