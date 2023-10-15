using System;

namespace HereticalSolutions.GameEntities
{
    [Serializable]
    public struct ECSEntityCreatedDeltaDTO
    {
        public string EntityGUID;
        
        public string PrototypeID;
    }
}