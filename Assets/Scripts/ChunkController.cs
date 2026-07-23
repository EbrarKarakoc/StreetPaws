using System.Collections.Generic;
using UnityEngine;

// Her chunk (yol parçası) prefab'inin KÖK objesine takılır.
// Faz 4: Chunk her öne geldiğinde, üzerindeki spawn noktalarına rastgele
// engel/collectible yerleştirir (havuzdan çeker) ve eskilerini temizler.
public class ChunkController : MonoBehaviour
{
    [Header("Chunk Settings")]
    // Bu chunk'ın Z eksenindeki uzunluğu (ChunkManager'daki chunkLength ile AYNI olmalı!)
    public float chunkLength = 20f;

    [Header("Spawn Points")]
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
                           ObjectPool starPool, float obstacleChance, float collectibleChance,
                           float starChance)
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
                // Collectible şeridi: nadiren coin yerine STAR çıkar (starChance),
                // gerisi normal coin. Star da coin gibi havuzlanır (tek-sahiplik iadesi).
                if (starPool != null && Random.value < starChance)
                {
                    Spawn(starPool, spawnPoints[i]);
                }
                else
                {
                    Spawn(collectiblePool, spawnPoints[i]);
                }
                nonObstacleCount++;
            }
            else
            {
                nonObstacleCount++; // Boş nokta da geçilebilir sayılır
            }
        }
    }

    // Bu chunk'a ait itemleri havuzlarına iade eder.
    // Bu chunk, spawn ettiği her item'ın TEK iade sahibidir: toplanan collectible'lar
    // kendini yalnızca gizler (SetActive false), havuza asıl iadeyi burası yapar. Böylece
    // aynı obje asla iki kez iade edilmez ve aktif bir item başka chunk'a "çalınmaz".
    public void ClearSpawns()
    {
        foreach (SpawnedItem item in spawnedItems)
        {
            item.pool.Return(item.obj);
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

        spawnedItems.Add(new SpawnedItem { obj = obj, pool = pool });
    }
}
