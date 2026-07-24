using UnityEngine;

// Kedi hareketi: sabit ileri koşu + mesafeye bağlı hız rampası + swerve (X ekseni kayma).
// Kedide Rigidbody varsa "Is Kinematic" açık olmalı — hareketi bu script (transform) yönetiyor.
public class CatController : MonoBehaviour
{
    [Header("Forward Movement — Speed Ramp")]
    public float baseSpeed = 8f;
    public float maxSpeed = 16f;
    public float speedGainPer100m = 0.4f;

    [Header("Swerve (Lateral Slide)")]
    public float swerveSpeed = 30f;
    public float maxSwerveAmount = 1.2f;

    [Header("Road Bounds")]
    public float leftBound = -3f;
    public float rightBound = 3f;

    [Header("Runtime State (Live)")]
    [SerializeField] private float distanceTraveled;
    [SerializeField] private float currentSpeed;

    // ScoreManager Paws skorunu buradan okur (1 m = 1 Paws)
    public float DistanceTraveled => distanceTraveled;

    private bool isGameOver;

    public void Die()
    {
        isGameOver = true;
    }

    private void Update()
    {
        if (isGameOver)
        {
            return;
        }

        currentSpeed = Mathf.Min(baseSpeed + (distanceTraveled / 100f) * speedGainPer100m, maxSpeed);
        float forwardStep = currentSpeed * Time.deltaTime;

        float swerveAmount = 0f;
        if (Input.GetMouseButton(0))
        {
            swerveAmount = Mathf.Clamp(
                Input.GetAxis("Mouse X") * swerveSpeed * Time.deltaTime,
                -maxSwerveAmount,
                maxSwerveAmount);
        }

        Vector3 newPosition = transform.position + transform.forward * forwardStep;
        newPosition.x += swerveAmount;
        newPosition.x = Mathf.Clamp(newPosition.x, leftBound, rightBound);
        transform.position = newPosition;

        distanceTraveled += forwardStep;
    }
}
