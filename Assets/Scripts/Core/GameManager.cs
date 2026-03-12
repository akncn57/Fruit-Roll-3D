using System.Collections;
using UnityEngine;
using Core;
using Cameras;
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
            
            var diceAnimationLength = Random.Range(2f, 3.5f);
            yield return new WaitForSeconds(diceAnimationLength);
            
            var rolledDice = Random.Range(1, 6);
            _currentStepIndex += rolledDice;
            
            Debug.Log($"[GameManager] Dice Rolled! Rolled: {rolledDice}. New Target Step: {_currentStepIndex}");
            
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
