using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This makes it easy to create an instance of this asset in the Project window.
namespace Input
{
    [CreateAssetMenu(fileName = "InputIconDatabase", menuName = "Input/Icon Database")]
    public class InputIconDatabase : ScriptableObject
    {
        [System.Serializable]
        public class DeviceIcons
        {
            public string deviceName; // e.g., "Keyboard", "DualSense", "Xbox Controller"
            public List<BindingIcon> icons;
        }

        [System.Serializable]
        public class BindingIcon
        {
            // This is the path the Input System uses, e.g., "buttonSouth"
            [Tooltip("The ID/Path of the binding, e.g., 'buttonSouth', 'dpad/up', 'leftStickPress'")]
            public string bindingId;
            public Sprite icon;
        }

        public List<DeviceIcons> deviceIconSets;

        // A helper function to find the right icon.
        public Sprite FindIcon(string deviceName, string bindingId)
        {
            // Find the correct set of icons for the device.
            DeviceIcons deviceSet = deviceIconSets.FirstOrDefault(d => d.deviceName == deviceName);

            if (deviceSet != null)
            {
                // Find the specific icon for the binding within that set.
                BindingIcon bindingIcon = deviceSet.icons.FirstOrDefault(i => i.bindingId.ToLower() == bindingId.ToLower());
                if (bindingIcon != null)
                {
                    return bindingIcon.icon;
                }
            }
        
            // Return null if no icon is found.
            return null;
        }
    }
}