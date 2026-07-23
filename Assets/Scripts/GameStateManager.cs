using UnityEngine;
using UnityEngine.SceneManagement;

// Oyun durumunu yöneten TEK merci: koşu sürüyor mu, bitti mi?
// Engel çarpınca Obstacle bunun GameOver()'ını çağırır; kedi durur, ekran açılır,
// oyuncu herhangi bir giriş yapınca sahne baştan yüklenir (her şey sıfırlanır).
// Boş bir "GameFlow" objesine eklenir.
public class GameStateManager : MonoBehaviour
{
    // Sahnedeki tek örnek — Obstacle gibi prefab'ler sahnedeki objelere Inspector'dan
    // referans tutamadığı için buraya bu static üzerinden ulaşırlar (basit singleton)
    public static GameStateManager Instance { get; private set; }

    [Header("References")]
    // Çarpışmada durdurulacak kedi
    public CatController cat;

    // Açılacak Game Over ekranı (GameOverPanel'deki GameOverScreen bileşeni)
    public GameOverScreen gameOverScreen;

    [Header("Restart")]
    // Game Over sonrası girişlerin kaç saniye yok sayılacağı
    // (çarpma anında zaten ekranda olan parmak/tık, ekranı anında geçmesin)
    public float restartDelay = 0.8f;

    private bool isGameOver;   // Oyun bitti mi
    private float gameOverTime; // Game Over'ın gerçekleştiği an (bekleme süresi için)

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null; // Sahne yeniden yüklenince eski referans temizlensin
        }
    }

    // Obstacle çağırır: koşuyu bitir, ekranı göster
    public void GameOver()
    {
        if (isGameOver)
        {
            return; // Birden fazla engele aynı anda değince bir kez çalışsın
        }

        isGameOver = true;
        gameOverTime = Time.time;

        cat.Die();

        // Skoru sonlandır: mevcut mesafe rekoru geçtiyse kaydet, final değerleri karta ilet.
        // (Coin'e bakılmaz — rekor yalnızca mesafeyle kırılır.)
        int finalPaws = 0;
        int best = 0;
        bool isNewRecord = false;
        if (ScoreManager.Instance != null)
        {
            finalPaws = ScoreManager.Instance.CurrentPaws;
            isNewRecord = ScoreManager.Instance.CommitRecord();
            best = ScoreManager.Instance.bestDistance;
        }

        gameOverScreen.Show(finalPaws, best, isNewRecord);
    }

    private void Update()
    {
        // Sadece Game Over'dayken ve kısa bekleme geçtikten sonra giriş dinle
        if (!isGameOver || Time.time - gameOverTime < restartDelay)
        {
            return;
        }

        if (AnyInputDetected())
        {
            // Sahneyi baştan yükle: kedi, chunk'lar, havuzlar — her şey sıfırlanır
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // Mouse tıklaması, herhangi bir tuş veya ekrana yeni dokunuş var mı
    private bool AnyInputDetected()
    {
        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            return true;
        }

        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
    }
}
