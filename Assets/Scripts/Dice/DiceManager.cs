using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dice
{
    public class DiceManager : MonoBehaviour
    {
        public static DiceManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private GameObject dicePrefab;
        [SerializeField] private Transform diceSpawnPoint; 
        [SerializeField] private int diceCountToRoll = 2;
        
        [Header("Physics Tweaks")]
        [SerializeField] private float throwForceMin = 5f;
        [SerializeField] private float throwForceMax = 10f;
        [SerializeField] private float torqueStrength = 20f;

        private readonly List<DiceController> _activeDice = new List<DiceController>();
        
        [Header("Fail Safe")]
        [SerializeField] private float fallThresholdY = -5f;
        
        public event Action<int> OnAllDiceStopped;

        public List<int> ForcedDiceValues { get; set; } = new List<int>();

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
        
        public void RollDice()
        {
            ClearDice();

            for (var i = 0; i < diceCountToRoll; i++)
            {
                var spawnOffset = Random.insideUnitSphere * 0.2f;
                var diceObj = Instantiate(dicePrefab, diceSpawnPoint.position + spawnOffset, Random.rotation, transform);
                var diceScript = diceObj.GetComponent<DiceController>();
                
                _activeDice.Add(diceScript);
                
                var baseDirection = (diceSpawnPoint.forward + Vector3.up * 0.8f).normalized;
                var randomDirection = baseDirection + (Random.insideUnitSphere * 0.2f);
                var force = randomDirection * Random.Range(throwForceMin, throwForceMax);
                
                var torque = new Vector3(
                    Random.Range(-torqueStrength, torqueStrength),
                    Random.Range(-torqueStrength, torqueStrength),
                    Random.Range(-torqueStrength, torqueStrength)
                );

                int? forceVal = null;
                if (i < ForcedDiceValues.Count && ForcedDiceValues[i] > 0)
                {
                    forceVal = ForcedDiceValues[i];
                }

                diceScript.Roll(force, torque, forceVal);
            }

            StartCoroutine(WatchAllDiceRoutine());
        }

        private IEnumerator WatchAllDiceRoutine()
        {
            var allStopped = false;

            while (!allStopped)
            {
                allStopped = true;
                
                foreach (var dice in _activeDice)
                {
                    if (dice.transform.position.y < fallThresholdY)
                    {
                        Debug.LogWarning("[DiceManager] A dice fell off the board! Respawning it...");
                        
                        ResetDice(dice);
                    }

                    if (dice.IsRolling)
                    {
                        allStopped = false;
                        break;
                    }
                }

                yield return new WaitForEndOfFrame();
            }

            var finalTotal = 0;
            
            foreach (var dice in _activeDice)
            {
                finalTotal += dice.CurrentValue;
            }

            Debug.Log($"[DiceManager] All {_activeDice.Count} dice stopped! TOTAL VALUE: {finalTotal}");
            
            OnAllDiceStopped?.Invoke(finalTotal);
        }

        public void ClearDice()
        {
            foreach (var dice in _activeDice)
            {
                if (dice != null) Destroy(dice.gameObject);
            }
            
            _activeDice.Clear();
        }

        private void ResetDice(DiceController dice)
        {
            var rb = dice.GetComponent<Rigidbody>();
            
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            dice.transform.position = new Vector3(diceSpawnPoint.position.x, diceSpawnPoint.position.y + 2f, diceSpawnPoint.position.z);
            dice.Roll(Vector3.down * 2f, Random.insideUnitSphere * torqueStrength, dice.ForcedValue);
        }
    }
}
