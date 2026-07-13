using TMPro;
using UnityEngine;

// Toplanan pati altınlarını sayar ve sağ üstteki bara yazar.
// GameFlow objesine eklenir; Collectible'lar singleton üzerinden AddCoins çağırır.
// (İleride: yıldız +5, bildirimler ve diğer bar öğeleri de buradan yönetilebilir.)
public class ScoreManager : MonoBehaviour
{
    // Prefab'ler sahne referansı tutamadığı için tek örneğe buradan ulaşılır
    public static ScoreManager Instance { get; private set; }

    [Header("Referanslar")]
    // Sağ üst bardaki sayaç yazısı (TopBar > CoinText)
    public TMP_Text coinText;

    [Header("Görünüm")]
    // Sayının önüne yazılacak etiket (Inspector'dan değiştirilebilir)
    public string prefix = "Paws: ";

    [Header("Durum")]
    // Bu koşuda toplanan altın — Inspector'dan izlenebilsin diye public
    public int coins;

    private void Awake()
    {
        Instance = this;
        UpdateUI(); // Oyun başında "Paws: 0" yazsın
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null; // Sahne yeniden yüklenince temiz başlangıç
        }
    }

    // Collectible çağırır. Miktar parametreli: ileride yıldız için +5 hazır.
    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinText != null) // Atanmamışsa hata fırlatma, sessizce geç
        {
            coinText.text = prefix + coins;
        }
    }
}
