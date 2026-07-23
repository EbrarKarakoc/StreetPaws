using UnityEngine;

// Star (nadir pickup): kedi değince 2× skor çarpanını başlatır ve KENDİNİ gizler.
// Para VERMEZ (GDD). Havuza asıl iadeyi ChunkController.ClearSpawns yapar — tıpkı Collectible
// gibi tek-sahiplik deseni (topla → SetActive(false), aliasing önlenir).
// Star prefab'inin collider'ı "Is Trigger" işaretli olmalı.
public class StarPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Kediyi CatController bileşeninden tanı (tag'e bağlı değil)
        if (other.GetComponentInParent<CatController>() == null)
        {
            return; // kedi değilse umursama
        }

        // 2× çarpanı başlat/yenile + ekran parlaması + rozet (hepsi ScoreManager'da)
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ActivateStar();
        }
        else
        {
            Debug.LogWarning("StarPickup: Sahnede ScoreManager yok!");
        }

        gameObject.SetActive(false); // sadece gizlen; iadeyi chunk yapar
    }
}
