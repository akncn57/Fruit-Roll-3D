using System;
using System.Collections.Generic;
using System.Linq;
using Items;

namespace Inventory
{
    [Serializable]
    public class InventoryData
    {
        public List<ItemData> Items = new List<ItemData>();

        public void Add(ItemData itemData)
        {
            var existingItem = Items.FirstOrDefault(item => item.Type == itemData.Type);

            if (existingItem != null)
            {
                existingItem.Amount += itemData.Amount;
            }
            else
            {
                Items.Add(new ItemData
                {
                    Type = itemData.Type, 
                    Amount = itemData.Amount
                });
            }
        }

        public int GetAmount(ItemType itemType)
        {
            var existingItem = Items.FirstOrDefault(item => item.Type == itemType);
            return existingItem != null ? existingItem.Amount : 0;
        }

        public void Clear()
        {
            Items.Clear();
        }
    }
}