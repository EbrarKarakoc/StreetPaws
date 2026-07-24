using System.Collections;
using TMPro;
using UnityEngine;

// Game Over ekranı: karartma fade-in, kart zıplayarak gelir, "yeniden başla" nabız gibi atar.
public class GameOverScreen : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup panelGroup;
    public RectTransform cardTransform;
    public CanvasGroup restartGroup;

    [Header("Score / Record")]
    public TMP_Text finalScoreText;
    public TMP_Text cardBestText;
    public GameObject newRecordBadge;

    [Header("Animation Settings")]
    public float fadeDuration = 0.35f;
    public float titlePopDuration = 0.5f;
    public float titleOvershoot = 1.7f;
    public float pulseSpeed = 3f;

    public void Show(int finalPaws, int best, bool isNewRecord)
    {
        gameObject.SetActive(true);

        if (finalScoreText != null)
        {
            finalScoreText.text = finalPaws + " Paws";
        }
        if (cardBestText != null)
        {
            cardBestText.text = "Best: " + best;
        }
        if (newRecordBadge != null)
        {
            newRecordBadge.SetActive(isNewRecord);
        }

        StopAllCoroutines();
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        panelGroup.alpha = 0f;
        cardTransform.localScale = Vector3.zero;
        restartGroup.alpha = 0f;

        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            panelGroup.alpha = t / fadeDuration;
            yield return null;
        }
        panelGroup.alpha = 1f;

        for (float t = 0f; t < titlePopDuration; t += Time.deltaTime)
        {
            cardTransform.localScale = Vector3.one * EaseOutBack(t / titlePopDuration);
            yield return null;
        }
        cardTransform.localScale = Vector3.one;

        while (true)
        {
            restartGroup.alpha = 0.65f + 0.35f * Mathf.Sin(Time.time * pulseSpeed);
            yield return null;
        }
    }

    private float EaseOutBack(float x)
    {
        float c1 = titleOvershoot;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }
}
