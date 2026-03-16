using System.Collections;
using UnityEngine;
using Core;
using Utils;
using Cameras;
using Dice;
using Map;
using Inventory;
using CameraType = Cameras.CameraType;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState CurrentState { get; private set; }

        [Header("Delay Settings")]
        [SerializeField] private float topViewDuration = 2f;
        [SerializeField] private float cameraSwitchDelayForDice = 1.5f;
        [SerializeField] private float waitAfterDiceStop = 1f;
        [SerializeField] private float cameraSwitchDelayForStep = 2.0f;
        
        private int _currentStepIndex = 0;

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

        private void Start()
        {
            ChangeState(GameState.TopView);
        }

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;
            
            EditorLogger.Log(nameof(GameManager), $"State changed to: {newState}");

            switch (newState)
            {
                case GameState.TopView:
                    UI.GameInfoUIController.Instance?.ShowMessage("Sıradaki tur için haritaya göz atılıyor...");
                    StartCoroutine(TopViewRoutine());
                    break;
                    
                case GameState.WaitingForRoll:
                    UI.GameInfoUIController.Instance?.ShowMessage("İlerlemek için zarları at!");
                    CameraManager.Instance.SwitchCamera(CameraType.Step);
                    break;
                    
                case GameState.RollingDice:
                    UI.GameInfoUIController.Instance?.ShowMessage("Zarlar atılıyor...");
                    StartCoroutine(RollingDiceRoutine());
                    break;
                    
                case GameState.MovingMap:
                    UI.GameInfoUIController.Instance?.ShowMessage("Hedefe ilerleniyor...");
                    StartCoroutine(MovingMapRoutine());
                    break;
            }
        }

        private IEnumerator TopViewRoutine()
        {
            // Switch to top-down map view
            CameraManager.Instance.SwitchCamera(CameraType.Top);
            
            // Wait for duration then prepare for the physical dice roll
            yield return new WaitForSeconds(topViewDuration);
            
            ChangeState(GameState.WaitingForRoll);
        }
        
        public void OnRollButtonTapped()
        {
            // Only allow rolling if we are waiting for player input
            if (CurrentState == GameState.WaitingForRoll)
            {
                ChangeState(GameState.RollingDice);
            }
        }

        private IEnumerator RollingDiceRoutine()
        {
            // Transition to dice view camera
            CameraManager.Instance.SwitchCamera(CameraType.DiceRoll);

            var isDiceRolling = true;
            var rolledSum = 0;

            // Callback triggered when all physical dice stop moving
            var onDiceStoppedAction = (System.Action<int>)((total) => 
            {
                rolledSum = total;
                isDiceRolling = false;
            });

            DiceManager.Instance.OnAllDiceStopped += onDiceStoppedAction;

            // Wait for the Cinemachine blend transition to finish before rolling
            yield return new WaitForSeconds(cameraSwitchDelayForDice);

            DiceManager.Instance.RollDice();

            // Hold execution until our action sets isDiceRolling to false
            while (isDiceRolling)
            {
                yield return null;
            }

            DiceManager.Instance.OnAllDiceStopped -= onDiceStoppedAction;

            _currentStepIndex += rolledSum;
            
            EditorLogger.Log(nameof(GameManager), $"Dice Physics Completed! Rolled: {rolledSum}. New Target Step: {_currentStepIndex}");

            UI.GameInfoUIController.Instance?.ShowMessage($"Zarları attın! Toplam {rolledSum} geldi, {rolledSum} adım ilerliyorsun!");

            // Let the player review the rolled output before snapping the camera back
            yield return new WaitForSeconds(waitAfterDiceStop);
            
            DiceManager.Instance.ClearDice();
            
            ChangeState(GameState.MovingMap);
        }

        private IEnumerator MovingMapRoutine()
        {
            // Focus on the character
            CameraManager.Instance.SwitchCamera(CameraType.Step);
            
            // Wait shortly before moving
            yield return new WaitForSeconds(cameraSwitchDelayForStep);
            
            var isMapMoving = true;
            
            // Callback triggered when map reaches the target step
            var onMapMovedAction = (System.Action)(() =>
            {
                isMapMoving = false;
            });

            MapManager.Instance.OnMapMovementCompleted += onMapMovedAction;
            
            // Start map scrolling transition
            MapManager.Instance.MoveMapToStep(_currentStepIndex, 20f);
            
            // Wait until movement finishes
            while (isMapMoving)
            {
                yield return null;
            }
            
            MapManager.Instance.OnMapMovementCompleted -= onMapMovedAction;
            
            // Check the landed step's type
            var stepData = MapManager.Instance.GetStepData(_currentStepIndex);
            if (stepData != null)
            {
                if (stepData.Type == StepType.ResetData)
                {
                    UI.GameInfoUIController.Instance?.ShowMessage("Eyvah! Tüm ödüllerin silindi ve başlangıca dönüyorsun.");
                    InventoryManager.Instance.ClearInventory();
                    EditorLogger.Log(nameof(GameManager), "Landed on ResetData tile! Clearing inventory and restarting...");
                    yield return new WaitForSeconds(2.5f);
                    _currentStepIndex = 0;
                    ChangeState(GameState.MovingMap);
                    yield break;
                }
                else if (stepData.Type == StepType.ReturnToStart)
                {
                    UI.GameInfoUIController.Instance?.ShowMessage("Başlangıca geri dönüyorsun!");
                    EditorLogger.Log(nameof(GameManager), "Landed on ReturnToStart tile! Restarting...");
                    yield return new WaitForSeconds(2.5f);
                    _currentStepIndex = 0;
                    ChangeState(GameState.MovingMap);
                    yield break; // Stop this routine and let the new state transition take over
                }
                else // StepType.Normal
                {
                    // Check for reward on the landed step and add to inventory
                    if (stepData.Reward != null && stepData.Reward.Amount > 0)
                    {
                        UI.GameInfoUIController.Instance?.ShowMessage($"Tebrikler! {stepData.Reward.Amount}x {stepData.Reward.Type} kazandın!");
                        // Add silently to prevent instant UI jump
                        InventoryManager.Instance.AddItem(stepData.Reward, silent: true);
                        
                        // The map moves so the landed step is near the origin (where the character is)
                        Vector3 tilePos = Vector3.zero; 
                        
                        // Fire the visual reward animator
                        if (UI.InOutManager.Instance != null)
                        {
                            UI.InOutManager.Instance.AnimateReward(stepData.Reward.Type, stepData.Reward.Amount, tilePos);
                        }
                        else
                        {
                            EditorLogger.Warning(nameof(GameManager), "RewardUIAnimator is missing. Visuals skipped.");
                        }
                        
                        yield return new WaitForSeconds(2f);
                    }
                    else
                    {
                        UI.GameInfoUIController.Instance?.ShowMessage("Boş bir kareye geldin.");
                        yield return new WaitForSeconds(1.5f);
                    }
                }
            }
            
            // Movement complete, back to top view observation
            ChangeState(GameState.TopView);
        }
    }
}
