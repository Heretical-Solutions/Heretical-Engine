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
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<CompositeVisitor>()
                ?? null;

            return new CompositeVisitor(
                loadVisitorsRepository,
                saveVisitorsRepository,
                logger);
        }
    }
}