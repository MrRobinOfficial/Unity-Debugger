using UnityEditor.ShortcutManagement;
using UnityEngine.Windows;
using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;

namespace uDebugger.Editor
{
	public class DebuggerEditorWindow : EditorWindow
	{
        private static DebuggerEditorWindow m_Instance = null;

        private static readonly System.Type m_InspectorType = System.Type.GetType(typeName: "UnityEditor.InspectorWindow,UnityEditor.dll");

        [MenuItem(
            itemName: "Tools/uDebugger/Open Debugger Console [Editor] ^m",
            isValidateFunction: false,
            priority = -5000)]
        private static void Open()
        {
            if (m_Instance != null)
            {
                m_Instance.Close();
                return;
            }

            m_Instance = GetWindow<DebuggerEditorWindow>(
                title: "Debugger Console [Editor]",
                focus: true,
                m_InspectorType);

            m_Instance.titleContent = new GUIContent("Debugger Console [Editor]");

            // Limit size of the window
            m_Instance.minSize = new Vector2(200, 600);
            m_Instance.maxSize = new Vector2(500, 1920);

            m_Instance.Focus();
        }

        private void OnEnable() => DebuggerEditor.OnForceRefresh += DrawContent;

        private void OnDisable() => DebuggerEditor.OnForceRefresh -= DrawContent;

        private void CreateGUI()
        {
            m_Input = string.Empty;
            DrawContent();

            Focus();

            var inputField = rootVisualElement.Q<TextField>("inputField");
            inputField.Focus();
        }

        private static string m_Input = string.Empty;

        private void DrawContent()
        {
            rootVisualElement.Clear();

            var treeClone = Debugger.VisualTreeAsset.CloneTree();

            rootVisualElement.Add(treeClone);
            rootVisualElement.styleSheets.Add(Debugger.StyleSheet);

            var inputField = rootVisualElement.Q<TextField>("inputField");
            inputField.SetValueWithoutNotify(m_Input);

            inputField.style.unityFontStyleAndWeight = new(DebuggerSettingsWindow.instance.fontStyle);

            inputField.style.fontSize = new(DebuggerSettingsWindow.instance.fontSize);

            inputField.RegisterValueChangedCallback(evt =>
            {
                m_Input = evt.newValue;
            });

            inputField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return ||
                    evt.keyCode == KeyCode.KeypadEnter)
                {
                    Debug.Log(inputField.text);

                    inputField.SetValueWithoutNotify(string.Empty);
                    inputField.Focus();
                }
            });
        }

        /*
            % (ctrl on Windows and Linux, cmd on macOS),
            ^ (ctrl on Windows, Linux, and macOS),
            # (shift),
            & (alt).
        */

        //void ResetCaret()
        //{
        //    var text = string.IsNullOrEmpty(inputField.text) ? string.Empty : inputField.text;

        //    inputField.SelectRange(text.Length, text.Length);
        //}

        //void RefreshList(string input)
        //{
        //    var commands = FindCommands(input);

        //    if (commands == null || commands.Count == 0)
        //    {
        //        outputField.value = string.Empty;
        //        return;
        //    }

        //    currentCommand = commands.ElementAt(index: 0);
        //    outputField.value = GetAliasFromCommands(commands);
        //}

        //void HandleKeyCallback(KeyDownEvent ctx)
        //{
        //    switch (ctx.keyCode)
        //    {
        //        case KeyCode.Return:
        //            var input = inputField.text;

        //            inputField.value = string.Empty;
        //            ResetCaret();
        //            inputField.Focus();
        //            RefreshList(inputField.text);

        //            if (SendCommand(input, out var command))
        //            {
        //                if (DebuggerManager.Cache == null)
        //                    return;

        //                if (DebuggerManager.Cache.Count > COMMAND_EXECUTED_CAPAICTY - 1)
        //                    DebuggerManager.Cache.Clear();

        //                const string IGNORE_COMMAND_01 = "clearCache";

        //                if (!DebuggerManager.Cache.Contains(input) && !input.Equals(IGNORE_COMMAND_01))
        //                    DebuggerManager.Cache.Add(input);

        //                ExecutionIndex = DebuggerManager.Cache.Count;
        //            }
        //            break;

        //        case KeyCode.Tab:
        //            if (currentCommand == null)
        //                return;

        //            inputField.value = currentCommand?.alias;

        //            ResetCaret();
        //            RefreshList(inputField.text);
        //            break;

        //        case KeyCode.U when ctx.ctrlKey:

        //            break;

        //        case KeyCode.UpArrow:

        //            if (DebuggerManager.Cache == null || DebuggerManager.Cache.Count == 0)
        //                return;

        //            inputField.value = DebuggerManager.Cache[ExecutionIndex];
        //            ResetCaret();
        //            RefreshList(inputField.text);
        //            ExecutionIndex--;
        //            break;

        //        case KeyCode.DownArrow:

        //            if (DebuggerManager.Cache == null || DebuggerManager.Cache.Count == 0)
        //                return;

        //            //inputField.SetValueWithoutNotify(commandsExecuted[ExecutionIndex].alias);
        //            inputField.value = DebuggerManager.Cache[ExecutionIndex];
        //            ResetCaret();
        //            RefreshList(inputField.text);
        //            ExecutionIndex++;
        //            break;
        //    }
        //}
    }
}