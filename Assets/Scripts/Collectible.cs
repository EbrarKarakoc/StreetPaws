using UnityEngine;

// Toplanabilir obje (pati altını vb.): kedi değince skoru artırır ve KENDİNİ gizler.
// Havuza iadeyi collectible YAPMAZ — o iş, bu objeyi spawn eden ChunkController'a
// aittir (ClearSpawns). Böylece toplanan obje, sahibi chunk geri dönüşene kadar havuza
// girmez; başka bir chunk onu Get edip "çalamaz" (çapraz-chunk aliasing önlenir).
// Collectible prefab'inin collider'ı "Is Trigger" işaretli olmalı.
public class Collectible : MonoBehaviour
{
    [Header("Value")]
    // Bu obje toplanınca skora kaç puan eklenir (coin=1; ileride yıldız=5)
    public int value = 1;

    private void OnTriggerEnter(Collider other)
    {
        // Tag'e bağımlı kalmamak için kediyi CatController bileşeninden tanıyoruz
        if (other.GetComponentInParent<CatController>() == null)
        {
            return; // Kedi değilse umursama
        }

        // Skoru artır
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddCoins(value);
        }
        else
        {
            Debug.LogWarning("Collectible: Sahnede ScoreManager yok! GameFlow objesine eklemeyi unutma.");
        }

        // Havuza iade ETME — sadece gizlen. Asıl iadeyi, bu objeyi spawn eden chunk
        // recycle olurken ChunkController.ClearSpawns yapar (tek iade otoritesi).
        // Böylece obje havuza erken dönüp başka chunk tarafından "çalınamaz".
        gameObject.SetActive(false);
    }
}
