using System.Collections.Generic;
using UnityEngine;

// Chunk (yol parçası) prefab'inin kök objesine takılır: öne geldiğinde spawn noktalarına
// rastgele engel/collectible yerleştirir, eskilerini temizler.
public class ChunkController : MonoBehaviour
{
    [Header("Chunk Settings")]
    public float chunkLength = 20f;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    // Bu chunk'ta duran itemler + geldikleri havuz (iade edebilmek için ikisi birlikte tutulur)
    private struct SpawnedItem
    {
        public GameObject obj;
        public ObjectPool pool;
    }

    private readonly List<SpawnedItem> spawnedItems = new List<SpawnedItem>();

    public void SpawnItems(ObjectPool[] obstaclePools, ObjectPool collectiblePool,
                           ObjectPool starPool, float obstacleChance, float collectibleChance,
                           float starChance)
    {
        ClearSpawns();

        // Adil zorluk: en az bir nokta engel OLMASIN, yol tamamen kapanmasın
        int nonObstacleCount = 0;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            bool isLastPoint = i == spawnPoints.Length - 1;
            float roll = Random.value;

            if (roll < obstacleChance)
            {
                if (isLastPoint && nonObstacleCount == 0)
                {
                    continue;
                }

                if (obstaclePools != null && obstaclePools.Length > 0)
                {
                    Spawn(obstaclePools[Random.Range(0, obstaclePools.Length)], spawnPoints[i]);
                }
            }
            else if (roll < obstacleChance + collectibleChance)
            {
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
                nonObstacleCount++;
            }
        }
    }

    // Bu chunk, spawn ettiği itemlerin TEK iade sahibidir (çapraz-chunk aliasing önlenir)
    public void ClearSpawns()
    {
        foreach (SpawnedItem item in spawnedItems)
        {
            item.pool.Return(item.obj);
        }

        spawnedItems.Clear();
    }

    private void Spawn(ObjectPool pool, Transform point)
    {
        if (pool == null)
        {
            return;
        }

        GameObject obj = pool.Get();
        obj.transform.position = point.position;
        obj.transform.rotation = point.rotation;

        spawnedItems.Add(new SpawnedItem { obj = obj, pool = pool });
    }
}
