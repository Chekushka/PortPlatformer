using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(ScrollRect))]
public class ScrollToSelected : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("How fast the scroll view should move.")]
    [SerializeField] private float scrollDuration = 0.25f;
    [SerializeField] private Ease scrollEase = Ease.OutCubic;

    private ScrollRect m_ScrollRect;
    private RectTransform m_ContentPanel;
    private GameObject m_LastSelected;
    private Tween m_CurrentTween;

    private void Awake()
    {
        m_ScrollRect = GetComponent<ScrollRect>();
        m_ContentPanel = m_ScrollRect.content;
    }

    private void Update()
    {
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected == null || currentSelected == m_LastSelected)
        {
            return;
        }

        if (!currentSelected.transform.IsChildOf(m_ContentPanel))
        {
            m_LastSelected = currentSelected;
            return;
        }

        RectTransform selectedRect = currentSelected.GetComponent<RectTransform>();
        
        float contentHeight = m_ContentPanel.rect.height;
        float viewportHeight = m_ScrollRect.viewport.rect.height;
        
        float elementPos = Mathf.Abs(selectedRect.anchoredPosition.y);
        
        float elementTop = elementPos - selectedRect.rect.height * (1 - selectedRect.pivot.y);
        float elementBottom = elementPos + selectedRect.rect.height * selectedRect.pivot.y;
        
        float scrollPos = m_ScrollRect.verticalNormalizedPosition;
        float viewportTop = (1 - scrollPos) * (contentHeight - viewportHeight);
        float viewportBottom = viewportTop + viewportHeight;

        float targetPos = scrollPos;
        
        if (elementTop < viewportTop)
        {
            targetPos = 1 - (elementTop / (contentHeight - viewportHeight));
        }
        else if (elementBottom > viewportBottom)
        {
            targetPos = 1 - ((elementBottom - viewportHeight) / (contentHeight - viewportHeight));
        }
        else
        {
            m_LastSelected = currentSelected;
            return;
        }
        
        if (m_CurrentTween != null)
        {
            m_CurrentTween.Kill();
        }

        // Generic DOTween.To() to animate the value.
        m_CurrentTween = DOTween.To(
            () => m_ScrollRect.verticalNormalizedPosition, // Getter: What to read from
            x => m_ScrollRect.verticalNormalizedPosition = x,  // Setter: What to write to
            targetPos,
            scrollDuration
        ).SetEase(scrollEase).SetUpdate(true);

        m_LastSelected = currentSelected;
    }
}