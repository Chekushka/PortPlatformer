using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Input
{
    public class GuideRowUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI actionNameText;
        [SerializeField] private Image iconImage;
    
        public InputActionReference Action { get; private set; }

        public void Setup(InputActionReference actionRef)
        {
            Action = actionRef;
            actionNameText.text = actionRef.action.name;
        }

        public void SetIcon(Sprite icon)
        {
            if (icon != null)
            {
                iconImage.sprite = icon;
                iconImage.enabled = true;
            }
            else
            {
                // Hide the image if no icon was found.
                iconImage.enabled = false;
            }
        }
    }
}