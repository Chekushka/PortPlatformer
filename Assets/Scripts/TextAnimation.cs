using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextAnimation : MonoBehaviour
{
    [Header("Animation Type")] [SerializeField]
    private AnimationType animationType;

    [Header("Settings")] [SerializeField] private bool playOnStart = true;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private float heightMultiplier = 10f; // Множник висоти
    [SerializeField] private bool isLooping = true;

    private TMP_Text _textComponent;
    private Coroutine _animCoroutine;

    private void Start()
    {
        _textComponent = GetComponent<TMP_Text>();
        if (playOnStart)
            PlayAnimation("Welcome!");
    }

    public void PlayAnimation(string textToType = "")
    {
        if (_animCoroutine != null) StopCoroutine(_animCoroutine);
        switch (animationType)
        {
            case AnimationType.Wave:
                _animCoroutine = StartCoroutine(WaveAnimCoroutine(textToType));
                break;
            case AnimationType.Typing:
                _animCoroutine = StartCoroutine(TypingAnimCoroutine(textToType));
                break;
            case AnimationType.Vertices:
               
                break;
            default:
                break;
        }
    }

    private IEnumerator TypingAnimCoroutine(string textToType)
    {
        _textComponent.text = textToType;
        _textComponent.maxVisibleCharacters = 0;

        _textComponent.ForceMeshUpdate();

        int visibleCharacters = textToType.Length;
        int counter = 0;

        while (counter <= visibleCharacters)
        {
            _textComponent.maxVisibleCharacters += 1;
            counter++;
            yield return new WaitForSeconds(animationDuration / visibleCharacters);
        }

        _animCoroutine = null;
    }
    
    private IEnumerator VerticesAnimCoroutine(string textToType)
    {
       
        yield break; 
    }

    private IEnumerator WaveAnimCoroutine(string textToType)
    {
        _textComponent.text = textToType;
        _textComponent.ForceMeshUpdate();
        
        TMP_TextInfo textInfo = _textComponent.textInfo;
        
        // КЕШУВАННЯ: Копіюємо початкові позиції вершин
        Vector3[][] allOriginalVertices = new Vector3[textInfo.meshInfo.Length][];
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            allOriginalVertices[i] = (Vector3[])textInfo.meshInfo[i].vertices.Clone();
        }

        float elapsedTime = 0f;

        // Цикл працює або поки не вийде час, або нескінченно, якщо isLooping = true
        while (isLooping || elapsedTime < animationDuration)
        {
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int matIndex = charInfo.materialReferenceIndex;
                int vIndex = charInfo.vertexIndex;

                Vector3[] sourceVertices = allOriginalVertices[matIndex];
                Vector3[] destVertices = textInfo.meshInfo[matIndex].vertices;

                // Розрахунок прогресу для кривої (0...1)
                // Mathf.Repeat змушує значення повертатися до 0 після досягнення 1
                float normalizedTime = isLooping 
                    ? Mathf.Repeat(elapsedTime / animationDuration, 1f) 
                    : Mathf.Clamp01(elapsedTime / animationDuration);

                // Формула руху: Sin (хвиля) * Multiplier (висота) * Curve (вплив у часі)
                float wave = Mathf.Sin(elapsedTime * Mathf.PI * 2f + i);
                float curveValue = animationCurve.Evaluate(normalizedTime);
                float offset = wave * heightMultiplier * curveValue;

                for (int j = 0; j < 4; j++)
                {
                    destVertices[vIndex + j] = sourceVertices[vIndex + j] + Vector3.up * offset;
                }
            }

            _textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        _animCoroutine = null;
    }

    public void PlayTestAnimation()
    {
        PlayAnimation("Welcome!");
    }

    public void StopAnimation()
    {
        if (_animCoroutine != null)
        {
            StopCoroutine(_animCoroutine);
            _animCoroutine = null;
        }

        _textComponent.maxVisibleCharacters = _textComponent.text.Length;
    }
}


public enum AnimationType
{
    None,
    Vertices,
    Typing,
    Wave
}