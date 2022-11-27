using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using NaughtyAttributes;
using TMPro;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Debugger
{
    [AddComponentMenu("Tools/Debugger/Debugger Manager [Runtime]"), DisallowMultipleComponent]
    public class DebuggerRuntimeManager : MonoBehaviour
    {
        public static event UnityAction OnShowEvent;
        public static event UnityAction OnHideEvent;

        public static DebuggerRuntimeManager Instance { get; private set; } = null;

        [Header("References")]
        [SerializeField, Required] Canvas m_Canvas = default; 
        [SerializeField, Required] TMP_InputField m_InputField = default;
        [SerializeField] Button m_RefreshBtn = default;
        [SerializeField] Button m_SubmitBtn = default;
        [SerializeField] Button m_ExitBtn = default;

        [Header("Input")]
#if ENABLE_INPUT_SYSTEM
        [SerializeField] Key m_ToggleConsoleKey = Key.Backquote;
        [SerializeField] Key m_CompletionKey = Key.Tab;
#else
        [SerializeField] KeyCode m_ToggleConsoleKey = KeyCode.Backslash;
        [SerializeField] KeyCode m_CompletionKey = KeyCode.Tab;
#endif

        [Header("Events"), Space]
        [SerializeField] UnityEvent m_OnShowEvent;
        [SerializeField] UnityEvent m_OnHideEvent;

        private const string k_PrefabPath = "Prefabs/Debugger Manager";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            if (FindObjectOfType<DebuggerRuntimeManager>() != null)
                return;

            Instantiate(Resources.Load<GameObject>(k_PrefabPath));
        }

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

        private void OnEnable()
        {
            Hide();

            if (m_RefreshBtn != null)
                m_RefreshBtn.onClick.AddListener(OnRefresh);

            if (m_SubmitBtn != null)
                m_SubmitBtn.onClick.AddListener(OnSubmit);

            if (m_ExitBtn != null)
                m_ExitBtn.onClick.AddListener(OnExit);

            m_InputField.onSubmit.AddListener(OnSubmit);
            m_InputField.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDisable()
        {
            if (m_RefreshBtn != null)
                m_RefreshBtn.onClick.RemoveAllListeners();

            if (m_SubmitBtn != null)
                m_SubmitBtn.onClick.RemoveAllListeners();

            if (m_ExitBtn != null)
                m_ExitBtn.onClick.RemoveAllListeners();

            m_InputField.onSubmit.RemoveAllListeners();
            m_InputField.onValueChanged.RemoveAllListeners();
        }

        private void Update() => HandleInput();

        private void HandleInput()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current[m_ToggleConsoleKey].wasPressedThisFrame)
            {
                if (IsActive)
                    Hide();
                else
                    Show();
            }
#else
            if (Input.GetKeyDown(m_ToggleConsoleKey))
            {
                if (IsActive)
                    Hide();
                else
                    Show();
            }
#endif
        }

        public bool IsActive => m_Canvas.gameObject.activeInHierarchy;

        private CursorLockMode cursorLock;
        private bool cursorVisible;

        private void ClearInputField() => m_InputField.SetTextWithoutNotify(string.Empty);

        private void FocusIputField()
        {
            if (EventSystem.current == null)
                return;

            var e = EventSystem.current;
            e.SetSelectedGameObject(m_InputField.gameObject, pointer: null);
            m_InputField.OnPointerClick(eventData: new PointerEventData(e));
        }

        [ContextMenu(itemName: "Show", isValidateFunction: false, priority: 1000150)]
        public void Show()
        {
            cursorVisible = Cursor.visible;
            cursorLock = Cursor.lockState;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            ClearInputField();
            m_Canvas.gameObject.SetActive(true);
            FocusIputField();

            m_OnShowEvent?.Invoke();
            OnShowEvent?.Invoke();
        }

        [ContextMenu(itemName: "Hide", isValidateFunction: false, priority: 1000150)]
        public void Hide()
        {
            Cursor.visible = cursorVisible;
            Cursor.lockState = cursorLock;

            m_Canvas.gameObject.SetActive(false);

            m_OnHideEvent?.Invoke();
            OnHideEvent?.Invoke();
        }

        private async System.Threading.Tasks.Task Internal_ShowAnimation() => 
            await System.Threading.Tasks.Task.CompletedTask;

        private async System.Threading.Tasks.Task Internal_HideAnimation() =>
            await System.Threading.Tasks.Task.CompletedTask;

#if UNITY_EDITOR
        [ContextMenu(itemName: "Show", isValidateFunction: true, priority: 1000150)]
        internal bool CanShow() => !m_Canvas.gameObject.activeInHierarchy;

        [ContextMenu(itemName: "Hide", isValidateFunction: true, priority: 1000150)]
        internal bool CanHide() => m_Canvas.gameObject.activeInHierarchy;
#endif

        private void OnValueChanged(string newValue)
        {
            // TODO: Refresh list
        }

        private void OnSubmit(string input)
        {
            //DebuggerManager.SendCommand(input);

            ClearInputField();
            FocusIputField();
        }

        #region Button Callbacks

        private void OnRefresh() { }
        private void OnSubmit() => OnSubmit(m_InputField.text);
        private void OnExit() => Hide();

        #endregion
    }
}