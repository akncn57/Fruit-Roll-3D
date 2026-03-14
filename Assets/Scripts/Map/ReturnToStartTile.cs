using UnityEngine;
using TMPro;

namespace Map
{
    public class ReturnToStartTile : MonoBehaviour, IMapTile
    {
        [SerializeField] private TextMeshPro stepIndexText;

        public void Initialize(MapStepData stepData)
        {
            if (stepIndexText != null)
            {
                stepIndexText.text = (stepData.StepIndex + 1).ToString();
            }
        }
    }
}
