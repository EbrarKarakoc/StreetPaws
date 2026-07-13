using UnityEngine;

// Kedinin hareketini yöneten script: otomatik ileri koşu + serbest yatay kayma (swerve).
// Tek sorumluluk: SADECE hareket. Skor, çarpışma, oyun akışı başka scriptlerin işi.
// NOT: Kedide Rigidbody varsa "Is Kinematic" AÇIK olmalı — hareketi fizik motoru değil
// bu script (transform) yönetiyor; kinematik kapalı olursa ikisi çakışır.
public class CatController : MonoBehaviour
{
    [Header("İleri Hareket")]
    // Kedinin saniyedeki ileri koşu hızı (birim/sn)
    public float forwardSpeed = 8f;

    [Header("Swerve (Yatay Kayma)")]
    // Mouse/parmak yatay hareketini kayma hızına çeviren katsayı
    public float swerveSpeed = 30f;

    // Tek karede uygulanabilecek en büyük yatay kayma miktarı (ani sıçramayı önler)
    public float maxSwerveAmount = 1.2f;

    [Header("Yol Sınırları")]
    // Kedinin gidebileceği en soldaki X konumu
    public float leftBound = -3f;

    // Kedinin gidebileceği en sağdaki X konumu
    public float rightBound = 3f;

    private bool isGameOver; // Çarpışma sonrası hareketi kilitler

    // GameStateManager çağırır: kediyi olduğu yerde durdurur
    public void Die()
    {
        isGameOver = true;
    }

    private void Update()
    {
        if (isGameOver)
        {
            return; // Oyun bitti — ne ileri koş ne swerve dinle
        }

        // --- Yatay (swerve) hareketi hesapla ---
        float swerveAmount = 0f;

        if (Input.GetMouseButton(0)) // Sol tuş (veya dokunmatikte parmak) basılı olduğu sürece
        {
            // "Mouse X": bu karedeki yatay fare/parmak hareketi.
            // swerveSpeed ile hıza çevrilir, Time.deltaTime ile FPS'ten bağımsızlaşır,
            // maxSwerveAmount ile tek karelik kayma sınırlanır.
            swerveAmount = Mathf.Clamp(
                Input.GetAxis("Mouse X") * swerveSpeed * Time.deltaTime,
                -maxSwerveAmount,
                maxSwerveAmount);
        }

        // --- İleri hareket + yatay kaymayı tek seferde uygula ---
        Vector3 newPosition = transform.position + transform.forward * (forwardSpeed * Time.deltaTime);
        newPosition.x += swerveAmount;

        // Kedi yol sınırları dışına çıkamasın
        newPosition.x = Mathf.Clamp(newPosition.x, leftBound, rightBound);

        transform.position = newPosition;
    }
}
