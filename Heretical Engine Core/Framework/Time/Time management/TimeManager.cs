using HereticalSolutions.Repositories;

namespace HereticalSolutions.Time
{
    public class TimeManager
    {
        private readonly IRepository<string, ISynchronizable> contextsRepository;

        /// <summary>
        /// Initializes a new instance of the TimeManager class.
        /// </summary>
        /// <param name="contextsRepository">The repository to store the synchronizable contexts.</param>
        public TimeManager(IRepository<string, ISynchronizable> contextsRepository)
        {
            this.contextsRepository = contextsRepository;
        }

        /// <summary>
        /// Retrieves the synchronizable context by the specified id.
        /// </summary>
        /// <param name="id">The id of the synchronizable context.</param>
        /// <returns>The synchronizable context with the specified id if found; otherwise, null.</returns>
        public ISynchronizable GetSynchronizable(string id)
        {
            // There may not be the synchronizable by the given id therefore TryGet
            if (!contextsRepository.TryGet(id, out var synchronizable))
                return null;

            return synchronizable;
        }
        
        /// <summary>
        /// Retrieves the synchronization provider by the specified id.
        /// </summary>
        /// <param name="id">The id of the synchronization provider.</param>
        /// <returns>The synchronization provider with the specified id if found; otherwise, null.</returns>
        public ISynchronizationProvider GetProvider(string id)
        {
            // There may not be the synchronizable by the given id therefore TryGet
            if (!contextsRepository.TryGet(id, out var synchronizable))
                return null;
            
            return (ISynchronizationProvider)synchronizable;
        }
        
        /// <summary>
        /// Adds a synchronizable context to the repository with the specified id.
        /// </summary>
        /// <param name="id">The id of the synchronizable context.</param>
        /// <param name="context">The synchronizable context to add.</param>
        public void AddSynchronizable(string id, ISynchronizable context)
        {
            contextsRepository.Add(id, context);
        }

        /// <summary>
        /// Removes the synchronizable context with the specified id from the repository.
        /// </summary>
        /// <param name="id">The id of the synchronizable context to remove.</param>
        public void RemoveSynchronizable(string id)
        {
            contextsRepository.Remove(id);
        }
    }
}