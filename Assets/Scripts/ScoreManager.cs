using TMPro;
using UnityEngine;

// HUD sayaçları + rekor + STAR çarpanı. Üç değeri yönetir (GDD: skor ve para tamamen ayrı):
//   • Paws = kat edilen mesafe × çarpan (normal 1, star aktifken 2) → SKOR
//   • Coin = toplanan pati altını → yalnızca dükkanda harcanır, skoru/rekoru ETKİLEMEZ
//   • Rekor = en iyi Paws, PlayerPrefs'te saklanır ("rekor satın alınamaz")
// Star: 8 sn boyunca 2× Paws (yeni star süreyi yeniler, istiflenmez); ekran parlaması + 2× rozeti.
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    private const string BestKey = "StreetPaws_BestDistance";

    [Header("References")]
    // Paws (mesafe skoru) kaynağı — her kare buradan mesafe okunur
    public CatController cat;
    public TMP_Text pawsText;   // üst bar: Paws (skor)
    public TMP_Text coinText;   // üst bar: coin (altın)
    public TMP_Text bestText;   // üst bar: rekor

    [Header("Star (2x Multiplier)")]
    // Star aktifken skor çarpanı (GDD: 2×)
    public float starMultiplier = 2f;
    // Star etkisinin süresi (saniye)
    public float starDuration = 8f;
    // 2× rozeti — star aktifken görünür (UIPulse ile atar). Atanması şart değil.
    public GameObject multiplierBadge;
    // Star toplama parlaması. Atanması şart değil.
    public ScreenFlash screenFlash;

    [Header("Appearance")]
    public string pawsPrefix = "";
    public string coinPrefix = "";
    public string bestPrefix = "Best: ";

    [Header("State")]
    public int coins;
    public int bestDistance;

    // Paws BİRİKTİRİCİSİ: ham mesafe değil, mesafe×çarpan toplamı (star ile hızlı büyür)
    private float pawsAccumulated;
    // Bir önceki karedeki mesafe (delta hesaplamak için)
    private float lastDistance;
    // Star için kalan süre (>0 ise çarpan aktif)
    private float starTimeLeft;
    // Ekranda en son yazdığımız Paws (sadece değişince yaz → kare-başı allocation önler)
    private int lastShownPaws = -1;

    // Game Over kartına iletmek için güncel Paws
    public int CurrentPaws => Mathf.FloorToInt(pawsAccumulated);

    private void Awake()
    {
        Instance = this;
        bestDistance = PlayerPrefs.GetInt(BestKey, 0);
        if (multiplierBadge != null) multiplierBadge.SetActive(false); // rozet başta gizli
        UpdateCoinUI();
        UpdateBestUI();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        if (cat == null) return; // mesafe kaynağı yoksa geç

        // --- Bu karede kat edilen mesafe farkı ---
        float d = cat.DistanceTraveled;
        float delta = d - lastDistance;
        lastDistance = d;

        // --- Star sayacı: süre bitince rozeti gizle, çarpan 1'e döner ---
        if (starTimeLeft > 0f)
        {
            starTimeLeft -= Time.deltaTime;
            if (starTimeLeft <= 0f)
            {
                starTimeLeft = 0f;
                if (multiplierBadge != null) multiplierBadge.SetActive(false);
            }
        }

        // --- Paws'ı çarpanla biriktir (star aktifse ×2, değilse ×1) ---
        float multiplier = starTimeLeft > 0f ? starMultiplier : 1f;
        if (delta > 0f) pawsAccumulated += delta * multiplier;

        // --- Yazıyı sadece tam sayı değişince güncelle ---
        int paws = Mathf.FloorToInt(pawsAccumulated);
        if (paws != lastShownPaws)
        {
            lastShownPaws = paws;
            if (pawsText != null) pawsText.text = pawsPrefix + paws;
        }
    }

    // StarPickup çağırır: çarpanı başlat/yenile + parlama + rozet
    public void ActivateStar()
    {
        starTimeLeft = starDuration; // yenile (istiflenmez: hep 8 sn'ye döner, çarpan hep 2)
        if (multiplierBadge != null && !multiplierBadge.activeSelf) multiplierBadge.SetActive(true);
        if (screenFlash != null) screenFlash.Flash();
    }

    // Collectible çağırır. Sadece altını artırır — Paws'a/rekora DOKUNMAZ.
    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinUI();
    }

    // Oyun bitince GameStateManager çağırır: mevcut Paws rekoru geçtiyse kaydet.
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
