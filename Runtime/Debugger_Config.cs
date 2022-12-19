using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace uDebugger
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static partial class Debugger
    {
        private const string k_DefaultStyleSheetPath = "StyleSheets/Default_DebuggerStyle";

        private const string k_DefaultXMLPath = "UI/DebuggerUXML";

        private static StyleSheet m_StyleSheet = null;
        private static VisualTreeAsset m_VisualTreeAsset = null;

        static Debugger()
        {
            if (m_StyleSheet == null)
                m_StyleSheet = Resources.Load<StyleSheet>(k_DefaultStyleSheetPath);

            if (m_VisualTreeAsset == null)
                m_VisualTreeAsset = Resources.Load<VisualTreeAsset>(k_DefaultXMLPath);

            Debug.Log(m_VisualTreeAsset);
        }

        public static StyleSheet StyleSheet
        {
            get => m_StyleSheet;
            set
            {
                m_StyleSheet = value;
            }
        }

        public static VisualTreeAsset VisualTreeAsset
        {
            get => m_VisualTreeAsset;
            set
            {
                m_VisualTreeAsset = value;
            }
        }
	} 
}
