using System.Collections;
using Items;
using Inventory;
using UnityEngine;
using Utils;

namespace UI
{
    public class InOutManager : MonoBehaviour
    {
        public static InOutManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private Transform inOutsParent;
        
        [Header("Animation Settings")]
        [SerializeField] private FlyingRewardIcon rewardPrefab;
        [SerializeField] private float spawnDelay = 0.08f;
        [SerializeField] private int maxIcons = 10;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AnimateReward(ItemType type, int totalRewardAmount, Vector3 worldSourcePosition)
        {
            StartCoroutine(AnimateRewardRoutine(type, totalRewardAmount, worldSourcePosition));
        }

        private IEnumerator AnimateRewardRoutine(ItemType type, int totalAmount, Vector3 worldSourcePosition)
        {
            if (!ItemUIController.ActiveControllers.TryGetValue(type, out var controller))
            {
                EditorLogger.Warning(nameof(InOutManager), $"No UI controller found for {type}. Cannot animate.");
                yield break;
            }

            var definition = InventoryManager.Instance.GetItemDefinition(type);
            
            if (definition == null || definition.itemIcon == null)
            {
                EditorLogger.Warning(nameof(InOutManager), $"No item definition or icon for {type}.");
                controller.AddVisualAmount(totalAmount);
                yield break;
            }
            
            var numIcons = maxIcons;

            if (totalAmount < maxIcons && totalAmount > 0)
            {
                numIcons = totalAmount;
                
            }
            
            var startPos = Vector3.zero;
            
            if (Camera.main != null)
            {
                startPos = Camera.main.WorldToScreenPoint(worldSourcePosition);
                startPos.z = 0; // Ensure it is on the UI plane layer
            }
            else
            {
                startPos = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            }
            
            if (rewardPrefab == null)
            {
                EditorLogger.Error(nameof(InOutManager), "Reward Prefab is not assigned! Cannot animate.");
                controller.AddVisualAmount(totalAmount);
                yield break;
            }

            int baseAmountPerIcon = totalAmount / numIcons;
            int remainder = totalAmount % numIcons;

            for (var i = 0; i < numIcons; i++)
            {
                int amountForThisIcon = baseAmountPerIcon + (i < remainder ? 1 : 0);
                var iconInstance = Instantiate(rewardPrefab, inOutsParent, false);
                iconInstance.Init(definition.itemIcon, startPos, controller, amountForThisIcon);
                
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }
}
