using Input;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RebindActionUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI actionNameText;
    [SerializeField] private Image bindingIconImage;
    [SerializeField] private TextMeshProUGUI bindingFallbackText; // Optional
    [SerializeField] private Button rebindButton;

    [Header("System References")]
    [Tooltip("Assign your Icon Database ScriptableObject here.")]
    [SerializeField] private InputIconDatabase iconDatabase;
    private PlayerInput m_PlayerInput;

    private InputAction m_ActionToRebind;
    private InputActionRebindingExtensions.RebindingOperation m_RebindingOperation;
    
    public void Initialize(InputAction action, PlayerInput input)
    {
        m_ActionToRebind = action;
        m_PlayerInput = input;
        actionNameText.text = action.name;

        rebindButton.onClick.AddListener(StartRebinding);
        
        InputSystemEvents.OnBindingsChanged += UpdateBindingDisplay;
        m_PlayerInput.onControlsChanged += OnDeviceChanged;

        UpdateBindingDisplay();
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent errors
        InputSystemEvents.OnBindingsChanged -= UpdateBindingDisplay;
        if (m_PlayerInput != null)
        {
            m_PlayerInput.onControlsChanged -= OnDeviceChanged;
        }
        if (m_RebindingOperation != null)
        {
            m_RebindingOperation.Dispose();
        }
    }

    private void OnDeviceChanged(PlayerInput input)
    {
        UpdateBindingDisplay();
    }

    public void UpdateBindingDisplay()
    {
        if (m_ActionToRebind == null || m_PlayerInput == null || iconDatabase == null) return;

        string deviceName = m_PlayerInput.currentControlScheme;
        int bindingIndex = m_ActionToRebind.GetBindingIndex(group: deviceName);
        if (bindingIndex == -1) bindingIndex = 0;

        string fullPath = m_ActionToRebind.bindings[bindingIndex].effectivePath;
        int slashIndex = fullPath.IndexOf('/');
        string bindingId = (slashIndex == -1) ? fullPath : fullPath.Substring(slashIndex + 1);

        Sprite icon = null;
        if (!string.IsNullOrEmpty(bindingId))
        {
            icon = iconDatabase.FindIcon(deviceName, bindingId);
        }
        
        if (icon != null)
        {
            bindingIconImage.sprite = icon;
            bindingIconImage.enabled = true;
            if (bindingFallbackText != null) bindingFallbackText.enabled = false;
        }
        else
        {
            bindingIconImage.enabled = false;
            if (bindingFallbackText != null)
            {
                bindingFallbackText.enabled = true;
                bindingFallbackText.text = InputControlPath.ToHumanReadableString(
                    fullPath, InputControlPath.HumanReadableStringOptions.OmitDevice).ToUpper();
            }
        }
    }

    private void StartRebinding()
    {
        rebindButton.interactable = false;
        m_ActionToRebind.Disable();
        GameObject buttonToReselect = gameObject;

        m_RebindingOperation = m_ActionToRebind.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnComplete(operation =>
            {
                operation.Dispose();
                m_ActionToRebind.Enable();
                rebindButton.interactable = true;
                SaveBindingOverrides();
                InputSystemEvents.InvokeBindingsChanged();
                EventSystem.current.SetSelectedGameObject(buttonToReselect);
            })
            .Start();
    }

    private void SaveBindingOverrides()
    {
        if (m_PlayerInput != null)
        {
            string rebinds = m_PlayerInput.actions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString("keyRebinds", rebinds);
            PlayerPrefs.Save();
        }
    }
}