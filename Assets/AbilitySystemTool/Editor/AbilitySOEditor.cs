using System.Collections.Generic;
using UnityEditor;

namespace AbilitySystemTool
{
    [CustomEditor(typeof(AbilitySO))]
    public sealed class AbilitySOEditor : Editor
    {
        private List<ValidationMessage> _messages;

        private void OnEnable()
        {
            _messages ??= new List<ValidationMessage>(8);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _messages.Clear();
            AbilityValidation.Validate((AbilitySO)target, _messages);
            ValidationSummaryGUI.Draw(_messages);

            EditorGUILayout.Space();
            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }

    }
}