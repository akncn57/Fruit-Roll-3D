using System;
using Items;

namespace Map
{
    [Serializable]
    public class MapStepData
    {
        public int StepIndex;
        public StepType Type = StepType.Normal;
        public ItemData Reward;
    }
}
