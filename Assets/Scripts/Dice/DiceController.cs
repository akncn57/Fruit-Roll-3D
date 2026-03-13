using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Dice
{
    [RequireComponent(typeof(Rigidbody))]
    public class DiceController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody rb;
        
        [Header("Dice Configuration")]
        [SerializeField] private List<DiceFace> diceFaces;

        [Header("Effects")]
        [SerializeField] private ParticleSystem[] stopParticles;

        public bool IsRolling => _isRolling;
        public int CurrentValue => _currentValue;
        public int? ForcedValue => _forcedValue;
        
        private bool _isRolling = false;
        private int _currentValue = 0;
        private int? _forcedValue = null;

        public void Roll(Vector3 throwForce, Vector3 throwTorque, int? forcedValue = null)
        {
            _isRolling = true;
            _currentValue = 0;
            _forcedValue = forcedValue;
            
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

            if (_forcedValue.HasValue)
            {
                ForceFaceValue(_forcedValue.Value);
            }

            CalculateValue();

            if (stopParticles != null)
            {
                foreach (var ps in stopParticles)
                {
                    if (ps != null) ps.Play();
                }
            }
            
            _isRolling = false;
        }

        private void ForceFaceValue(int targetValue)
        {
            foreach (var face in diceFaces)
            {
                if (face.faceValue == targetValue)
                {
                    Quaternion deltaRot = Quaternion.FromToRotation(face.transform.up, Vector3.up);
                    transform.rotation = deltaRot * transform.rotation;
                    return;
                }
            }
        }

        private void CalculateValue()
        {
            foreach (var face in diceFaces)
            {
                if (face.IsFaceUp())
                {
                    _currentValue = face.faceValue;
                    EditorLogger.Log(nameof(DiceController), $"A dice landed on value: {_currentValue}");
                    return;
                }
            }
            
            EditorLogger.Warning(nameof(DiceController), "Dice stopped, but no clear face is pointing up!");
            _currentValue = 0;
        }
    }
}
