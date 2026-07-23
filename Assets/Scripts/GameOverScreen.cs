using System.Collections;
using TMPro;
using UnityEngine;

// Fiyakalı Game Over ekranı: karartma yumuşakça belirir (fade-in),
// mesaj kutusu (kart) zıplayarak gelir (ease-out-back, hedefi aşıp geri oturur),
// "yeniden başla" yazısı nabız gibi atar. Hepsi kodla — ekstra paket gerekmez.
// GameOverPanel objesine eklenir; GameStateManager, Show() ile tetikler.
public class GameOverScreen : MonoBehaviour
{
    [Header("References")]
    // Panelin kendi CanvasGroup'u (karartma + tüm içeriğin fade'i)
    public CanvasGroup panelGroup;

    // Mesaj kutusunun (Card) RectTransform'u — başlık ve yazılar bunun içinde,
    // zıplama animasyonu kartın tamamına uygulanır
    public RectTransform cardTransform;

    // "dokun" yazısının CanvasGroup'u (nabız efekti için)
    public CanvasGroup restartGroup;

    [Header("Score / Record")]
    // Kartta gösterilen final skor ("X Paws")
    public TMP_Text finalScoreText;

    // Kartta gösterilen rekor ("Best: X")
    public TMP_Text cardBestText;

    // Opsiyonel: yalnızca yeni rekor kırılınca aktifleşen rozet/yazı (atanması ŞART değil)
    public GameObject newRecordBadge;

    [Header("Animation Settings")]
    // Karartmanın görünme süresi (saniye)
    public float fadeDuration = 0.35f;

    // Kartın zıplayarak gelme süresi (saniye)
    public float titlePopDuration = 0.5f;

    // Zıplamanın taşma miktarı (büyük değer = daha abartılı fiyaka)
    public float titleOvershoot = 1.7f;

    // "dokun" yazısının nabız hızı
    public float pulseSpeed = 3f;

    // GameStateManager çağırır: final skoru/rekoru yaz, paneli aç ve animasyonu başlat
    public void Show(int finalPaws, int best, bool isNewRecord)
    {
        gameObject.SetActive(true);

        // Kart yazıları — bir kez çalışır (kare-başı değil), allocation sorun değil
        if (finalScoreText != null)
        {
            finalScoreText.text = finalPaws + " Paws";
        }
        if (cardBestText != null)
        {
            cardBestText.text = "Best: " + best;
        }
        // Rozet opsiyonel: atanmışsa yalnızca yeni rekorda göster
        if (newRecordBadge != null)
        {
            newRecordBadge.SetActive(isNewRecord);
        }

        StopAllCoroutines(); // Güvenlik: üst üste çağrılırsa animasyon karışmasın
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        // Başlangıç durumları: her şey görünmez
        panelGroup.alpha = 0f;
        cardTransform.localScale = Vector3.zero;
        restartGroup.alpha = 0f;

        // 1) Karartma fade-in
        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            panelGroup.alpha = t / fadeDuration;
            yield return null;
        }
        panelGroup.alpha = 1f;

        // 2) Kart pop: 0'dan büyüyerek gelir, hedefi hafif aşar, geri oturur
        for (float t = 0f; t < titlePopDuration; t += Time.deltaTime)
        {
            cardTransform.localScale = Vector3.one * EaseOutBack(t / titlePopDuration);
            yield return null;
        }
        cardTransform.localScale = Vector3.one;

        // 3) "dokun" yazısı sonsuz nabız (panel kapanana kadar sürer)
        while (true)
        {
            restartGroup.alpha = 0.65f + 0.35f * Mathf.Sin(Time.time * pulseSpeed);
            yield return null;
        }
    }

    // Standart ease-out-back eğrisi: 0..1 girdi alır, sona doğru hafif taşma yapar
    private float EaseOutBack(float x)
    {
        float c1 = titleOvershoot;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }
}
