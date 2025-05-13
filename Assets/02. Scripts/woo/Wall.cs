using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [Header("Alpha Settings")]
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private float targetAlpha = 0.1f;

    [Header("Asset")]
    [SerializeField] private SpriteRenderer top;

    // controll
    private Coroutine curCo;
    private float originalAlpha;

    private void Start()
    {
        originalAlpha = top.color.a;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (curCo != null) StopCoroutine(curCo);
            curCo = StartCoroutine(FadeToAlpha(targetAlpha));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (curCo != null)
                StopCoroutine(curCo);

            curCo = StartCoroutine(FadeToAlpha(originalAlpha));
        }
    }

    // 기둥 윗부분 투명 활/비활성화 연출
    private IEnumerator FadeToAlpha(float target)
    {
        if (top == null) yield break;

        Color color = top.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            float newAlpha = Mathf.Lerp(startAlpha, target, t);
            color.a = newAlpha;
            top.color = color;
            yield return null;
        }

        // set
        color.a = target;
        top.color = color;
    }
}
