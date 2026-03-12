using UnityEngine;

namespace Dice
{
    public class DiceFace : MonoBehaviour
    {
        public int faceValue;
        
        public bool IsFaceUp()
        {
            return Vector3.Dot(transform.up, Vector3.up) > 0.95f; 
        }
    }
}
