// Assets/Editor/AssetPathDumper.cs
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AbilitySystemTool
{
    internal static class AssetPathDumper
    {
        [MenuItem("Tools/Export/Copy Asset Paths (Selection or Assets)")]
        public static void CopyAssetPaths()
        {
            // If selection is empty, use "Assets"
            string[] roots = Selection.assetGUIDs != null && Selection.assetGUIDs.Length > 0
                ? Selection.assetGUIDs.Select(g => AssetDatabase.GUIDToAssetPath(g)).ToArray()
                : new[] { "Assets" };

            // Folder + files' GUID (selected roots' under)
            var guids = AssetDatabase.FindAssets("", roots);

            var sb = new StringBuilder(guids.Length * 40);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                sb.AppendLine(path);
            }

            EditorGUIUtility.systemCopyBuffer = sb.ToString();
            Debug.Log($"Copied {guids.Length} asset paths to clipboard.");
        }
    }
}