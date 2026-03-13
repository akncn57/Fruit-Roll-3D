using UnityEngine;
using TMPro;

namespace Dice
{
    public class DiceInputController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField firstDiceInput;
        [SerializeField] private TMP_InputField secondDiceInput;

        private void Start()
        {
            if (firstDiceInput != null)
                firstDiceInput.onValueChanged.AddListener(UpdateForcedValues);
                
            if (secondDiceInput != null)
                secondDiceInput.onValueChanged.AddListener(UpdateForcedValues);
        }

        private void UpdateForcedValues(string _)
        {
            if (DiceManager.Instance == null) return;
            
            DiceManager.Instance.ForcedDiceValues.Clear();
            
            if (firstDiceInput != null && int.TryParse(firstDiceInput.text, out int val1))
            {
                DiceManager.Instance.ForcedDiceValues.Add(val1);
            }
            
            if (secondDiceInput != null && int.TryParse(secondDiceInput.text, out int val2))
            {
                DiceManager.Instance.ForcedDiceValues.Add(val2);
            }
        }
    }
}
