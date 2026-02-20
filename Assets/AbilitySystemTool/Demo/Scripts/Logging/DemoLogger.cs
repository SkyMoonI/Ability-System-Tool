using System.Diagnostics;
using UnityEngine;

namespace AbilitySystemTool
{
    internal static class DemoLogger
    {
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Log(string message) => UnityEngine.Debug.Log(message);

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Warn(string message) => UnityEngine.Debug.LogWarning(message);

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Warn(string message, Object context) => UnityEngine.Debug.LogWarning(message, context);

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Error(string message) => UnityEngine.Debug.LogError(message);
    }
}