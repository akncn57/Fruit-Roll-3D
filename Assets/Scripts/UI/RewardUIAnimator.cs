using System.Collections;
using Items;
using Inventory;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class RewardUIAnimator : MonoBehaviour
    {
        public static RewardUIAnimator Instance { get; private set; }

        [Header("Animation Settings")]
        [SerializeField] private FlyingRewardIcon rewardPrefab;
        [SerializeField] private float spawnDelay = 0.08f;
        [SerializeField] private int maxIcons = 10;

        private Canvas _parentCanvas;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                _parentCanvas = GetComponentInParent<Canvas>();
                if (_parentCanvas == null)
                {
                    var canvasObj = new GameObject("RewardAnimatorCanvas");
                    _parentCanvas = canvasObj.AddComponent<Canvas>();
                    _parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    _parentCanvas.sortingOrder = 99; // Render on top of everything
                    canvasObj.AddComponent<CanvasScaler>();
                    DontDestroyOnLoad(canvasObj);
                    transform.SetParent(canvasObj.transform);
                }
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
                EditorLogger.Warning(nameof(RewardUIAnimator), $"No UI controller found for {type}. Cannot animate.");
                yield break;
            }

            var definition = InventoryManager.Instance.GetItemDefinition(type);
            if (definition == null || definition.itemIcon == null)
            {
                EditorLogger.Warning(nameof(RewardUIAnimator), $"No item definition or icon for {type}.");
                controller.AddVisualAmount(totalAmount);
                yield break;
            }

            int amountPerIcon = totalAmount / maxIcons;
            int remainder = totalAmount % maxIcons;
            int numIcons = maxIcons;

            if (totalAmount < maxIcons && totalAmount > 0)
            {
                numIcons = totalAmount;
                amountPerIcon = 1;
                remainder = 0;
            }
            
            Vector3 startPos = Vector3.zero;
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
                EditorLogger.Error(nameof(RewardUIAnimator), "Reward Prefab is not assigned! Cannot animate.");
                controller.AddVisualAmount(totalAmount);
                yield break;
            }

            for (int i = 0; i < numIcons; i++)
            {
                var iconInstance = Instantiate(rewardPrefab, _parentCanvas.transform, false);
                iconInstance.Init(definition.itemIcon, startPos, controller);
                
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }
}
