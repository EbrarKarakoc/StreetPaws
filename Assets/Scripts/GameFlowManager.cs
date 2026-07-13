using UnityEngine;
using UnityEngine.SceneManagement;

// Oyunun akışını (Start ekranı -> Oynanış -> Game Over -> yeniden başlatma) yöneten script.
// PlayerMovement hareket/skor işine bakar, bu script ise sadece ekran/durum geçişlerine bakar.
public class GameFlowManager : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;    // Oyun başlamadan önce görünen panel
    [SerializeField] private GameObject gameOverPanel;  // Oyun bitince görünen panel
    [SerializeField] private PlayerMovement player;     // Oyuncuya "başla" komutu vermek için referans

    // Oyunun 3 olası durumu; enum kullanmak yanlış/geçersiz bir duruma düşmeyi derleme zamanında engeller
    private enum State { Start, Playing, GameOver }
    private State state;

    private void Start()
    {
        state = State.Start;
        startPanel.SetActive(true);   // Oyun ilk açıldığında Start ekranı görünsün
        gameOverPanel.SetActive(false); // Game Over paneli başta kapalı olsun
    }

    private void Update()
    {
        // Oynanış sırasında bu script'in yapacağı bir şey yok, input dinlemeye gerek yok
        if (state == State.Playing)
        {
            return;
        }

        if (AnyInputDetected())
        {
            if (state == State.Start)
            {
                BeginGame();
            }
            else if (state == State.GameOver)
            {
                RestartGame();
            }
        }
    }

    // Mouse tıklaması, herhangi bir tuş veya ekrana yeni dokunma olup olmadığını tek bir yerden kontrol eder
    private bool AnyInputDetected()
    {
        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            return true;
        }

        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
    }

    private void BeginGame()
    {
        state = State.Playing;
        startPanel.SetActive(false); // Start panelini kapat
        player.BeginGame();          // PlayerMovement'a hareket etmeye başlamasını söyle
    }

    // PlayerMovement, oyuncu öldüğünde (TriggerGameOver içinde) bunu çağırır
    public void ShowGameOver()
    {
        state = State.GameOver;
        gameOverPanel.SetActive(true);
    }

    // Skor, coin/star/bomba durumları dahil her şeyi elle sıfırlamak yerine sahneyi baştan yükleyerek sıfırlıyoruz
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
