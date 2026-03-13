using UnityEngine;

namespace Environment
{
    public class ObjectRotator : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [Tooltip("The axis and speed of rotation in degrees per second.")]
        [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 0, 100f);
        
        [Tooltip("Whether the rotation is relative to the object's local space or world space.")]
        [SerializeField] private Space rotationSpace = Space.Self;

        private void Update()
        {
            // Rotate the object continuously based on the set speed and time
            transform.Rotate(rotationSpeed * Time.deltaTime, rotationSpace);
        }
    }
}
