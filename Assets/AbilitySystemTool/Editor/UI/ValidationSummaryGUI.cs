using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AbilitySystemTool
{
    public static class ValidationSummaryGUI
    {
        public static void Draw(List<ValidationMessage> messages)
        {
            if (messages == null) return;

            int errors = 0, warnings = 0, infos = 0;
            for (int i = 0; i < messages.Count; i++)
            {
                switch (messages[i].Severity)
                {
                    case ValidationSeverity.Error: errors++; break;
                    case ValidationSeverity.Warning: warnings++; break;
                    default: infos++; break;
                }
            }

            // Header
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField(
                    $"Validation Summary: {errors} Errors, {warnings} Warnings, {infos} Infos",
                    EditorStyles.boldLabel
                );

                if (messages.Count == 0)
                {
                    EditorGUILayout.LabelField("No issues found.");
                    return;
                }

                DrawGroup(messages, ValidationSeverity.Error);
                DrawGroup(messages, ValidationSeverity.Warning);
                DrawGroup(messages, ValidationSeverity.Info);
            }
        }

        private static void DrawGroup(List<ValidationMessage> messages, ValidationSeverity severity)
        {
            GUIContent icon = GetIcon(severity);

            for (int i = 0; i < messages.Count; i++)
            {
                ValidationMessage message = messages[i];
                if (message.Severity != severity) continue;

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label(icon, GUILayout.Width(18f), GUILayout.Height(18f));
                    EditorGUILayout.LabelField(message.Message, EditorStyles.wordWrappedLabel);

                    if (message.Context != null)
                    {
                        if (GUILayout.Button("Ping", GUILayout.Width(44f)))
                        {
                            EditorGUIUtility.PingObject(message.Context);
                            Selection.activeObject = message.Context;
                        }
                    }
                }
            }
        }

        private static GUIContent GetIcon(ValidationSeverity severity)
        {
            switch (severity)
            {
                case ValidationSeverity.Error: return EditorGUIUtility.IconContent("console.erroricon");
                case ValidationSeverity.Warning: return EditorGUIUtility.IconContent("console.warnicon");
                default: return EditorGUIUtility.IconContent("console.infoicon");
            }
        }
    }
}