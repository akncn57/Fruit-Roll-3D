using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Items;

namespace Map
{
    public class MapTile : MonoBehaviour
    {
        [SerializeField] private TextMeshPro stepIndexText;
        [SerializeField] private Transform itemSpawnPoint;
        [SerializeField] private TextMeshPro rewardAmountText;

        [Header("Item Models")]
        [SerializeField] private GameObject applePrefab;
        [SerializeField] private GameObject pearPrefab;
        [SerializeField] private GameObject strawberryPrefab;

        public void Initialize(MapStepData stepData)
        {
            // Update the real 3D text in the world
            if (stepIndexText != null)
            {
                stepIndexText.text = (stepData.StepIndex + 1).ToString();
            }

            // Spawn the reward inside the box if it exists
            if (stepData.Reward != null && stepData.Reward.Amount > 0)
            {
                SpawnRewardItem(stepData.Reward.Type);
                
                if (rewardAmountText != null) 
                {
                    rewardAmountText.text = stepData.Reward.Amount.ToString();
                }
            }
            else if (rewardAmountText != null)
            {
                // Clear the text if there is no reward or amount is 0
                rewardAmountText.text = "";
            }
        }

        private void SpawnRewardItem(ItemType type)
        {
            GameObject prefabToSpawn = null;

            // Match Enum Type with corresponding 3D Prefabs
            switch (type)
            {
                case ItemType.Apple:
                    prefabToSpawn = applePrefab;
                    break;
                case ItemType.Pear:
                    prefabToSpawn = pearPrefab;
                    break;
                case ItemType.Strawberry:
                    prefabToSpawn = strawberryPrefab;
                    break;
                case ItemType.None:
                    break;
            }

            if (prefabToSpawn != null && itemSpawnPoint != null)
            {
                // Instantiate the 3D reward inside the box
                Instantiate(prefabToSpawn, itemSpawnPoint.position, itemSpawnPoint.rotation, itemSpawnPoint);
            }
        }
    }
}
