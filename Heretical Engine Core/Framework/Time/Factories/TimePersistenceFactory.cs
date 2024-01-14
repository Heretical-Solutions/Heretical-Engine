using HereticalSolutions.Persistence.Visitors;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Time;
using HereticalSolutions.Time.Visitors;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Factories
{
    public static partial class TimeFactory
    {
        public static CompositeVisitor BuildSimpleCompositeVisitorWithTimerVisitors(
            ILoggerResolver loggerResolver = null)
        {
            IFormatLogger persistentTimerVisitorLogger =
                loggerResolver?.GetLogger<PersistentTimerVisitor>()
                ?? null;

            IFormatLogger runtimeTimerVisitorLogger =
                loggerResolver?.GetLogger<RuntimeTimerVisitor>()
                ?? null;

            #region Load visitors repository

            // Create a repository to store the visitors for loading objects
            IRepository<Type, object> loadVisitorsDatabase = RepositoriesFactory.BuildDictionaryRepository<Type, object>();

            // Add a persistent timer visitor to the load visitors repository
            loadVisitorsDatabase.Add(
                typeof(IPersistentTimer),
                new PersistentTimerVisitor(
                    loggerResolver,
                    persistentTimerVisitorLogger));

            // Add a runtime timer visitor to the load visitors repository
            loadVisitorsDatabase.Add(
                typeof(IRuntimeTimer),
                new RuntimeTimerVisitor(
                    loggerResolver,
                    runtimeTimerVisitorLogger));
            
            // Create an immutable object repository for the load visitors
            IReadOnlyObjectRepository loadVisitorsRepository = RepositoriesFactory.BuildDictionaryObjectRepository(loadVisitorsDatabase);
            
            #endregion
            
            #region Save visitors repository
            
            // Create a repository to store the visitors for saving objects
            IRepository<Type, object> saveVisitorsDatabase = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            // Add a persistent timer visitor to the save visitors repository
            saveVisitorsDatabase.Add(
                typeof(IPersistentTimer),
                new PersistentTimerVisitor(
                    loggerResolver,
                    persistentTimerVisitorLogger));
            
            // Add a runtime timer visitor to the save visitors repository
            saveVisitorsDatabase.Add(
                typeof(IRuntimeTimer),
                new RuntimeTimerVisitor(
                    loggerResolver,
                    runtimeTimerVisitorLogger));
            
            // Create an immutable object repository for the save visitors
            IReadOnlyObjectRepository saveVisitorsRepository = RepositoriesFactory.BuildDictionaryObjectRepository(saveVisitorsDatabase);
            
            #endregion

            // Build and return a composite visitor using the load and save visitors repositories
            return PersistenceFactory.BuildCompositeVisitor(
                loadVisitorsRepository,
                saveVisitorsRepository,
                loggerResolver);
        }
    }
}