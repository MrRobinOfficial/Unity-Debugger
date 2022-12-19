using UnityEngine;
using UnityEngine.Events;

namespace uDebugger
{
    [Icon(path: "Assets/uDebugger/Resources/Debug.png")]
    [AddComponentMenu("Tools/uDebugger/Debugger Console")]
    public sealed class DebuggerConsole : MonoBehaviour
    {
        public static event UnityAction OnShowEvent;
        public static event UnityAction OnHideEvent;

        public static DebuggerConsole Instance { get; private set; } = null;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            transform.hideFlags = HideFlags.HideInInspector;
        }

        private void Start()
        {
            
        }

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void Update()
        {
            
        }

        private void OnDestroy()
        {
            
        }

        private void ClearInputField() { }

        private void FocusIputField() { }

        public void Show()
        {

        }

        public void Hide()
        {

        }

        private void OnValueChanged(string newValue) { }

        private void OnSubmit(string value) { }
    }
}
