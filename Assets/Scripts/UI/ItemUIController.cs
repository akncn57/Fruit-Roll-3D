using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Items;
using Inventory;
using Utils;

namespace UI
{
    public class ItemUIController : MonoBehaviour
    {
        public static readonly System.Collections.Generic.Dictionary<ItemType, ItemUIController> ActiveControllers = new System.Collections.Generic.Dictionary<ItemType, ItemUIController>();

        [Header("Configuration")]
        [SerializeField] private ItemType itemTypeToDisplay;

        [Header("UI References")]
        [SerializeField] private Image itemIconImage;
        [SerializeField] private TextMeshProUGUI itemAmountText;

        private int _currentDisplayAmount;

        private void Start()
        {
            SetupVisuals();
            ActiveControllers[itemTypeToDisplay] = this;

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged += HandleInventoryChanged;
                SetVisualAmount(InventoryManager.Instance.GetInventory().GetAmount(itemTypeToDisplay));
            }
            else
            {
                EditorLogger.Error(nameof(ItemUIController), "InventoryManager instance is missing on Start!");
            }
        }

        private void OnDestroy()
        {
            if (ActiveControllers.TryGetValue(itemTypeToDisplay, out var controller) && controller == this)
            {
                ActiveControllers.Remove(itemTypeToDisplay);
            }

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged -= HandleInventoryChanged;
            }
        }

        private void SetupVisuals()
        {
            if (InventoryManager.Instance == null) return;

            var definition = InventoryManager.Instance.GetItemDefinition(itemTypeToDisplay);
            if (definition != null)
            {
                if (itemIconImage != null)
                {
                    itemIconImage.sprite = definition.itemIcon;
                }
            }
            else
            {
                EditorLogger.Warning(nameof(ItemUIController), $"No ItemDefinition found in InventoryManager for {itemTypeToDisplay}");
            }
        }

        private void HandleInventoryChanged(ItemType type, int newAmount)
        {
            if (type == itemTypeToDisplay)
            {
                SetVisualAmount(newAmount);
            }
        }

        private void SetVisualAmount(int amount)
        {
            _currentDisplayAmount = amount;
            UpdateAmountDisplay();
        }

        public void AddVisualAmount(int amountToAdd)
        {
            _currentDisplayAmount += amountToAdd;
            UpdateAmountDisplay();
        }

        public Vector3 GetIconPosition()
        {
            if (itemIconImage != null)
            {
                return itemIconImage.transform.position;
            }
            return transform.position;
        }

        private void UpdateAmountDisplay()
        {
            if (itemAmountText != null)
            {
                itemAmountText.text = _currentDisplayAmount.ToString();
            }
        }
    }
}
