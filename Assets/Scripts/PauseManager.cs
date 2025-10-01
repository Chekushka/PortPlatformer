using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI; // Using new Input System

public class PauseManager : Singleton<PauseManager>
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Image pauseMenuBackground;
    [SerializeField] private Color backgroundColor;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject firstSelectedButton;

    private List<IPausable> m_PausableObjects = new List<IPausable>();
    private bool m_IsPaused = false;
    private string m_OriginalActionMap;

    private void Start()
    {
        // Ensure the pause menu is hidden at the start
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            pauseMenuBackground.color = Color.clear;
        }

        playerInput.actions["EnablePause"].performed += SubscribeToggle;
        playerInput.actions["Pause"].performed += SubscribeToggle;
    }

    // A method for pausable objects to register themselves
    public void Register(IPausable pausable)
    {
        if (!m_PausableObjects.Contains(pausable))
        {
            m_PausableObjects.Add(pausable);
        }
    }

    // A method for pausable objects to unregister themselves
    public void Unregister(IPausable pausable)
    {
        if (m_PausableObjects.Contains(pausable))
        {
            m_PausableObjects.Remove(pausable);
        }
    }

    private void SubscribeToggle(InputAction.CallbackContext ctx)
    {
        TogglePause();
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
        // This is the hybrid approach: stop physics and standard animations
        Time.timeScale = 0f;

        // Manually pause all registered objects
        foreach (IPausable pausable in m_PausableObjects)
        {
            pausable.Pause();
        }

        // Show the pause menu
        if (pauseMenuUI != null)
        {
            pauseMenuUI.transform.localScale = Vector3.one * 0.1f;
            pauseMenuUI.SetActive(true);
            pauseMenuBackground.DOColor(backgroundColor, 0.3f);
            pauseMenuUI.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic);
        }
        
        // Clear previous selection to avoid issues
        EventSystem.current.SetSelectedGameObject(null); 
        // Set the new selection
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);

        // Switch Action Maps to a "UI" map
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
            pauseMenuBackground.DOColor(Color.clear, 0.3f);
            pauseMenuUI.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InFlash).OnComplete(() =>
            {
                pauseMenuUI.SetActive(false);
            });
        }
        
        // Important: Clear selection when closing the menu
        EventSystem.current.SetSelectedGameObject(null);

        // Switch back to the original Action Map
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
        }
    }
}