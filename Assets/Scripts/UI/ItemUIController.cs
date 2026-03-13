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
        [Header("Configuration")]
        [SerializeField] private ItemType itemTypeToDisplay;

        [Header("UI References")]
        [SerializeField] private Image itemIconImage;
        [SerializeField] private TextMeshProUGUI itemAmountText;

        private void Start()
        {
            SetupVisuals();

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged += HandleInventoryChanged;
                UpdateAmountDisplay(InventoryManager.Instance.GetInventory().GetAmount(itemTypeToDisplay));
            }
            else
            {
                EditorLogger.Error(nameof(ItemUIController), "InventoryManager instance is missing on Start!");
            }
        }

        private void OnDestroy()
        {
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
                UpdateAmountDisplay(newAmount);
            }
        }

        private void UpdateAmountDisplay(int amount)
        {
            if (itemAmountText != null)
            {
                itemAmountText.text = amount.ToString();
            }
        }
    }
}
