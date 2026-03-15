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
            public string deviceName;
            public List<BindingIcon> icons;
        }
        
        [System.Serializable]
        public class DeviceDisplay
        {
            public string deviceName;
            public Sprite deviceSprite;
        }

        [System.Serializable]
        public class BindingIcon
        {
            [Tooltip("The ID/Path of the binding, e.g., 'buttonSouth', 'dpad/up', 'leftStickPress'")]
            public string bindingId;
            public Sprite icon;
        }

        public List<DeviceIcons> deviceIconSets;
        
        [Header("Device Display Sprites")]
        public List<DeviceDisplay> deviceDisplays;
        
        
        public Sprite FindIcon(string deviceName, string bindingId)
        {
            
            DeviceIcons deviceSet = deviceIconSets.FirstOrDefault(d => d.deviceName == deviceName);

            if (deviceSet != null)
            {
                BindingIcon bindingIcon = deviceSet.icons.FirstOrDefault(i => i.bindingId.ToLower() == bindingId.ToLower());
                if (bindingIcon != null)
                {
                    return bindingIcon.icon;
                }
            }
            
            return null;
        }
        
        public Sprite FindDeviceSprite(string deviceName)
        {
            return deviceDisplays.Find(d => d.deviceName == deviceName)?.deviceSprite;
        }
    }
}