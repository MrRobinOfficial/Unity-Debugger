using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using NaughtyAttributes;

using UEditor = UnityEditor.Editor;

namespace uDebugger.Editor
{
    // TODO: Create UnityEditor UI Elements
    // Enable Password (Use password mask text, so it hides)
    // Create loop for search stylesheet path (dropdown)

    [FilePath("Debugger/DebuggerSettings.dat", FilePathAttribute.Location.ProjectFolder)]
    public class DebuggerSettingsWindow : ScriptableSingleton<DebuggerSettingsWindow>
    {
        public enum DeveloperLogLevel : byte
        {
            Basic,
            Developer,
        }

        [System.Flags]
        public enum LogType : byte
        {
            //
            // Summary:
            //     LogType used for regular log messages.
            Log = 0x1,
            //
            // Summary:
            //     LogType used for Warnings.
            Warning = 0x2,
            //
            // Summary:
            //     LogType used for Errors.
            Error = 0x4,
            //
            // Summary:
            //     LogType used for Exceptions.
            Exception = 0x8,
            //
            // Summary:
            //     LogType used for Asserts. (These could also indicate an error inside Unity itself.)
            Assert = 0x16,
        }

        public enum StyleProfile : byte
        {
            Default,
            Custom,
            Cyber,
            Moon,
            Sunset,
            Xbox,
        }

        [Header("General Config")]
        public DeveloperLogLevel logLevel = DeveloperLogLevel.Basic;
        public LogType logType = LogType.Log | LogType.Exception | LogType.Error | LogType.Warning | LogType.Assert;
        public ushort maxLogSize = 255;
        public bool enableCaseSenstive = false;
        public bool enableTimestamp = true;
        public bool enablePassword = false;
        [ShowIf(nameof(enablePassword))] public string password = default;
        public bool usePrefix = false;
        [ShowIf(nameof(usePrefix))] public char prefix = '/';

        [Header("Style Config")]
        public StyleProfile styleProfile = StyleProfile.Default;
        [ShowIf(nameof(styleProfile), StyleProfile.Custom)] public StyleSheet customStylesheet = default;
        public Font font = default;
        public int fontSize = 12;
        public FontStyle fontStyle = FontStyle.Normal;

        public StyleSheet GetStyleSheet() => styleProfile switch
        {
            StyleProfile.Default => Resources.Load<StyleSheet>("UI/DefaultStylesheet"),
            StyleProfile.Custom => customStylesheet,
            StyleProfile.Cyber => Resources.Load<StyleSheet>("UI/CyberStylesheet"),
            StyleProfile.Moon => Resources.Load<StyleSheet>("UI/MoonStylesheet"),
            StyleProfile.Sunset => Resources.Load<StyleSheet>("UI/SunsetStylesheet"),
            StyleProfile.Xbox => Resources.Load<StyleSheet>("UI/XboxStylesheet"),
            _ => Resources.Load<StyleSheet>("UI/DefaultStylesheet"),
        };

        private static UEditor editor = null;

        [SettingsProvider]
        public static SettingsProvider GetProvider()
        {
            var obj = new SerializedObject(instance);

            var provider = new SettingsProvider("Project/Tools/DebuggerSettings", SettingsScope.Project)
            {
                label = "Debugger Settings",
                guiHandler = searchCtx =>
                {
                    obj.Update();

                    EditorGUI.BeginChangeCheck();

                    if (!editor)
                    {
                        instance.hideFlags = HideFlags.None;
                        UEditor.CreateCachedEditor(instance, editorType: null, ref editor);
                    }

                    editor.OnInspectorGUI();

                    obj.ApplyModifiedPropertiesWithoutUndo();

                    if (EditorGUI.EndChangeCheck())
                        DebuggerEditor.ForceRefresh();
                },
                keywords = new string[] { "Debugger", "Settings" },
            };

            return provider;
        }
    }
}