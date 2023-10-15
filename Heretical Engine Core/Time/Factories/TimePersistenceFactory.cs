using System;

using HereticalSolutions.Persistence.Visitors;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Time;
using HereticalSolutions.Time.Visitors;

namespace HereticalSolutions.Persistence.Factories
{
    /// <summary>
    /// Represents a factory for creating time-related composite visitors.
    /// </summary>
    public static partial class TimeFactory
    {
        /// <summary>
        /// Builds a composite visitor with timer visitors for loading and saving objects.
        /// </summary>
        /// <returns>A composite visitor with timer visitors.</returns>
        public static CompositeVisitor BuildSimpleCompositeVisitorWithTimerVisitors()
        {
            #region Load visitors repository
            
            // Create a repository to store the visitors for loading objects
            IRepository<Type, object> loadVisitorsDatabase = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            // Add a persistent timer visitor to the load visitors repository
            loadVisitorsDatabase.Add(typeof(IPersistentTimer), new PersistentTimerVisitor());
            
            // Add a runtime timer visitor to the load visitors repository
            loadVisitorsDatabase.Add(typeof(IRuntimeTimer), new RuntimeTimerVisitor());
            
            // Create an immutable object repository for the load visitors
            IReadOnlyObjectRepository loadVisitorsRepository = RepositoriesFactory.BuildDictionaryObjectRepository(loadVisitorsDatabase);
            
            #endregion
            
            #region Save visitors repository
            
            // Create a repository to store the visitors for saving objects
            IRepository<Type, object> saveVisitorsDatabase = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            // Add a persistent timer visitor to the save visitors repository
            saveVisitorsDatabase.Add(typeof(IPersistentTimer), new PersistentTimerVisitor());
            
            // Add a runtime timer visitor to the save visitors repository
            saveVisitorsDatabase.Add(typeof(IRuntimeTimer), new RuntimeTimerVisitor());
            
            // Create an immutable object repository for the save visitors
            IReadOnlyObjectRepository saveVisitorsRepository = RepositoriesFactory.BuildDictionaryObjectRepository(saveVisitorsDatabase);
            
            #endregion

            // Build and return a composite visitor using the load and save visitors repositories
            return PersistenceFactory.BuildCompositeVisitor(
                loadVisitorsRepository,
                saveVisitorsRepository);
        }
    }
}