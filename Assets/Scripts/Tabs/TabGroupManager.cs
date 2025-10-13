// Attach this script to a parent GameObject that holds all your tabs and menus.

using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Tabs
{
    public class TabGroupManager : MonoBehaviour
    {
        [System.Serializable]
        public class TabbedMenu
        {
            public TabButton tabButton;
            public GameObject menuPanel;
        }

        [Tooltip("The list of tabs and their corresponding menu panels.")]
        [SerializeField] private List<TabbedMenu> tabbedMenus;

        [Tooltip("The index of the tab that should be selected when the game starts.")]
        [SerializeField] private int startingTabIndex = 0;
        
        [Header("Input Navigation")]
        [SerializeField] private InputActionReference previousTabAction;
        [SerializeField] private InputActionReference nextTabAction;
        private int m_CurrentTabIndex;
        
        [Header("Navigation Hint Images")]
        [SerializeField] private RectTransform previousTabHintRect;
        [SerializeField] private RectTransform nextTabHintRect;
        [SerializeField] private float flashScaleFactor = 1.2f;
        [SerializeField] private float flashDuration = 0.1f;
        [SerializeField] private Ease flashEaseType = Ease.OutQuad;

        [Header("Sprites")]
        [SerializeField] private Sprite activeTabSprite;
        [SerializeField] private Sprite defaultTabSprite;

        private void Start()
        {
            SelectTab(startingTabIndex);
            
            if (previousTabHintRect != null) previousTabHintRect.localScale = Vector3.one;
            if (nextTabHintRect != null) nextTabHintRect.localScale = Vector3.one;
        }
        
        private void OnEnable()
        {
            previousTabAction.action.performed += NavigateLeft;
            nextTabAction.action.performed += NavigateRight;
        }

        private void OnDisable()
        {
            previousTabAction.action.performed -= NavigateLeft;
            nextTabAction.action.performed -= NavigateRight;
        }
        
        public void OnTabSelected(TabButton selectedTab)
        {
            for (int i = 0; i < tabbedMenus.Count; i++)
            {
                if (tabbedMenus[i].tabButton == selectedTab)
                {
                    SelectTab(i);
                    break;
                }
            }
        }

        private void SelectTab(int index)
        {
            m_CurrentTabIndex = Mathf.Clamp(index, 0, tabbedMenus.Count - 1);

            for (int i = 0; i < tabbedMenus.Count; i++)
            {
                bool isSelected = (i == m_CurrentTabIndex);
            
                if (tabbedMenus[i].menuPanel != null)
                    tabbedMenus[i].menuPanel.SetActive(isSelected);

                if (isSelected)
                    tabbedMenus[i].tabButton.Select();
                else
                    tabbedMenus[i].tabButton.Deselect();
            }
        }
    
        private void NavigateLeft(InputAction.CallbackContext context)
        {
            m_CurrentTabIndex--;
            if (m_CurrentTabIndex < 0)
            {
                m_CurrentTabIndex = tabbedMenus.Count - 1;
            }
            SelectTab(m_CurrentTabIndex);
            
            AnimateHintImageScale(previousTabHintRect);
        }

        private void NavigateRight(InputAction.CallbackContext context)
        {
            m_CurrentTabIndex++;
            if (m_CurrentTabIndex >= tabbedMenus.Count)
            {
                m_CurrentTabIndex = 0;
            }
            SelectTab(m_CurrentTabIndex);
            
            AnimateHintImageScale(nextTabHintRect);
        }
        
        private void AnimateHintImageScale(RectTransform hintRect)
        {
            if (hintRect == null) return;
            hintRect.DOKill();
            hintRect.DOScale(Vector3.one * flashScaleFactor, flashDuration)
                .SetEase(flashEaseType)
                .SetLoops(2, LoopType.Yoyo) 
                .SetUpdate(true)           
                .OnComplete(() => {
                    hintRect.localScale = Vector3.one; 
                });
        }

        public Sprite GetActiveSprite() => activeTabSprite;
        public Sprite GetDefaultSprite() => defaultTabSprite;
    }
}