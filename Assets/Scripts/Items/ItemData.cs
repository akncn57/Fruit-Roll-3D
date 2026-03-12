using System;

namespace Items
{
    [Serializable]
    public class ItemData
    {
        public ItemType Type { get; set; }
        public int Amount { get; set; }
    }
}