using UnityEngine;
using TMPro;

namespace UI
{
    public class GameInfoUIController : MonoBehaviour
    {
        public static GameInfoUIController Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI infoText;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void ShowMessage(string message)
        {
            if (infoText != null)
            {
                infoText.text = message;
            }
            else
            {
                Debug.LogWarning($"[GameInfoUIController] Trying to show message '{message}' but infoText is not assigned!");
            }
        }
    }
}
