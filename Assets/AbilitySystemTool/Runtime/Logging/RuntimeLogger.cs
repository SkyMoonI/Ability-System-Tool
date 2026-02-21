using System.Diagnostics;
using UnityEngine;

namespace AbilitySystemTool
{
    internal static class RuntimeLogger
    {
        [Conditional("AST_LOG")]
        public static void Log(string message) => UnityEngine.Debug.Log(message);

        [Conditional("AST_LOG")]
        public static void Warn(string message) => UnityEngine.Debug.LogWarning(message);

        [Conditional("AST_LOG")]
        public static void Warn(string message, Object context) => UnityEngine.Debug.LogWarning(message, context);

        [Conditional("AST_LOG")]
        public static void Error(string message) => UnityEngine.Debug.LogError(message);

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void DevAssert(bool condition, string message, Object context) => UnityEngine.Debug.Assert(condition, message, context);
    }
}