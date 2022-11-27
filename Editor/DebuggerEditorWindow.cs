using UnityEditor.ShortcutManagement;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using System.Linq;

using static Debugger.DebuggerExtensions;
using static Debugger.DebuggerManager;
//using static Debugger.DebuggerSettings;

namespace Debugger.Editor
{
    public class DebuggerEditorWindow : UnityEditor.EditorWindow
    {
        private const string COUNT_COLOR = "#ff8000";
        private const string UXML_PATH = "Assets/Debugger/Resources/UI/DebuggerEditor.uxml";

        private static readonly Vector2 MIN_SIZE = new(300, 700);
        public static DebuggerEditorWindow Instance { get; private set; } = null;

        [Shortcut
        (
            id: "Open_Debugger_Editor",
            defaultKeyCode: KeyCode.M,
            defaultShortcutModifiers: ShortcutModifiers.Control
        )]
        public static void Toggle()
        {
            if (Instance == null)
                Open();
            else
                Instance.Close();
        }

        [MenuItem("Tools/Debugger/Debugger Editor", priority = 0)]
        public static void Open()
        {
            var windowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            Instance = GetWindow<DebuggerEditorWindow>(title: "Debugger Editor", windowType);
            Instance.minSize = MIN_SIZE;
            Instance.Show();
        }

        private void OnEnable() => DebuggerManager.OnInit += DebuggerManager_OnInit;

        private void OnDisable() => DebuggerManager.OnInit -= DebuggerManager_OnInit;

        private void CreateGUI()
        {
            rootVisualElement.Clear();

            var root = rootVisualElement;

            //var treeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            var treeAsset = Resources.Load<VisualTreeAsset>("UI/DebuggerEditor");
            var treeUXML = treeAsset.CloneTree();
            root.Add(treeUXML);

            if (previousSheet != null)
                root.styleSheets.Remove(previousSheet);

            var styleSheet = DebuggerSettings.instance.GetStyleSheet();

            if (styleSheet != null)
                root.styleSheets.Add(styleSheet);

            root.Bind(new SerializedObject(this));

            var methodCount = root.Q<Label>(name = "MethodCount");
            var fieldCount = root.Q<Label>(name = "FieldCount");
            var timeSpan = root.Q<Label>(name = "TimeSpan");

            //methodCount.text = $"{Methods.Count} Methods Collected";
            //fieldCount.text = $"{Fields.Count} Fields Collected";
            //timeSpan.text = $"Command Collection took {CollectionElapsed.TotalMilliseconds:0.0} ms";

            var outputField = root.Q<TextField>(name = "OutputField");
            outputField.style.unityFont = DebuggerSettings.instance.font;
            outputField.style.fontSize = DebuggerSettings.instance.fontSize;
            outputField.style.unityFontStyleAndWeight = DebuggerSettings.instance.fontStyle;

            var inputField = root.Q<TextField>(name = "InputField");
            inputField.style.unityFont = DebuggerSettings.instance.font;
            inputField.style.fontSize = DebuggerSettings.instance.fontSize;
            inputField.style.unityFontStyleAndWeight = DebuggerSettings.instance.fontStyle;
            inputField.Focus();
            
            inputField.RegisterCallback<KeyDownEvent>(ctx => HandleKeyCallback(ctx));

            inputField.RegisterValueChangedCallback(ctx =>
            {
                //inputField.isPasswordField = IsTypingPassword;

                if (ctx.newValue == ctx.previousValue)
                    return;

                RefreshList(ctx.newValue);
            });

            void ResetCaret()
            {
                var text = string.IsNullOrEmpty(inputField.text) ? string.Empty : inputField.text;

                inputField.SelectRange(text.Length, text.Length);
            }

            void RefreshList(string input)
            {
                //var commands = FindCommands(input);

                //if (commands == null || commands.Count == 0)
                //{
                //    outputField.value = string.Empty;
                //    return;
                //}

                //currentCommand = commands.ElementAt(index: 0);
                //outputField.value = GetAliasFromCommands(commands);
            }

            void HandleKeyCallback(KeyDownEvent ctx)
            {
                switch (ctx.keyCode)
                {
                    case KeyCode.Return:
                        var input = inputField.text;

                        inputField.value = string.Empty;
                        ResetCaret();
                        inputField.Focus();
                        RefreshList(inputField.text);

                        if (SendCommand(input, out var command))
                        {
                            if (DebuggerManager.Cache == null)
                                return;

                            if (DebuggerManager.Cache.Count > COMMAND_EXECUTED_CAPAICTY - 1)
                                DebuggerManager.Cache.Clear();

                            const string IGNORE_COMMAND_01 = "clearCache";

                            if (!DebuggerManager.Cache.Contains(input) && !input.Equals(IGNORE_COMMAND_01))
                                DebuggerManager.Cache.Add(input);

                            ExecutionIndex = DebuggerManager.Cache.Count;
                        }
                        break;

                    case KeyCode.Tab:
                        if (currentCommand == null)
                            return;

                        inputField.value = currentCommand?.alias;

                        ResetCaret();
                        RefreshList(inputField.text);
                        break;

                    case KeyCode.U when ctx.ctrlKey:

                        break;

                    case KeyCode.UpArrow:

                        if (DebuggerManager.Cache == null || DebuggerManager.Cache.Count == 0)
                            return;

                        inputField.value = DebuggerManager.Cache[ExecutionIndex];
                        ResetCaret();
                        RefreshList(inputField.text);
                        ExecutionIndex--;
                        break;

                    case KeyCode.DownArrow:

                        if (DebuggerManager.Cache == null || DebuggerManager.Cache.Count == 0)
                            return;

                        //inputField.SetValueWithoutNotify(commandsExecuted[ExecutionIndex].alias);
                        inputField.value = DebuggerManager.Cache[ExecutionIndex];
                        ResetCaret();
                        RefreshList(inputField.text);
                        ExecutionIndex++;
                        break;
                }
            }

            previousSheet = styleSheet;
        }

        //styleSheet = DebuggerSettings.instance.GetStyleSheet();

        private Command? currentCommand;
        private StyleSheet previousSheet;

        private void DebuggerManager_OnInit() => CreateGUI();
    }
}