using System;
using UnityEngine;
using Utils;

namespace Cameras
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }

        [Header("Cameras")]
        [SerializeField] private GameObject topCamera;
        [SerializeField] private GameObject stepCamera;
        [SerializeField] private GameObject diceRollCamera;

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
        
        public void SwitchCamera(CameraType type)
        {
            if (topCamera) topCamera.SetActive(false);
            if (stepCamera) stepCamera.SetActive(false);
            if (diceRollCamera) diceRollCamera.SetActive(false);

            switch (type)
            {
                case CameraType.Top:
                    if (topCamera) topCamera.SetActive(true);
                    break;
                case CameraType.Step:
                    if (stepCamera) stepCamera.SetActive(true);
                    break;
                case CameraType.DiceRoll:
                    if (diceRollCamera) diceRollCamera.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            EditorLogger.Log(nameof(CameraManager), $"Switched to {type} camera.");
        }
    }
}
