using UnityEngine;

// Kedi değince skoru artırır ve kendini gizler; havuza iadeyi spawn eden ChunkController yapar
// (çapraz-chunk aliasing önlenir). Collider "Is Trigger" işaretli olmalı.
public class Collectible : MonoBehaviour
{
    [Header("Value")]
    public int value = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<CatController>() == null)
        {
            return;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddCoins(value);
        }
        else
        {
            Debug.LogWarning("Collectible: Sahnede ScoreManager yok! GameFlow objesine eklemeyi unutma.");
        }

        gameObject.SetActive(false);
    }
}
