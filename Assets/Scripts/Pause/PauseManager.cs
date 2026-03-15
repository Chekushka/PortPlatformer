using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Pause
{
    public class PauseManager : Singleton<PauseManager>
    {
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private PlayerInput playerInput;
        [FormerlySerializedAs("backAction")]
        [Tooltip("An action in Player Input that stands for UI/Cancel.")]
        [SerializeField] private InputActionReference cancelAction;
        [SerializeField] private GameObject firstSelectedButton;

        [Header("Blur")]
        [SerializeField] private Volume volume;
        [SerializeField] private float blurPower = 1;
            
        private List<IPausable> m_PausableObjects = new List<IPausable>();
        private bool m_IsPaused = false;
        private string m_OriginalActionMap;
        private float m_DefaultFocusDistance;

        private void Start()
        {
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(false);

                if (volume.profile.TryGet(out DepthOfField dof))
                {
                    m_DefaultFocusDistance = dof.focusDistance.value;
                }
            }

            playerInput.actions["EnablePause"].performed += SubscribeToggle;
            playerInput.actions["Pause"].performed += SubscribeToggle;
            cancelAction.action.performed += SubscribeCancelAction;
        }
        
        public void Register(IPausable pausable)
        {
            if (!m_PausableObjects.Contains(pausable))
            {
                m_PausableObjects.Add(pausable);
            }
        }
        
        public void Unregister(IPausable pausable)
        {
            if (m_PausableObjects.Contains(pausable))
            {
                m_PausableObjects.Remove(pausable);
            }
        }

        private void EnableBlur()
        {
            if (volume.profile.TryGet(out DepthOfField dof))
                dof.focusDistance.value = blurPower;
        }
        
        private void DisableBlur()
        {
            if (volume.profile.TryGet(out DepthOfField dof))
                dof.focusDistance.value = m_DefaultFocusDistance;
        }

        private void SubscribeToggle(InputAction.CallbackContext ctx)
        {
            TogglePause();
        }

        private void SubscribeCancelAction(InputAction.CallbackContext ctx)
        {
            Debug.Log("Pause event");
            Debug.Log(Settings.IsOpened);
            if (m_IsPaused && !Settings.IsOpened)
            {
                PerformResume();
                m_IsPaused = false;
            }
        }

        public void TogglePause()
        {
            m_IsPaused = !m_IsPaused;
            if (m_IsPaused)
            {
                PerformPause();
            }
            else
            {
                PerformResume();
            }
        }
    
        private void PerformPause()
        {
            Time.timeScale = 0f;
            
            foreach (IPausable pausable in m_PausableObjects)
            {
                pausable.Pause();
            }
            
            if (pauseMenuUI != null)
            {
                EnableBlur();
                pauseMenuUI.transform.localScale = Vector3.one * 0.1f;
                pauseMenuUI.SetActive(true);
                pauseMenuUI.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic);
            }
            
            EventSystem.current.SetSelectedGameObject(null); 
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
            
            if (playerInput != null)
            {
                m_OriginalActionMap = playerInput.currentActionMap.name;
                playerInput.SwitchCurrentActionMap("UI");
            }
        }

        private void PerformResume()
        {
            Time.timeScale = 1f;

            foreach (IPausable pausable in m_PausableObjects)
            {
                pausable.Resume();
            }

            if (pauseMenuUI != null)
            {
                DisableBlur();
                pauseMenuUI.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InFlash).OnComplete(() =>
                {
                    pauseMenuUI.SetActive(false);
                });
            }
            
            EventSystem.current.SetSelectedGameObject(null);
            
            if (playerInput != null)
            {
                playerInput.SwitchCurrentActionMap(m_OriginalActionMap);
            }
        }

        private void OnDisable()
        {
            if (playerInput != null)
            {
                playerInput.actions["EnablePause"].performed -= SubscribeToggle;
                playerInput.actions["Pause"].performed -= SubscribeToggle;
                cancelAction.action.performed -= SubscribeCancelAction;
            }
        }
    }
}