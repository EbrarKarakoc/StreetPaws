using UnityEngine;
using UnityEngine.SceneManagement;

// Oyun bitti mi bitmedi mi'yi yöneten tek merci. Boş bir "GameFlow" objesine eklenir.
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("References")]
    public CatController cat;
    public GameOverScreen gameOverScreen;

    [Header("Restart")]
    public float restartDelay = 0.8f;

    private bool isGameOver;
    private float gameOverTime;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void GameOver()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;
        gameOverTime = Time.time;

        cat.Die();

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
        if (!isGameOver || Time.time - gameOverTime < restartDelay)
        {
            return;
        }

        if (AnyInputDetected())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private bool AnyInputDetected()
    {
        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            return true;
        }

        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
    }
}
