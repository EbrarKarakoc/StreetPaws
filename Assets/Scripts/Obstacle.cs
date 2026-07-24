using UnityEngine;

// Kedi çarparsa oyunu bitirir. Collider "Is Trigger" işaretli olmalı.
public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<CatController>() == null)
        {
            return;
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.GameOver();
        }
        else
        {
            Debug.LogWarning("Obstacle: Sahnede GameStateManager yok! 'GameFlow' objesini kurmayı unutma.");
        }
    }
}
