using UnityEditor;
using UnityEngine;
using UEditor = UnityEditor.Editor;

namespace Debugger.Editor
{
    [CustomEditor(typeof(DebuggerRuntimeManager), editorForChildClasses: true)]
    [CanEditMultipleObjects]
    public class DebuggerRuntimeManagerEditor : UEditor
    {
        public override void OnInspectorGUI()
        {
            var manager = (DebuggerRuntimeManager)target;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (GUILayout.Button(text: "Open In Editor"))
                DebuggerEditorWindow.Open();

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            EditorGUI.BeginDisabledGroup(manager.IsActive);

            if (GUILayout.Button(text: "Show"))
                manager.Show();

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!manager.IsActive);

            if (GUILayout.Button(text: "Hide"))
                manager.Hide();

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            base.OnInspectorGUI();
        }
    } 
}
