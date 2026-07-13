using System.Collections.Generic;
using UnityEngine;

// Her chunk (yol parçası) prefab'inin KÖK objesine takılır.
// Faz 4: Chunk her öne geldiğinde, üzerindeki spawn noktalarına rastgele
// engel/collectible yerleştirir (havuzdan çeker) ve eskilerini temizler.
public class ChunkController : MonoBehaviour
{
    [Header("Chunk Ayarları")]
    // Bu chunk'ın Z eksenindeki uzunluğu (ChunkManager'daki chunkLength ile AYNI olmalı!)
    public float chunkLength = 20f;

    [Header("Spawn Noktaları")]
    // Prefab içine elle yerleştirilen boş objeler (örn. x = -3/0/3, farklı z'lerde).
    // Her nokta her turda ya engel, ya collectible alır ya da boş kalır.
    public Transform[] spawnPoints;

    // Bu chunk'ta şu an duran itemler + geldikleri havuz (iade edebilmek için ikisi birlikte tutulur)
    private struct SpawnedItem
    {
        public GameObject obj;
        public ObjectPool pool;
    }

    private readonly List<SpawnedItem> spawnedItems = new List<SpawnedItem>();

    // Chunk öne taşındığında ChunkManager çağırır: önce eskileri temizler,
    // sonra her spawn noktası için zar atıp engel/collectible/boş kararı verir.
    // obstaclePools: her eleman farklı bir engel türünün havuzu; tür rastgele seçilir.
    public void SpawnItems(ObjectPool[] obstaclePools, ObjectPool collectiblePool,
                           float obstacleChance, float collectibleChance)
    {
        ClearSpawns(); // Önceki turdan kalanlar üst üste binmesin

        // ADİL ZORLUK KURALI: Tüm spawn noktaları aynı anda engel olursa yol tamamen
        // kapanır ve oyun imkansızlaşır. Bu yüzden en az bir noktanın engel OLMAMASI
        // garanti edilir (aşağıdaki nonObstacleCount kontrolü).
        int nonObstacleCount = 0;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            bool isLastPoint = i == spawnPoints.Length - 1;
            float roll = Random.value; // 0..1 arası zar

            if (roll < obstacleChance)
            {
                // Son noktaya geldik ve öncekilerin HEPSİ engel olduysa → boş bırak
                if (isLastPoint && nonObstacleCount == 0)
                {
                    continue;
                }

                // Rastgele bir engel TÜRÜ seç (çöp kutusu / ağaç / bank ...)
                if (obstaclePools != null && obstaclePools.Length > 0)
                {
                    Spawn(obstaclePools[Random.Range(0, obstaclePools.Length)], spawnPoints[i]);
                }
            }
            else if (roll < obstacleChance + collectibleChance)
            {
                Spawn(collectiblePool, spawnPoints[i]);
                nonObstacleCount++;
            }
            else
            {
                nonObstacleCount++; // Boş nokta da geçilebilir sayılır
            }
        }
    }

    // Bu chunk'a ait itemleri havuzlarına iade eder
    public void ClearSpawns()
    {
        foreach (SpawnedItem item in spawnedItems)
        {
            // Toplanmış collectible kendini ZATEN havuza iade etti (activeSelf = false).
            // İkinci kez Return edersek havuzda çift kayıt oluşur → aynı obje iki kez
            // dağıtılır ve hayalet objeler görürüz. O yüzden sadece aktif olanları iade et.
            if (item.obj.activeSelf)
            {
                item.pool.Return(item.obj);
            }
        }

        spawnedItems.Clear();
    }

    // Havuzdan bir obje alıp verilen noktaya yerleştirir ve kayıt tutar
    private void Spawn(ObjectPool pool, Transform point)
    {
        if (pool == null)
        {
            return; // Havuz bağlanmamışsa (Inspector'da atama unutulduysa) sessizce geç
        }

        GameObject obj = pool.Get();
        obj.transform.position = point.position;
        obj.transform.rotation = point.rotation;

        // Collectible ise kendini iade edebilmesi için havuz referansını ver
        Collectible collectible = obj.GetComponent<Collectible>();
        if (collectible != null)
        {
            collectible.pool = pool;
        }

        spawnedItems.Add(new SpawnedItem { obj = obj, pool = pool });
    }
}
