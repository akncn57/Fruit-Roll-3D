using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dice
{
    [RequireComponent(typeof(Rigidbody))]
    public class DiceController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody rb;
        
        [Header("Dice Configuration")]
        [SerializeField] private List<DiceFace> diceFaces;

        public bool IsRolling => _isRolling;
        public int CurrentValue => _currentValue;
        
        private bool _isRolling = false;
        private int _currentValue = 0;

        public void Roll(Vector3 throwForce, Vector3 throwTorque)
        {
            _isRolling = true;
            _currentValue = 0;
            
            rb.isKinematic = false;
            rb.AddForce(throwForce, ForceMode.Impulse);
            rb.AddTorque(throwTorque, ForceMode.Impulse);
            
            StartCoroutine(CheckStopRoutine());
        }

        private IEnumerator CheckStopRoutine()
        {
            yield return new WaitForSeconds(0.5f);
            
            while (rb.linearVelocity.sqrMagnitude > 0.01f || rb.angularVelocity.sqrMagnitude > 0.01f)
            {
                yield return new WaitForEndOfFrame();
            }

            CalculateValue();
            
            _isRolling = false;
        }

        private void CalculateValue()
        {
            foreach (var face in diceFaces)
            {
                if (face.IsFaceUp())
                {
                    _currentValue = face.faceValue;
                    Debug.Log($"[DiceController] A dice landed on value: {_currentValue}");
                    return;
                }
            }
            
            Debug.LogWarning("[DiceController] Dice stopped, but no clear face is pointing up!");
            _currentValue = 0;
        }
    }
}
