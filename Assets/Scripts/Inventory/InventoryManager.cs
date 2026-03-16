using Items;
using UnityEngine;
using Utils;

namespace Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }
        
        [Header("Item Definitions")]
        [SerializeField] private ItemDefinition[] itemDefinitions;
        
        public event System.Action<ItemType, int> OnInventoryChanged;
        
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
        
        public void AddItem(ItemData item, bool silent = false)
        {
            _currentData.Add(item);
            _inventoryService.SaveInventory(_currentData);
            EditorLogger.Log(nameof(InventoryManager), $"{item.Amount}x {item.Type} added to inventory.");
            
            if (!silent)
            {
                OnInventoryChanged?.Invoke(item.Type, _currentData.GetAmount(item.Type));
            }
        }

        public void ClearInventory()
        {
            // Trigger 0 amount UI updates for existing items
            foreach (var item in _currentData.Items)
            {
                OnInventoryChanged?.Invoke(item.Type, 0);
            }
            
            _currentData.Clear();
            _inventoryService.SaveInventory(_currentData);
            EditorLogger.Log(nameof(InventoryManager), "Inventory cleared due to landing on ResetData tile.");
        }

        public ItemDefinition GetItemDefinition(ItemType type)
        {
            if (itemDefinitions == null) return null;
            
            foreach (var def in itemDefinitions)
            {
                if (def != null && def.itemType == type) return def;
            }
            return null;
        }
    }
}