using UnityEngine;

// Toplanabilir obje (pati altını vb.): kedi değince skoru artırır ve KENDİNİ havuza iade eder.
// Collectible prefab'inin collider'ı "Is Trigger" işaretli olmalı.
public class Collectible : MonoBehaviour
{
    [Header("Değer")]
    // Bu obje toplanınca skora kaç puan eklenir (coin=1; ileride yıldız=5)
    public int value = 1;

    // Bu objeyi veren havuz — spawn sırasında ChunkController tarafından atanır,
    // Inspector'dan elle atanmaz (o yüzden gizli)
    [HideInInspector] public ObjectPool pool;

    private void OnTriggerEnter(Collider other)
    {
        // Tag'e bağımlı kalmamak için kediyi CatController bileşeninden tanıyoruz
        if (other.GetComponentInParent<CatController>() == null)
        {
            return; // Kedi değilse umursama
        }

        // Skoru artır (Debug.Log yok artık — Console'u şişirmesin)
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddCoins(value);
        }
        else
        {
            Debug.LogWarning("Collectible: Sahnede ScoreManager yok! GameFlow objesine eklemeyi unutma.");
        }

        if (pool != null)
        {
            pool.Return(gameObject); // Destroy yok — havuza geri dön, tekrar kullanılacak
        }
        else
        {
            gameObject.SetActive(false); // Havuz atanmamışsa en azından görünmez ol
        }
    }
}
