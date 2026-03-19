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
    [SerializeField] private bool isLooping = true;
    [SerializeField] private AnimationCurve animationCurve;

    [Header("Wave Animation Settings")] [SerializeField]
    private float heightMultiplier = 10f;

    [Header("Scale Animation Settings")] [SerializeField]
    private float scaleMultiplier = 1.2f;

    [Header("Gradient Animation Settings")] [SerializeField]
    private float colorSpread = 0.1f;

    [SerializeField] private Gradient animationGradient;

    private TMP_Text _textComponent;
    private Dictionary<AnimationType, Coroutine> _animCoroutines;

    private void Start()
    {
        _textComponent = GetComponent<TMP_Text>();
        _animCoroutines = new Dictionary<AnimationType, Coroutine>();
        if (playOnStart)
            PlayAnimation("Welcome!");
    }

    public void PlayAnimation(string textToType = "")
    {
        if (_animCoroutines.ContainsKey(animationType)) return;
        Coroutine anim;
        switch (animationType)
        {
            case AnimationType.Wave:
                anim = StartCoroutine(WaveAnimCoroutine(textToType));
                break;
            case AnimationType.Typing:
                anim = StartCoroutine(TypingAnimCoroutine(textToType));
                break;
            case AnimationType.Scale:
                anim = StartCoroutine(ScaleAnimCoroutine(textToType));
                break;
            case AnimationType.Gradient:
                anim = StartCoroutine(GradientAnimCoroutine(textToType));
                break;
            case AnimationType.None:
            default:
                anim = StartCoroutine(WaveAnimCoroutine(textToType));
                break;
        }
            
        _animCoroutines.Add(animationType, anim);
    }

    private IEnumerator TypingAnimCoroutine(string textToType)
    {
        _textComponent.text = textToType;

        while (isLooping)
        {
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
        }
    }

    private IEnumerator ScaleAnimCoroutine(string textToType)
    {
        _textComponent.text = textToType;
        _textComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = _textComponent.textInfo;
        Vector3[][] originalVertices = new Vector3[textInfo.meshInfo.Length][];
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
            originalVertices[i] = (Vector3[])textInfo.meshInfo[i].vertices.Clone();

        float elapsedTime = 0f;

        while (isLooping || elapsedTime < animationDuration)
        {
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var character = textInfo.characterInfo[i];
                if (!character.isVisible) continue;

                var matIndex = character.materialReferenceIndex;
                var vIndex = character.vertexIndex;

                Vector3[] sourceVertices = originalVertices[matIndex];
                Vector3[] destVertices = textInfo.meshInfo[matIndex].vertices;

                float normalizedTime = isLooping
                    ? Mathf.Repeat(elapsedTime / animationDuration, 1f)
                    : Mathf.Clamp01(elapsedTime / animationDuration);

                float curveValue = animationCurve.Evaluate(normalizedTime);
                float offset = curveValue * scaleMultiplier;

                Vector3 charCenter = (sourceVertices[vIndex] + sourceVertices[vIndex + 2]) / 2f;

                for (int j = 0; j < 4; j++)
                {
                    Vector3 direction = sourceVertices[vIndex + j] - charCenter;
                    destVertices[vIndex + j] = charCenter + direction * (1 + offset);
                }
            }

            _textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
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
    }

    private IEnumerator GradientAnimCoroutine(string textToType)
    {
        _textComponent.text = textToType;
        _textComponent.ForceMeshUpdate();
        TMP_TextInfo textInfo = _textComponent.textInfo;

        float elapsedTime = 0f;
        while (isLooping || elapsedTime < animationDuration)
        {
            float t = isLooping
                ? Mathf.Repeat(elapsedTime / animationDuration, 1f)
                : Mathf.Clamp01(elapsedTime / animationDuration);

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int matIndex = charInfo.materialReferenceIndex;
                int vIndex = charInfo.vertexIndex;
                Color32[] vertexColors = textInfo.meshInfo[matIndex].colors32;

                float timePhase = Mathf.Repeat(t + (i * colorSpread), 1f);
                Color targetColor = animationGradient.Evaluate(timePhase);

                for (int j = 0; j < 4; j++)
                {
                    vertexColors[vIndex + j] = targetColor;
                }
            }

            _textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
    }

    public void PlayTestAnimation()
    {
        PlayAnimation("Welcome!");
    }

    public void StopAnimation()
    {
        foreach (var coroutine in _animCoroutines)
            StopCoroutine(coroutine.Value);

        _animCoroutines.Clear();
        _textComponent.maxVisibleCharacters = _textComponent.text.Length;
    }
}


public enum AnimationType
{
    None,
    Scale,
    Typing,
    Wave,
    Gradient
}