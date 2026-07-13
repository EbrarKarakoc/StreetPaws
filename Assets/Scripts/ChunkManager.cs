using System.Collections.Generic;
using UnityEngine;

// Sahnedeki TEK yönetici obje: chunk'ları Start'ta art arda dizer,
// kedi ilerledikçe arkada kalan chunk'ı havuza iade edip önden yenisini alır
// → sonsuz yol illüzyonu. Faz 4: öne gelen her chunk'a engel/collectible spawn'ı
// tetiklenir; ihtimaller ve havuzlar buradan (tek yerden) yönetilir.
public class ChunkManager : MonoBehaviour
{
    [Header("Chunk Ayarları")]
    // Aynı anda sahnede duracak chunk sayısı
    public int chunkCount = 5;

    // Her chunk'ın Z uzunluğu (prefab'deki ChunkController.chunkLength ile AYNI olmalı!)
    public float chunkLength = 20f;

    [Header("Referanslar")]
    // Kedinin transform'u — ne kadar ilerlediğini takip etmek için
    public Transform player;

    // Chunk'ları veren havuz (PoolManager objesindeki ObjectPool bileşeni)
    public ObjectPool chunkPool;

    [Header("Spawn Havuzları (Faz 4)")]
    // Engel havuzları — HER ENGEL TÜRÜ İÇİN BİR HAVUZ (çöp kutusu, ağaç, bank...).
    // Spawn sırasında bu diziden rastgele bir tür seçilir → görsel çeşitlilik.
    public ObjectPool[] obstaclePools;

    // Collectible'ları veren havuz (CollectiblePool objesi)
    public ObjectPool collectiblePool;

    [Header("Spawn İhtimalleri")]
    // Her spawn noktasının ENGEL olma ihtimali (0-1 arası)
    [Range(0f, 1f)] public float obstacleChance = 0.3f;

    // Her spawn noktasının COLLECTIBLE olma ihtimali (0-1 arası).
    // İkisinin toplamı 1'i geçmesin; kalan ihtimal = nokta boş kalır.
    [Range(0f, 1f)] public float collectibleChance = 0.4f;

    [Header("Geri Dönüşüm")]
    // En arkadaki chunk'ın merkezi, oyuncunun kaç chunk-boyu gerisinde kalınca öne taşınsın.
    // 1 = tam bir chunk boyu geride kalınca (kamera arkayı görmesin diye yeterli pay).
    public float recycleThreshold = 1f;

    // Sahnede aktif chunk'lar: Queue'nun başı = en arkadaki, sonu = en öndeki
    private readonly Queue<Transform> activeChunks = new Queue<Transform>();

    // En öndeki chunk'ın Z merkezi — yeni alınan chunk bunun bir boy önüne konur
    private float frontZ;

    private void Start()
    {
        if (chunkPool == null)
        {
            Debug.LogError("ChunkManager: Chunk Pool atanmamış! Inspector'dan PoolManager objesini sürükle.");
            enabled = false; // Update her kare hata basmasın diye scripti kapat
            return;
        }

        if (obstaclePools == null || obstaclePools.Length == 0)
        {
            Debug.LogWarning("ChunkManager: Obstacle Pools dizisi boş — hiç engel spawn edilmeyecek!");
        }

        // Prefab'deki uzunluk ile buradaki uzunluk uyuşmuyorsa kullanıcıyı uyar
        // (uyuşmazlık parçalar arasında boşluk veya üst üste binme yaratır)
        if (chunkPool.prefab != null)
        {
            ChunkController controller = chunkPool.prefab.GetComponent<ChunkController>();
            if (controller != null && !Mathf.Approximately(controller.chunkLength, chunkLength))
            {
                Debug.LogWarning(
                    $"ChunkManager: chunkLength ({chunkLength}) ile prefab'deki " +
                    $"ChunkController.chunkLength ({controller.chunkLength}) FARKLI! İkisini eşitle.");
            }
        }

        // chunkCount kadar chunk'ı havuzdan alıp, manager'ın olduğu noktadan ileri doğru diz.
        // İLK chunk'a item koyma (i > 0 şartı): kedi tam üstünde başlıyor,
        // daha oyun başlamadan engele çarpmasın.
        for (int i = 0; i < chunkCount; i++)
        {
            SpawnChunkAt(transform.position.z + i * chunkLength, withItems: i > 0);
        }
    }

    private void Update()
    {
        if (activeChunks.Count == 0)
        {
            return; // Kurulum yapılamadıysa hata fırlatma
        }

        // En arkadaki chunk'a bak (Queue'nun başı)
        Transform rearChunk = activeChunks.Peek();

        // Kedi bu chunk'ı yeterince geçtiyse: arkadakini havuza iade et, öne yenisini al
        if (player.position.z - rearChunk.position.z > chunkLength * recycleThreshold)
        {
            RecycleRearChunk();
        }
    }

    // Arkadaki chunk'ı (itemleriyle birlikte) havuza iade eder,
    // havuzdan alınan chunk'ı itemleriyle birlikte en öne yerleştirir
    private void RecycleRearChunk()
    {
        Transform rear = activeChunks.Dequeue();

        // Önce chunk'ın üzerindeki itemleri havuzlarına iade et
        ChunkController rearController = rear.GetComponent<ChunkController>();
        if (rearController != null)
        {
            rearController.ClearSpawns();
        }

        chunkPool.Return(rear.gameObject);

        SpawnChunkAt(frontZ + chunkLength, withItems: true);
    }

    // Havuzdan bir chunk alır, verilen Z merkezine yerleştirir ve istenirse item spawn'lar
    private void SpawnChunkAt(float z, bool withItems)
    {
        GameObject chunk = chunkPool.Get();
        chunk.transform.position = new Vector3(transform.position.x, transform.position.y, z);

        activeChunks.Enqueue(chunk.transform);
        frontZ = z; // En son yerleştirilen her zaman en öndedir

        if (withItems)
        {
            ChunkController controller = chunk.GetComponent<ChunkController>();
            if (controller != null)
            {
                controller.SpawnItems(obstaclePools, collectiblePool, obstacleChance, collectibleChance);
            }
        }
    }
}
