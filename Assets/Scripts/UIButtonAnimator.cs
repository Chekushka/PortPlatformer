using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening; // Make sure you have imported DOTween

[RequireComponent(typeof(Button))]
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
        // Store the button's original scale to return to it later.
        m_InitialScale = transform.localScale;
    }

    // Called when the mouse pointer enters the button's bounds.
    public void OnPointerEnter(PointerEventData eventData)
    {
        AnimateScaleUp();
    }

    // Called when the mouse pointer exits the button's bounds.
    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateScaleDown();
    }

    // Called when the button is selected via the EventSystem (e.g., gamepad).
    public void OnSelect(BaseEventData eventData)
    {
        AnimateScaleUp();
    }

    // Called when the button is deselected.
    public void OnDeselect(BaseEventData eventData)
    {
        AnimateScaleDown();
    }

    private void AnimateScaleUp()
    {
        // Kill any existing tween to prevent conflicts.
        if (m_CurrentTween != null && m_CurrentTween.IsActive())
        {
            m_CurrentTween.Kill();
        }
        // Animate the scale to the target value.
        m_CurrentTween = transform.DOScale(m_InitialScale * targetScale, animationDuration)
            .SetEase(easeType)
            .SetUpdate(true); // SetUpdate(true) makes it work even if Time.timeScale is 0 (e.g., in a pause menu).
    }

    private void AnimateScaleDown()
    {
        if (m_CurrentTween != null && m_CurrentTween.IsActive())
        {
            m_CurrentTween.Kill();
        }
        // Animate the scale back to its initial value.
        m_CurrentTween = transform.DOScale(m_InitialScale, animationDuration)
            .SetEase(easeType)
            .SetUpdate(true);
    }

    // It's good practice to kill tweens when the object is destroyed.
    private void OnDestroy()
    {
        if (m_CurrentTween != null)
        {
            m_CurrentTween.Kill();
        }
    }
}