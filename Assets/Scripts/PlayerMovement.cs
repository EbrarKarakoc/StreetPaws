using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Score")]
    public int score = 0; // Anlık skor, Inspector'dan da görülebilsin diye public

    [Header("UI References")]
    [SerializeField] private TMP_Text scoreText; // Ekrandaki "Paws:X" yazısı

    [Header("Game Flow")]
    [SerializeField] private GameFlowManager gameFlowManager; // Start/Game Over ekranlarını yöneten script

    [Header("Forward Movement")]
    [SerializeField] private float forwardSpeed = 2f; // Başlangıç ileri hızı
    [SerializeField] private float speedIncreaseAmount = 0.5f; // Her eşikte hıza eklenecek miktar
    [SerializeField] private float speedIncreaseDistance = 10f; // Kaç metrede bir hız artacak

    [Header("Swerve Settings")]
    [SerializeField] private float swerveSensitivity = 0.02f; // Ekran pikseli -> dünya birimine çeviren katsayı
    [SerializeField] private float smoothTime = 0.08f;        // Küçük değer = daha keskin/hızlı tepki, büyük değer = daha yumuşak/gecikmeli
    [SerializeField] private float minX = -4.5f; // Yolun sol sınırı
    [SerializeField] private float maxX = 4.5f;  // Yolun sağ sınırı

    private float targetX; // Kedinin ulaşmaya çalıştığı hedef X konumu
    private float currentXVelocity; // SmoothDamp'in kendi içinde kullandığı, hızı takip eden yardımcı değişken
    private Vector3 lastInputPosition; // Sürüklemede bir önceki mouse/touch konumu
    private bool isDragging; // Şu an ekrana basılı/sürükleniyor mu
    private bool isGameOver; // Oyun bitti mi
    private bool hasStarted; // Start ekranı geçildi, oyun fiilen başladı mı
    private float distanceTraveled; // Toplam kat edilen mesafe (metre)
    private float nextSpeedIncreaseDistance; // Bir sonraki hız artışının tetikleneceği mesafe eşiği

    private void Start()
    {
        targetX = transform.position.x; // Kedi hangi X'te başladıysa hedefi de orası olsun
        nextSpeedIncreaseDistance = speedIncreaseDistance; // İlk hız artış eşiğini kur

        UpdateScoreUI(); // Skor yazısını "Paws:0" ile başlat
    }

    // GameFlowManager, Start ekranı kapanınca bunu çağırır; oyuncu bu andan sonra hareket etmeye başlar
    public void BeginGame()
    {
        hasStarted = true;
    }

    private void Update()
    {
        // Start ekranı geçilmediyse veya oyun bittiyse kediyi hiç hareket ettirme
        if (!hasStarted || isGameOver)
        {
            return;
        }

        // Z ekseninde sürekli ileri hareket, girişten bağımsız her zaman aktif
        float moveDistance = forwardSpeed * Time.deltaTime; // FPS'den bağımsız olsun diye Time.deltaTime ile çarpıyoruz
        transform.Translate(Vector3.forward * moveDistance, Space.World);

        // Zorluk zamanla değil, kat edilen mesafeyle artsın; böylece hızı oyuncunun performansı belirler
        distanceTraveled += moveDistance;
        if (distanceTraveled >= nextSpeedIncreaseDistance)
        {
            forwardSpeed += speedIncreaseAmount; // Hızı artır
            nextSpeedIncreaseDistance += speedIncreaseDistance; // Bir sonraki eşiği ileri kaydır
        }

        HandleSwerveInput();

        // Şu anki X'ten hedef X'e ani sıçrama olmadan, yumuşak bir şekilde ilerle
        float smoothedX = Mathf.SmoothDamp(transform.position.x, targetX, ref currentXVelocity, smoothTime);
        transform.position = new Vector3(smoothedX, transform.position.y, transform.position.z);
    }

    // Unity, bu objenin collider'ı "Is Trigger" işaretli başka bir collider'a değince otomatik çağırır
    private void OnTriggerEnter(Collider other)
    {
        if (isGameOver)
        {
            return; // Oyun bittiyse artık hiçbir tetikleyiciyi işleme
        }

        if (other.CompareTag("Obstacle"))
        {
            TriggerGameOver();
        }
        else if (other.CompareTag("Coin"))
        {
            CollectCoin(other.gameObject);
        }
        else if (other.CompareTag("Star"))
        {
            CollectStar(other.gameObject);
        }
        else if (other.CompareTag("Bomb"))
        {
            HitBomb(other.gameObject);
        }
    }

    private void CollectCoin(GameObject coin)
    {
        Destroy(coin); // Toplanan coin'i sahneden kaldır, bir daha toplanamasın

        score++;
        Debug.Log("Score: " + score);
        UpdateScoreUI();
    }

    private void CollectStar(GameObject star)
    {
        Destroy(star);

        score += 5; // Star, coin'den daha değerli
        Debug.Log("Score: " + score);
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null) // Inspector'da atanmamışsa hata vermeden sessizce geç
        {
            scoreText.text = "Paws:" + score;
        }
    }

    private void HitBomb(GameObject bomb)
    {
        // Önce bombanın kendi patlama animasyonunu oynatmayı dene
        Animator bombAnimator = bomb.GetComponent<Animator>();
        if (bombAnimator != null)
        {
            bombAnimator.SetTrigger("Explode"); // Animator Controller'daki "Explode" tetikleyicisini çalıştır
        }
        else
        {
            Debug.Log("BOOM! Game Over"); // Animator yoksa en azından konsola yaz
        }

        TriggerGameOver();
    }

    private void TriggerGameOver()
    {
        isGameOver = true;

        // Hedef X'i şu anki konuma kilitle, SmoothDamp'in kalan bir hareketle kaymasını önle
        targetX = transform.position.x;
        currentXVelocity = 0f;
        isDragging = false;

        // UI'ı kendimiz göstermek yerine bu işi GameFlowManager'a devrediyoruz (tek sorumluluk prensibi)
        if (gameFlowManager != null)
        {
            gameFlowManager.ShowGameOver();
        }

        Debug.Log("Game Over! You hit an obstacle.");
    }

    private void HandleSwerveInput()
    {
        // --- Mouse (Editor / masaüstü WebGL) ---
        if (Input.GetMouseButtonDown(0)) // Tıklamanın başladığı tek frame
        {
            isDragging = true;
            lastInputPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0)) // Bırakıldığı frame
        {
            isDragging = false;
        }
        else if (isDragging && Input.GetMouseButton(0)) // Basılı tutulduğu sürece her frame
        {
            Vector3 currentInputPosition = Input.mousePosition;
            float deltaX = currentInputPosition.x - lastInputPosition.x; // Bir önceki frame'e göre fark
            lastInputPosition = currentInputPosition;

            ApplySwerveDelta(deltaX);
        }

        // --- Dokunma (Mobil) ---
        if (Input.touchCount > 0) // Ekranda en az bir parmak var mı
        {
            Touch touch = Input.GetTouch(0); // İlk parmağı al

            switch (touch.phase)
            {
                case TouchPhase.Began: // Parmak yeni değdi
                    isDragging = true;
                    break;
                case TouchPhase.Moved: // Parmak ekranda hareket ediyor
                    if (isDragging)
                    {
                        ApplySwerveDelta(touch.deltaPosition.x); // Unity'nin hazır verdiği çerçeveler arası fark
                    }
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled: // Parmak kalktı veya iptal oldu
                    isDragging = false;
                    break;
            }
        }
    }

    private void ApplySwerveDelta(float screenDeltaX)
    {
        // Ekran pikseli cinsindeki farkı dünya birimine çevir, sonra yoldan çıkmayacak şekilde sınırla
        targetX += screenDeltaX * swerveSensitivity;
        targetX = Mathf.Clamp(targetX, minX, maxX);
    }
}
