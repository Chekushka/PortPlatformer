using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public static bool IsOpened { get; private set; }
    [SerializeField] private GameObject window;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button closeButton;
    [FormerlySerializedAs("backAction")]
    [Tooltip("An action in Player Input that stands for UI/Cancel.")]
    [SerializeField] private InputActionReference cancelAction;
    [Tooltip("The very first button that should be selected when this menu opens.")]
    [SerializeField] private GameObject firstSelectedOnOpen;
    [Tooltip("The very first button in pause menu.")]
    [SerializeField] private GameObject pauseMenuFistButton;

    private void Start()
    {
        window.SetActive(false);
        SubscribeButtons();
    }

    private void SubscribeButtons()
    {
        settingsButton.onClick.AddListener(OpenMenu);
        closeButton.onClick.AddListener(CloseMenu);
        cancelAction.action.performed += TryDisableWindow;
    }
    
    private void OnDisable()
    {
        settingsButton.onClick.RemoveListener(OpenMenu);
        closeButton.onClick.RemoveListener(CloseMenu);
        cancelAction.action.performed -= TryDisableWindow;
    }

    private void TryDisableWindow(InputAction.CallbackContext context)
    {
        Debug.Log("Settings event");
        if(!IsOpened) return;
        CloseMenu();
    }

    private void OpenMenu()
    {
        window.SetActive(true);
        IsOpened = true;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedOnOpen);
    }
    
    private void CloseMenu()
    {
        window.SetActive(false);
        IsOpened = false;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseMenuFistButton);
    }
}