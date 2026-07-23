using UnityEngine;

// Basit "kalp atışı" nabız animasyonu: obje AKTİFKEN sürekli hafifçe büyüyüp küçülür.
// 2× çarpan rozetine takılır; rozet SetActive(true) olunca otomatik atmaya başlar,
// SetActive(false) olunca durur. Ekstra ayar/kurulum gerekmez.
public class UIPulse : MonoBehaviour
{
    [Header("Pulse Settings")]
    // Nabzın en küçük ve en büyük ölçeği (1 = orijinal boyut)
    public float minScale = 1f;
    public float maxScale = 1.18f;

    // Atış hızı (büyük = daha hızlı "tık tık")
    public float speed = 7f;

    private void OnEnable()
    {
        transform.localScale = Vector3.one * minScale; // her açılışta baştan başla
    }

    private void Update()
    {
        // Sin ile 0..1 arası gidip gelen değer → nabız gibi büyüyüp küçülür
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        float s = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = new Vector3(s, s, 1f);
    }
}
