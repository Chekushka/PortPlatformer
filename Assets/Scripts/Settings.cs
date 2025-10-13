using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject window;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private InputActionReference backAction;
    private bool m_SettingsEnabled;

    private void Start()
    {
        window.SetActive(false);
        SubscribeButtons();
    }

    private void SubscribeButtons()
    {
        settingsButton.onClick.AddListener(OnSettingsEnabled);
        closeButton.onClick.AddListener(OnSettingsDisabled);
        backAction.action.performed += TryDisableWindow;
    }
    
    private void OnDisable()
    {
        settingsButton.onClick.RemoveListener(OnSettingsEnabled);
        closeButton.onClick.RemoveListener(OnSettingsDisabled);
        backAction.action.performed -= TryDisableWindow;
    }

    private void TryDisableWindow(InputAction.CallbackContext context)
    {
        if(!m_SettingsEnabled) return;
        OnSettingsDisabled();
    }

    private void OnSettingsEnabled()
    {
        window.SetActive(true);
        m_SettingsEnabled = true;
    }
    
    private void OnSettingsDisabled()
    {
        window.SetActive(false);
        m_SettingsEnabled = false;
    }
}