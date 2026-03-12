using System.Collections;
using UnityEngine;
using Core;
using Cameras;
using Dice;
using Map;
using CameraType = Cameras.CameraType;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState CurrentState { get; private set; }

        [Header("Flow Settings")]
        [SerializeField] private float initialTopViewDuration = 3f;
        [SerializeField] private float topViewAfterMoveDuration = 2f;
        
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
            
            Debug.Log($"[GameManager] State changed to: {newState}");

            switch (newState)
            {
                case GameState.TopView:
                    StartCoroutine(TopViewRoutine());
                    break;
                    
                case GameState.WaitingForRoll:
                    CameraManager.Instance.SwitchCamera(CameraType.Step);
                    break;
                    
                case GameState.RollingDice:
                    StartCoroutine(RollingDiceRoutine());
                    break;
                    
                case GameState.MovingMap:
                    StartCoroutine(MovingMapRoutine());
                    break;
            }
        }

        private IEnumerator TopViewRoutine()
        {
            CameraManager.Instance.SwitchCamera(CameraType.Top);
            
            yield return new WaitForSeconds(topViewAfterMoveDuration);
            
            ChangeState(GameState.WaitingForRoll);
        }
        
        public void OnRollButtonTapped()
        {
            if (CurrentState == GameState.WaitingForRoll)
            {
                ChangeState(GameState.RollingDice);
            }
        }

        private IEnumerator RollingDiceRoutine()
        {
            CameraManager.Instance.SwitchCamera(CameraType.DiceRoll);

            bool isDiceRolling = true;
            int rolledSum = 0;

            // Define Action to listen for when dice stop
            System.Action<int> onDiceStoppedAction = (total) => 
            {
                rolledSum = total;
                isDiceRolling = false;
            };

            // Subscribe to the event
            DiceManager.Instance.OnAllDiceStopped += onDiceStoppedAction;

            // Wait for the camera transition to finish before rolling the dice
            yield return new WaitForSeconds(1.5f); // Adjust this based on Cinemachine blend time

            // Physically throw all dice
            DiceManager.Instance.RollDice();

            // Wait frame by frame until the physical dice stop rolling and event fires
            while (isDiceRolling)
            {
                yield return null;
            }

            // Unsubscribe from the event
            DiceManager.Instance.OnAllDiceStopped -= onDiceStoppedAction;

            _currentStepIndex += rolledSum;
            
            Debug.Log($"[GameManager] Dice Physics Completed! Rolled: {rolledSum}. New Target Step: {_currentStepIndex}");

            // Wait a second to let the player see the stopped dice result
            yield return new WaitForSeconds(1f);
            
            // Clean up the physics dice
            DiceManager.Instance.ClearDice();
            
            ChangeState(GameState.MovingMap);
        }

        private IEnumerator MovingMapRoutine()
        {
            CameraManager.Instance.SwitchCamera(CameraType.Step);
            yield return new WaitForSeconds(0.5f);
            MapManager.Instance.MoveMapToStep(_currentStepIndex, 20f);
            yield return new WaitForSeconds(3f); 
            ChangeState(GameState.TopView);
        }
    }
}
