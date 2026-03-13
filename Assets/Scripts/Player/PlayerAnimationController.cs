using UnityEngine;
using Map;
using Utils;

namespace Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator animator;

        [Header("Animation Parameters")]
        [SerializeField] private string isWalkingParameter = "IsWalking";

        private int _isWalkingHash;

        private void Awake()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            _isWalkingHash = Animator.StringToHash(isWalkingParameter);
        }

        private void Start()
        {
            if (MapManager.Instance != null)
            {
                MapManager.Instance.OnMapMovementStarted += HandleMovementStarted;
                MapManager.Instance.OnMapMovementCompleted += HandleMovementStopped;
            }
            else
            {
                EditorLogger.Error(nameof(PlayerAnimationController), "MapManager is not initialized before PlayerAnimationController.");
            }
        }

        private void OnDestroy()
        {
            if (MapManager.Instance != null)
            {
                MapManager.Instance.OnMapMovementStarted -= HandleMovementStarted;
                MapManager.Instance.OnMapMovementCompleted -= HandleMovementStopped;
            }
        }

        private void HandleMovementStarted()
        {
            if (animator != null)
            {
                animator.SetBool(_isWalkingHash, true);
                EditorLogger.Log(nameof(PlayerAnimationController), "Started Walk Animation");
            }
        }

        private void HandleMovementStopped()
        {
            if (animator != null)
            {
                animator.SetBool(_isWalkingHash, false);
                EditorLogger.Log(nameof(PlayerAnimationController), "Stopped Walk Animation");
            }
        }
    }
}
