// Attach this script to your Action Row Prefab

using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Required for .Contains() on the binding groups

namespace Input
{
    public class RebindActionUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI actionNameText;
        [SerializeField] private TextMeshProUGUI bindingText;
        [SerializeField] private Button rebindButton;

        private InputAction m_ActionToRebind;
        private PlayerInput m_PlayerInput; // Reference to the PlayerInput component
        private InputActionRebindingExtensions.RebindingOperation m_RebindingOperation;

        // The Initialize method now accepts the PlayerInput component
        public void Initialize(InputAction action, PlayerInput input)
        {
            m_ActionToRebind = action;
            m_PlayerInput = input;
            actionNameText.text = action.name;

            rebindButton.onClick.AddListener(StartRebinding);
            UpdateBindingDisplay();
        }

        public void UpdateBindingDisplay()
        {
            if (m_ActionToRebind == null || m_PlayerInput == null) return;

            // Find the binding index that matches the current control scheme
            int bindingIndex = -1;
            string currentControlScheme = m_PlayerInput.currentControlScheme;

            for (int i = 0; i < m_ActionToRebind.bindings.Count; i++)
            {
                // Check if the binding's groups match the current control scheme
                if (!string.IsNullOrEmpty(m_ActionToRebind.bindings[i].groups) && 
                    m_ActionToRebind.bindings[i].groups.Contains(currentControlScheme))
                {
                    bindingIndex = i;
                    break;
                }
            }
        
            // If no scheme-specific binding is found, fall back to the first one
            if (bindingIndex == -1)
            {
                bindingIndex = 0;
            }

            bindingText.text = InputControlPath.ToHumanReadableString(
                m_ActionToRebind.bindings[bindingIndex].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
    
        // The rest of the script (StartRebinding, SaveBindingOverrides, etc.) remains the same...

        private void StartRebinding()
        {
            // You could show a "Waiting for input..." panel here
            rebindButton.interactable = false;
            m_ActionToRebind.Disable();

            m_RebindingOperation = m_ActionToRebind.PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse") // Optional: Exclude certain devices
                .OnComplete(operation =>
                {
                    operation.Dispose();
                    m_ActionToRebind.Enable();
                    rebindButton.interactable = true;
                    UpdateBindingDisplay();
                    SaveBindingOverrides();
                    InputSystemEvents.InvokeBindingsChanged();
                })
                .Start();
        }

        private void OnDestroy()
        {
            if (m_RebindingOperation != null)
            {
                m_RebindingOperation.Dispose();
            }
        }

        private void SaveBindingOverrides()
        {
            if(m_PlayerInput != null)
            {
                string rebinds = m_PlayerInput.actions.SaveBindingOverridesAsJson();
                PlayerPrefs.SetString("keyRebinds", rebinds);
                PlayerPrefs.Save();
            }
        }
    }
}