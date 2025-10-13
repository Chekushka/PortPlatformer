using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Animation Settings")]
    [Tooltip("The scale the button will tween to on hover/select.")]
    [SerializeField] private float targetScale = 1.1f;

    [Tooltip("The duration of the scale animation in seconds.")]
    [SerializeField] private float animationDuration = 0.2f;

    [Tooltip("The easing function for the animation.")]
    [SerializeField] private Ease easeType = Ease.OutBack;

    private Vector3 m_InitialScale;
    private Tween m_CurrentTween;

    private void Awake()
    {
        m_InitialScale = transform.localScale;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        AnimateScaleUp();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateScaleDown();
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        AnimateScaleUp();
    }
    
    public void OnDeselect(BaseEventData eventData)
    {
        AnimateScaleDown();
    }

    private void AnimateScaleUp()
    {
        if (m_CurrentTween != null && m_CurrentTween.IsActive())
        {
            m_CurrentTween.Kill();
        }
        
        m_CurrentTween = transform.DOScale(m_InitialScale * targetScale, animationDuration)
            .SetEase(easeType)
            .SetUpdate(true);
    }

    private void AnimateScaleDown()
    {
        if (m_CurrentTween != null && m_CurrentTween.IsActive())
        {
            m_CurrentTween.Kill();
        }
        
        m_CurrentTween = transform.DOScale(m_InitialScale, animationDuration)
            .SetEase(easeType)
            .SetUpdate(true);
    }
    
    private void OnDestroy()
    {
        if (m_CurrentTween != null)
        {
            m_CurrentTween.Kill();
        }
    }
}