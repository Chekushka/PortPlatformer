using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Input
{
    public class DeviceDisplayManager : MonoBehaviour
    {
        [Header("Core References")]
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private InputIconDatabase iconDatabase;

        [Header("Settings Menu UI")]
        [Tooltip("The Image in your settings menu that shows the active device.")]
        [SerializeField] private Image settingsDeviceImage;

        [Header("Notification Banner UI")]
        [Tooltip("The RectTransform of the banner that slides in.")]
        [SerializeField] private RectTransform deviceBannerRect;
        [Tooltip("The Image on the banner that shows the device icon.")]
        [SerializeField] private Image bannerDeviceIconImage;
    
        [Header("Banner Animation")]
        [SerializeField] private float slideInX = 250f;
        [SerializeField] private float slideOutX = -400f;
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private float displayDuration = 2.0f;
        [SerializeField] private Ease slideEase = Ease.OutCubic;
        
        [Header("Startup Behavior")]
        [Tooltip("If true, the banner will appear when the scene loads. If false, it will only appear on subsequent device changes.")]
        [SerializeField] private bool showBannerOnStartup = false;
        [Tooltip("The delay in seconds before the startup banner appears. Prevents it from playing during scene loads.")]
        [SerializeField] private float startupDelay = 1.0f;

        private bool m_IsInitialCheck = true;
        private Tween m_BannerTween;

        private void OnEnable()
        {
            if (playerInput == null) return;
            playerInput.onControlsChanged += OnDeviceChanged;
            
            UpdateSettingsImage(playerInput.currentControlScheme);
        }

        private void OnDisable()
        {
            if (playerInput == null) return;
            playerInput.onControlsChanged -= OnDeviceChanged;
        }

        private void OnDeviceChanged(PlayerInput input)
        {
            string deviceName = input.currentControlScheme;
            UpdateSettingsImage(deviceName);
            
            if (m_IsInitialCheck)
            {
                m_IsInitialCheck = false;
                if (showBannerOnStartup)
                {
                    StartCoroutine(ShowBannerWithDelay(deviceName, startupDelay));
                }
            }
            else
            {
                ShowDeviceBanner(deviceName);
            }
        }
        
        private IEnumerator ShowBannerWithDelay(string deviceName, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            ShowDeviceBanner(deviceName);
        }

        private void UpdateSettingsImage(string deviceName)
        {
            if (settingsDeviceImage == null || string.IsNullOrEmpty(deviceName)) return;

            Sprite deviceSprite = iconDatabase.FindDeviceSprite(deviceName);
            if (deviceSprite != null)
            {
                settingsDeviceImage.sprite = deviceSprite;
                settingsDeviceImage.enabled = true;
            }
            else
            {
                settingsDeviceImage.enabled = false;
            }
        }

        private void ShowDeviceBanner(string deviceName)
        {
            if (deviceBannerRect == null || string.IsNullOrEmpty(deviceName)) return;
            
            Sprite deviceSprite = iconDatabase.FindDeviceSprite(deviceName);
            if (bannerDeviceIconImage != null)
            {
                bannerDeviceIconImage.sprite = deviceSprite;
                bannerDeviceIconImage.enabled = (deviceSprite != null);
            }
            
            if (m_BannerTween != null && m_BannerTween.IsActive())
            {
                m_BannerTween.Kill();
            }
            
            DOTween.Sequence()
                .Append(deviceBannerRect.DOAnchorPosX(slideInX, animationDuration).SetEase(slideEase))
                .AppendInterval(displayDuration)
                .Append(deviceBannerRect.DOAnchorPosX(slideOutX, animationDuration).SetEase(slideEase))
                .SetUpdate(true);
        }
    }
}