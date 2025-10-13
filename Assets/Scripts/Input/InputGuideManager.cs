using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputGuideManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private InputIconDatabase iconDatabase;
        [SerializeField] private GameObject guideRowPrefab;
        [SerializeField] private Transform container;

        // List of actions you want to display in the guide.
        [SerializeField] private List<InputActionReference> actionsToShow;

        private string m_CurrentDeviceName = "";
        private List<GuideRowUI> m_GeneratedRows = new List<GuideRowUI>();

        private void OnEnable()
        {
            playerInput.onControlsChanged += OnDeviceChanged;
            GenerateGuide();
        }

        private void OnDisable()
        {
            playerInput.onControlsChanged -= OnDeviceChanged;
        }

        private void OnDeviceChanged(PlayerInput input)
        {
            UpdateGuide();
        }

        private void GenerateGuide()
        {
            // Clear any existing rows.
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
            m_GeneratedRows.Clear();

            // Create a row for each action.
            foreach (var actionRef in actionsToShow)
            {
                GameObject rowObj = Instantiate(guideRowPrefab, container);
                GuideRowUI rowUI = rowObj.GetComponent<GuideRowUI>();
                rowUI.Setup(actionRef);
                m_GeneratedRows.Add(rowUI);
            }
            UpdateGuide();
        }

        private void UpdateGuide()
        {
            if (playerInput.currentControlScheme == null) return;
        
            string deviceName = playerInput.currentControlScheme;

            // Update each row with the correct icon from the database.
            foreach (var row in m_GeneratedRows)
            {
                int bindingIndex = row.Action.action.GetBindingIndexForControl(playerInput.devices[0]);
                if (bindingIndex < 0) bindingIndex = 0; // Fallback to the first binding.

                string bindingId = row.Action.action.bindings[bindingIndex].effectivePath;
                // The path includes the device, e.g., "<Gamepad>/buttonSouth". We just want "buttonSouth".
                bindingId = bindingId.Split('/').Last();
            
                Sprite icon = iconDatabase.FindIcon(deviceName, bindingId);
                row.SetIcon(icon);
            }
        }
    }
}