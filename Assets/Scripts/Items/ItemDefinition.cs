using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "NewItemDef", menuName = "FruitRoll/Item Definition")]
    public class ItemDefinition : ScriptableObject
    {
        public ItemType itemType;
        public Sprite itemIcon;
        public GameObject itemPrefab;
    }
}
