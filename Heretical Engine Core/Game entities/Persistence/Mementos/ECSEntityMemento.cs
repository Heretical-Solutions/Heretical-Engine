using HereticalSolutions.Repositories;

namespace HereticalSolutions.GameEntities
{
    public class ECSEntityMemento
    {
        public string GUID { get; private set; }

        public string PrototypeID { get; private set; }

        public IRepository<int, ECSComponentDTO> ComponentDTOs { get; private set; }

        public ECSEntityMemento(
            string guid,
            string prototypeID,
            IRepository<int, ECSComponentDTO> componentDTOs)
        {
            GUID = guid;

            PrototypeID = prototypeID;

            ComponentDTOs = componentDTOs;
        }
    }
}