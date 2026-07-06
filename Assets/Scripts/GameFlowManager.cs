using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private PlayerMovement player;

    private enum State { Start, Playing, GameOver }
    private State state;

    private void Start()
    {
        state = State.Start;
        startPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    private void Update()
    {
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
        startPanel.SetActive(false);
        player.BeginGame();
    }

    public void ShowGameOver()
    {
        state = State.GameOver;
        gameOverPanel.SetActive(true);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}