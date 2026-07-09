using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Score")]
    public int score = 0;

    [Header("UI References")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Game Flow")]
    [SerializeField] private GameFlowManager gameFlowManager;

    [Header("Forward Movement")]
    [SerializeField] private float forwardSpeed = 2f;
    [SerializeField] private float speedIncreaseAmount = 0.5f;
    [SerializeField] private float speedIncreaseDistance = 10f; // meters traveled between each speed bump

    [Header("Swerve Settings")]
    [SerializeField] private float swerveSensitivity = 0.02f; // how much drag delta translates into world units
    [SerializeField] private float smoothTime = 0.08f;        // lower = snappier, higher = floatier
    [SerializeField] private float minX = -4.5f;
    [SerializeField] private float maxX = 4.5f;

    private float targetX;
    private float currentXVelocity; // used internally by SmoothDamp
    private Vector3 lastInputPosition;
    private bool isDragging;
    private bool isGameOver;
    private bool hasStarted;
    private float distanceTraveled;
    private float nextSpeedIncreaseDistance;

    private void Start()
    {
        targetX = transform.position.x;
        nextSpeedIncreaseDistance = speedIncreaseDistance;

        UpdateScoreUI();
    }

    public void BeginGame()
    {
        hasStarted = true;
    }

    private void Update()
    {
        // Sit still until the start screen is dismissed, and freeze completely once the game is over.
        if (!hasStarted || isGameOver)
        {
            return;
        }

        // Constant forward movement on Z, always on regardless of input.
        float moveDistance = forwardSpeed * Time.deltaTime;
        transform.Translate(Vector3.forward * moveDistance, Space.World);

        // Ramp difficulty up over distance instead of time, so swerve skill controls the pace.
        distanceTraveled += moveDistance;
        if (distanceTraveled >= nextSpeedIncreaseDistance)
        {
            forwardSpeed += speedIncreaseAmount;
            nextSpeedIncreaseDistance += speedIncreaseDistance;
        }

        HandleSwerveInput();

        // Smoothly move current X towards the target X for a responsive but non-jittery feel.
        float smoothedX = Mathf.SmoothDamp(transform.position.x, targetX, ref currentXVelocity, smoothTime);
        transform.position = new Vector3(smoothedX, transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isGameOver)
        {
            return;
        }

        if (other.CompareTag("Obstacle"))
        {
            TriggerGameOver();
        }
        else if (other.CompareTag("Coin"))
        {
            CollectCoin(other.gameObject);
        }
        else if (other.CompareTag("Star"))
        {
            CollectStar(other.gameObject);
        }
        else if (other.CompareTag("Bomb"))
        {
            HitBomb(other.gameObject);
        }
    }

    private void CollectCoin(GameObject coin)
    {
        Destroy(coin);

        score++;
        Debug.Log("Score: " + score);
        UpdateScoreUI();
    }

    private void CollectStar(GameObject star)
    {
        Destroy(star);

        score += 5;
        Debug.Log("Score: " + score);
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Paws:" + score;
        }
    }

    private void HitBomb(GameObject bomb)
    {
        // Try to play the bomb's own explosion animation before freezing the player.
        Animator bombAnimator = bomb.GetComponent<Animator>();
        if (bombAnimator != null)
        {
            bombAnimator.SetTrigger("Explode");
        }
        else
        {
            Debug.Log("BOOM! Game Over");
        }

        TriggerGameOver();
    }

    private void TriggerGameOver()
    {
        isGameOver = true;

        // Lock the swerve target to the current position so SmoothDamp can't cause any residual drift.
        targetX = transform.position.x;
        currentXVelocity = 0f;
        isDragging = false;

        if (gameFlowManager != null)
        {
            gameFlowManager.ShowGameOver();
        }

        Debug.Log("Game Over! You hit an obstacle.");
    }

    private void HandleSwerveInput()
    {
        // --- Mouse (Editor / WebGL desktop) ---
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastInputPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
        else if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 currentInputPosition = Input.mousePosition;
            float deltaX = currentInputPosition.x - lastInputPosition.x;
            lastInputPosition = currentInputPosition;

            ApplySwerveDelta(deltaX);
        }

        // --- Touch (Mobile) ---
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    isDragging = true;
                    break;
                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        ApplySwerveDelta(touch.deltaPosition.x);
                    }
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;
                    break;
            }
        }
    }

    private void ApplySwerveDelta(float screenDeltaX)
    {
        // Convert screen-space pixel delta into world-space X movement, then clamp to keep the cat on the road.
        targetX += screenDeltaX * swerveSensitivity;
        targetX = Mathf.Clamp(targetX, minX, maxX);
    }
}
