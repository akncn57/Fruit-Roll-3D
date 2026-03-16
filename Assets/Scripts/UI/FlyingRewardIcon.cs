using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class FlyingRewardIcon : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image iconImage;

        [Header("Animation Settings")]
        [SerializeField] private float flyDuration = 0.8f;
        [SerializeField] private float arcHeight = 150f;

        public void Init(Sprite iconSprite, Vector3 startPos, ItemUIController targetController, int amountToAdd = 0)
        {
            if (iconImage != null)
            {
                iconImage.sprite = iconSprite;
            }
            
            // Start animation
            StartCoroutine(FlyRoutine(startPos, targetController, amountToAdd));
        }

        private IEnumerator FlyRoutine(Vector3 startPos, ItemUIController targetController, int amountToAdd)
        {
            var tr = transform;
            tr.position = startPos;
            tr.localScale = Vector3.zero;

            float time = 0f;
            Vector3 randomOffset = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 0f);
            
            Vector3 initialPos = startPos;

            while (time < flyDuration)
            {
                time += Time.deltaTime;
                float t = time / flyDuration;

                // Ease In Out
                float easeInOutT = t * t * (3f - 2f * t);
                
                Vector3 targetPos = targetController.GetIconPosition();

                // Lerp between start + random offset to the target
                Vector3 currentPos = Vector3.Lerp(initialPos + randomOffset * (1f - easeInOutT), targetPos, easeInOutT);
                
                // Add arc for juiciness
                currentPos.y += Mathf.Sin(t * Mathf.PI) * arcHeight;
                
                tr.position = currentPos;

                // Scale logic: InOut (0 -> 1 -> 0)
                float scale = 1f;
                // Popup quickly
                if (t < 0.2f) scale = Mathf.Lerp(0f, 1f, t / 0.2f);
                // Shrink quickly at the end
                else if (t > 0.8f) scale = Mathf.Lerp(1f, 0f, (t - 0.8f) / 0.2f);
                
                tr.localScale = Vector3.one * scale;

                yield return null;
            }
            
            if (targetController != null && amountToAdd > 0)
            {
                targetController.AddVisualAmount(amountToAdd);
            }
            
            // Clean up prefab instance
            Destroy(gameObject);
        }
    }
}
