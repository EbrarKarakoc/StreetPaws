using System.Collections;
using UnityEngine;

// Tam ekran KISA parlama efekti (star toplama; ileride Faz 9 milestone flash da bunu kullanır).
// Tam ekranı kaplayan bir Image + CanvasGroup'a takılır. Flash() çağrılınca alpha hızla
// yükselip söner. Koşuyu DURDURMAZ (GDD kuralı) — sadece görsel bir darbe.
public class ScreenFlash : MonoBehaviour
{
    [Header("References")]
    // Parlayan tam ekran katmanının CanvasGroup'u (alpha'yı bu kontrol eder)
    public CanvasGroup group;

    [Header("Flash Settings")]
    // Parlamanın toplam süresi (saniye) — kısa olmalı
    public float flashDuration = 0.25f;

    // En parlak andaki alpha (1 = tam opak; 0.6 hoş bir darbe)
    public float peakAlpha = 0.6f;

    private void Awake()
    {
        if (group != null) group.alpha = 0f; // başta görünmez
    }

    // ScoreManager (star) çağırır: kısa bir parlama oynat
    public void Flash()
    {
        if (group == null) return;
        StopAllCoroutines(); // üst üste star'da animasyon karışmasın
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float half = flashDuration * 0.5f;

        // 1) Hızlı parla (0 → peak)
        for (float t = 0f; t < half; t += Time.deltaTime)
        {
            group.alpha = Mathf.Lerp(0f, peakAlpha, t / half);
            yield return null;
        }
        // 2) Sön (peak → 0)
        for (float t = 0f; t < half; t += Time.deltaTime)
        {
            group.alpha = Mathf.Lerp(peakAlpha, 0f, t / half);
            yield return null;
        }
        group.alpha = 0f;
    }
}
