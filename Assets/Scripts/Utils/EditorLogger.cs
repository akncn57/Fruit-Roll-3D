using System.Diagnostics;

namespace Utils
{
    public static class EditorLogger
    {
        [Conditional("UNITY_EDITOR")]
        public static void Log(string prefix, string message)
        {
            UnityEngine.Debug.Log($"<b><color=#4CAF50>[{prefix}]</color></b> {message}");
        }

        [Conditional("UNITY_EDITOR")]
        public static void Warning(string prefix, string message)
        {
            UnityEngine.Debug.LogWarning($"<b><color=#FFC107>[{prefix}]</color></b> {message}");
        }

        [Conditional("UNITY_EDITOR")]
        public static void Error(string prefix, string message)
        {
            UnityEngine.Debug.LogError($"<b><color=#F44336>[{prefix}]</color></b> {message}");
        }
    }
}