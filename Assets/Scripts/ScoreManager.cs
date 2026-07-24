using TMPro;
using UnityEngine;

// HUD sayaçları + rekor + star çarpanı. Skor (Paws = mesafe × çarpan) ve para (coin) tamamen
// ayrı; coin harcamak skoru/rekoru etkilemez.
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    private const string BestKey = "StreetPaws_BestDistance";

    [Header("References")]
    public CatController cat;
    public TMP_Text pawsText;
    public TMP_Text coinText;
    public TMP_Text bestText;

    [Header("Star (2x Multiplier)")]
    public float starMultiplier = 2f;
    public float starDuration = 8f;
    public GameObject multiplierBadge;
    public ScreenFlash screenFlash;

    [Header("Appearance")]
    public string pawsPrefix = "";
    public string coinPrefix = "";
    public string bestPrefix = "Best: ";

    [Header("State")]
    public int coins;
    public int bestDistance;

    private float pawsAccumulated;
    private float lastDistance;
    private float starTimeLeft;
    private int lastShownPaws = -1;

    public int CurrentPaws => Mathf.FloorToInt(pawsAccumulated);

    private void Awake()
    {
        Instance = this;
        bestDistance = PlayerPrefs.GetInt(BestKey, 0);
        if (multiplierBadge != null) multiplierBadge.SetActive(false);
        UpdateCoinUI();
        UpdateBestUI();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        if (cat == null) return;

        float d = cat.DistanceTraveled;
        float delta = d - lastDistance;
        lastDistance = d;

        if (starTimeLeft > 0f)
        {
            starTimeLeft -= Time.deltaTime;
            if (starTimeLeft <= 0f)
            {
                starTimeLeft = 0f;
                if (multiplierBadge != null) multiplierBadge.SetActive(false);
            }
        }

        float multiplier = starTimeLeft > 0f ? starMultiplier : 1f;
        if (delta > 0f) pawsAccumulated += delta * multiplier;

        int paws = Mathf.FloorToInt(pawsAccumulated);
        if (paws != lastShownPaws)
        {
            lastShownPaws = paws;
            if (pawsText != null) pawsText.text = pawsPrefix + paws;
        }
    }

    public void ActivateStar()
    {
        starTimeLeft = starDuration;
        if (multiplierBadge != null && !multiplierBadge.activeSelf) multiplierBadge.SetActive(true);
        if (screenFlash != null) screenFlash.Flash();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinUI();
    }

    public bool CommitRecord()
    {
        int paws = CurrentPaws;
        if (paws > bestDistance)
        {
            bestDistance = paws;
            PlayerPrefs.SetInt(BestKey, bestDistance);
            PlayerPrefs.Save();
            UpdateBestUI();
            return true;
        }
        return false;
    }

    private void UpdateCoinUI()
    {
        if (coinText != null) coinText.text = coinPrefix + coins;
    }

    private void UpdateBestUI()
    {
        if (bestText != null) bestText.text = bestPrefix + bestDistance;
    }
}
