using System;
using System.Collections.Generic;

namespace Map
{
    [Serializable]
    public class MapData
    {
        public string MapName;
        public int TotalSteps;
        public List<MapStepData> Steps = new List<MapStepData>();
    }
}
