using Core;
using Dice;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DiceInputUIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_InputField firstDiceInput;
        [SerializeField] private TMP_InputField secondDiceInput;
        [SerializeField] private Button rollDiceButton;

        private void Start()
        {
            if (firstDiceInput != null)
                firstDiceInput.onValueChanged.AddListener(UpdateForcedValues);
                
            if (secondDiceInput != null)
                secondDiceInput.onValueChanged.AddListener(UpdateForcedValues);
            
            if (rollDiceButton != null)
                rollDiceButton.onClick.AddListener(RollDice);
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

        private void RollDice()
        {
            if (GameManager.Instance == null) return;
            
            GameManager.Instance.OnRollButtonTapped();
        }
    }
}
