using System;
using System.Collections;
using Player;
using UnityEngine;

public class DamageVisualizer : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.5f;

    [Header("Other Settings")] [SerializeField]
    private float hitStopTime = 0.1f;

    private Renderer _targetRenderer;
    private PlayerHealth _playerHealth;
    private MaterialPropertyBlock _propertyBlock;
    private Coroutine _flashCoroutine;
    private Coroutine _hitStopCoroutine;
    
    private static readonly int ColorPropID = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        _targetRenderer = GetComponentInChildren<Renderer>();
        _propertyBlock = new MaterialPropertyBlock();
        _playerHealth = GetComponent<PlayerHealth>();
    }
    
    void OnEnable() => _playerHealth.OnDamageTaken += HandleDamageTaken;
    void OnDisable() => _playerHealth.OnDamageTaken -= HandleDamageTaken;

    private void HandleDamageTaken()
    {
        if(_flashCoroutine != null) StopCoroutine(_flashCoroutine);
        _flashCoroutine = StartCoroutine(FlashDamageCoroutine());
    }

    private IEnumerator FlashDamageCoroutine()
    {
        _targetRenderer.GetPropertyBlock(_propertyBlock);
        
        _propertyBlock.SetColor(ColorPropID, flashColor);
        _targetRenderer.SetPropertyBlock(_propertyBlock);
        
        if(_hitStopCoroutine != null) StopCoroutine(_hitStopCoroutine);
        _hitStopCoroutine = StartCoroutine(HitStopRoutine(hitStopTime));
        
        yield return new WaitForSeconds(flashDuration);
        
        _propertyBlock.Clear();
        _targetRenderer.SetPropertyBlock(_propertyBlock);
        
        _flashCoroutine = null;
    }
    
    private IEnumerator HitStopRoutine(float duration)
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        
        yield return new WaitForSecondsRealtime(duration);
    
        Time.timeScale = originalTimeScale;
        
        _hitStopCoroutine = null;
    }
}