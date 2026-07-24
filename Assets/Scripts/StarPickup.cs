using UnityEngine;

// Kedi değince 2× skor çarpanını başlatır, para vermez. İade Collectible gibi ChunkController'a ait.
public class StarPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<CatController>() == null)
        {
            return;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ActivateStar();
        }
        else
        {
            Debug.LogWarning("StarPickup: Sahnede ScoreManager yok!");
        }

        gameObject.SetActive(false);
    }
}
