// Attach this script to each of your UI tab Buttons.

using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Make sure you have DOTween imported

namespace Tabs
{
    [RequireComponent(typeof(Image))]
    public class TabButton : MonoBehaviour, IPointerClickHandler
    {
        [Header("Animation Settings")]
        [SerializeField] private float hoverOffsetY = 15f;
        [SerializeField] private float animationDuration = 0.25f;
        [SerializeField] private Ease easeType = Ease.OutCubic;

        private TabGroupManager m_TabGroup;
        private RectTransform m_RectTransform;
        private Image m_Image;
        private Vector2 m_InitialPosition;

        private void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_Image = GetComponent<Image>();
            m_InitialPosition = m_RectTransform.anchoredPosition;

            // Find the manager in the parent objects.
            m_TabGroup = GetComponentInParent<TabGroupManager>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            m_TabGroup.OnTabSelected(this);
        }

        public void Select()
        {
            m_RectTransform.DOKill();
            m_RectTransform.DOAnchorPosY(m_InitialPosition.y + hoverOffsetY, animationDuration)
                .SetEase(easeType)
                .SetUpdate(true); // SetUpdate(true) ensures it works even if Time.timeScale is 0.

            m_Image.sprite = m_TabGroup.GetActiveSprite();
            m_Image.color = Color.white;
        }

        public void Deselect()
        {
            m_RectTransform.DOKill();
            m_RectTransform.DOAnchorPosY(m_InitialPosition.y, animationDuration)
                .SetEase(easeType)
                .SetUpdate(true);
            
            m_Image.sprite = m_TabGroup.GetDefaultSprite();
            m_Image.color = Color.grey;
        }
    }
}