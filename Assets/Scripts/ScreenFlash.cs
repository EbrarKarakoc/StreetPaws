using System.Collections;
using UnityEngine;

// Tam ekran kısa parlama efekti (star toplama; ileride milestone flash da bunu kullanacak).
// Koşuyu durdurmaz.
public class ScreenFlash : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup group;

    [Header("Flash Settings")]
    public float flashDuration = 0.25f;
    public float peakAlpha = 0.6f;

    private void Awake()
    {
        if (group != null) group.alpha = 0f;
    }

    public void Flash()
    {
        if (group == null) return;
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float half = flashDuration * 0.5f;

        for (float t = 0f; t < half; t += Time.deltaTime)
        {
            group.alpha = Mathf.Lerp(0f, peakAlpha, t / half);
            yield return null;
        }
        for (float t = 0f; t < half; t += Time.deltaTime)
        {
            group.alpha = Mathf.Lerp(peakAlpha, 0f, t / half);
            yield return null;
        }
        group.alpha = 0f;
    }
}
