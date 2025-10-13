using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Input
{
    public class InputIconUpdater : MonoBehaviour
    {
        [System.Serializable]
        public class ActionIconPair
        {
            public InputActionReference action;
            public Image iconImage;
        }

        [Header("Core References")]
        [Tooltip("Reference to the PlayerInput component to detect the current device.")]
        [SerializeField] private PlayerInput playerInput;

        [Tooltip("The ScriptableObject database containing device-specific icons.")]
        [SerializeField] private InputIconDatabase iconDatabase;

        [Header("Icons to Manage")]
        [Tooltip("The list of actions and the UI images they should update.")]
        [SerializeField] private List<ActionIconPair> managedIcons;

        private void OnEnable()
        {
            if (playerInput != null)
                playerInput.onControlsChanged += OnDeviceChanged;
            InputSystemEvents.OnBindingsChanged += UpdateAllIcons;
            UpdateAllIcons();
        }

        private void OnDisable()
        {
            if (playerInput != null)
                playerInput.onControlsChanged -= OnDeviceChanged;
            InputSystemEvents.OnBindingsChanged -= UpdateAllIcons;
        }

        private void OnDeviceChanged(PlayerInput input)
        {
            UpdateAllIcons();
        }

        public void UpdateAllIcons()
        {
            if (playerInput == null || iconDatabase == null) return;
            
            string deviceName = playerInput.currentControlScheme;
            Debug.Log(deviceName);
            
            foreach (var pair in managedIcons)
            {
                if (pair.action == null || pair.iconImage == null) continue;
                
                int bindingIndex = pair.action.action.GetBindingIndex(group: deviceName);
                if (bindingIndex == -1)
                    bindingIndex = 0;
                string fullPath = pair.action.action.bindings[bindingIndex].effectivePath;
                int slashIndex = fullPath.IndexOf('/');
                string bindingId = (slashIndex == -1) ? fullPath : fullPath.Substring(slashIndex + 1);

                if (!string.IsNullOrEmpty(bindingId))
                {
                    var icon = iconDatabase.FindIcon(deviceName, bindingId);
                    
                    pair.iconImage.sprite = icon;
                    pair.iconImage.enabled = (icon != null);
                }
            }
        }
    }
}