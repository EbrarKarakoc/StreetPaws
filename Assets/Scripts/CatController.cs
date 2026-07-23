using UnityEngine;

// Kedinin hareketini yöneten script: otomatik ileri koşu + serbest yatay kayma (swerve).
// Faz 5: İleri hız artık SABİT değil — kat edilen mesafeyle yumuşakça artar (hız rampası)
// ve bir tavana kadar yükselir. Zorluk zamanla değil, oyuncunun ilerlemesiyle artar.
// Tek sorumluluk: hareket (ileri koşu + rampa + swerve). Skor, çarpışma, oyun akışı başka scriptlerin işi.
// NOT: Kedide Rigidbody varsa "Is Kinematic" AÇIK olmalı — hareketi fizik motoru değil
// bu script (transform) yönetiyor; kinematik kapalı olursa ikisi çakışır.
public class CatController : MonoBehaviour
{
    [Header("Forward Movement — Speed Ramp")]
    // Koşunun BAŞLANGIÇ hızı (birim/sn): mesafe 0'ken kedi bu hızda koşar
    public float baseSpeed = 8f;

    // Hızın çıkabileceği TAVAN (birim/sn): bu değere ulaşınca artış durur
    public float maxSpeed = 16f;

    // Her 100 metrede hıza eklenecek miktar — rampanın eğimi (dikliği)
    public float speedGainPer100m = 0.4f;

    [Header("Swerve (Lateral Slide)")]
    // Mouse/parmak yatay hareketini kayma hızına çeviren katsayı
    public float swerveSpeed = 30f;

    // Tek karede uygulanabilecek en büyük yatay kayma miktarı (ani sıçramayı önler)
    public float maxSwerveAmount = 1.2f;

    [Header("Road Bounds")]
    // Kedinin gidebileceği en soldaki X konumu
    public float leftBound = -3f;

    // Kedinin gidebileceği en sağdaki X konumu
    public float rightBound = 3f;

    [Header("Runtime State (Live)")]
    // Koşu başından beri kat edilen ileri mesafe (metre) — hem hız rampasının girdisi
    // hem de SKOR (Paws) kaynağı: ScoreManager bunu DistanceTraveled ile okur (1 m = 1 Paws).
    [SerializeField] private float distanceTraveled;

    // O anki hesaplanan ileri hız — Inspector'da rampayı izlemek/tune etmek için görünür
    [SerializeField] private float currentSpeed;

    // ScoreManager'ın Paws skorunu okuması için salt-okunur erişim
    public float DistanceTraveled => distanceTraveled;

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

        // --- HIZ RAMPASI: hızı, o ana kadar kat edilen mesafeye göre hesapla ---
        // Sürekli (yumuşak) artış: her 100 m'de +speedGainPer100m. Mathf.Min tavanı uygular,
        // yani hız maxSpeed'e ulaşınca orada sabitlenir (daha fazla yükselmez).
        currentSpeed = Mathf.Min(baseSpeed + (distanceTraveled / 100f) * speedGainPer100m, maxSpeed);

        // Bu karede alınacak ileri yol (FPS'ten bağımsız olsun diye * Time.deltaTime)
        float forwardStep = currentSpeed * Time.deltaTime;

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
        Vector3 newPosition = transform.position + transform.forward * forwardStep;
        newPosition.x += swerveAmount;

        // Kedi yol sınırları dışına çıkamasın
        newPosition.x = Mathf.Clamp(newPosition.x, leftBound, rightBound);

        transform.position = newPosition;

        // Kat edilen ileri mesafeyi biriktir (yalnızca ileri bileşen; swerve mesafe saymaz).
        // Bir sonraki karede rampa bu güncel mesafeyi kullanıp hızı biraz daha artıracak.
        distanceTraveled += forwardStep;
    }
}
