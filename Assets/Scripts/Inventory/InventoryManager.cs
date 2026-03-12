using Items;
using UnityEngine;

namespace Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }
        
        public InventoryData GetInventory() => _currentData;
        
        private InventoryService _inventoryService;
        private InventoryData _currentData;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Init();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Init()
        {
            _inventoryService = new InventoryService();
            _currentData = _inventoryService.LoadInventory();
        }
        
        public void AddItem(ItemData item)
        {
            _currentData.Add(item);
            _inventoryService.SaveInventory(_currentData);
            Debug.Log($"{item.Amount}x {item.Type} added to inventory.");
        }
    }
}