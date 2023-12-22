using System;

namespace HereticalSolutions.GameEntities
{
    [Serializable]
    public struct ECSComponentDeltaDTO
    {
        public string EntityGUID;
        
        public ECSComponentDTO ComponentDTO;
    }
}