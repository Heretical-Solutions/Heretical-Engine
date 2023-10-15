using System;

namespace HereticalSolutions.GameEntities
{
    [Serializable]
    public struct ECSEntityDTO
    {
        public string GUID;

        public string PrototypeID;

        public ECSComponentDTO[] ComponentDTOs;
    }
}