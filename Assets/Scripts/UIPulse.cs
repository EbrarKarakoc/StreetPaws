using UnityEngine;

// Obje aktifken sürekli hafifçe büyüyüp küçülür (nabız efekti).
public class UIPulse : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float minScale = 1f;
    public float maxScale = 1.18f;
    public float speed = 7f;

    private void OnEnable()
    {
        transform.localScale = Vector3.one * minScale;
    }

    private void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        float s = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = new Vector3(s, s, 1f);
    }
}
