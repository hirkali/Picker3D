using System;
using System.Collections.Generic;

namespace Data.ValueObjects
{
    [Serializable]
    public struct LevelData
    {
        public List<PoolData> Pools;

        public LevelData(List<PoolData> pools)
        {
            Pools = pools;
        }
    }
}