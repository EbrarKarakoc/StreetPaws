using UnityEngine;

// Engel: kedi çarparsa oyunu bitirir (GameStateManager üzerinden).
// Engel prefab'inin collider'ı "Is Trigger" işaretli olmalı.
public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Tag'e bağımlı kalmamak için kediyi CatController bileşeninden tanıyoruz
        // (GetComponentInParent: collider kedinin child objesindeyse de çalışsın)
        if (other.GetComponentInParent<CatController>() == null)
        {
            return; // Kedi değilse umursama
        }

        // Prefab olduğumuz için sahnedeki yöneticiye Inspector referansıyla değil
        // singleton üzerinden ulaşıyoruz
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
