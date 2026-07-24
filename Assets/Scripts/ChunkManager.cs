using System.Collections.Generic;
using UnityEngine;

// Sahnedeki tek yönetici: chunk'ları dizer, kedi ilerledikçe arkadaki chunk'ı öne taşır
// (sonsuz yol illüzyonu) ve öne gelen chunk'a engel/collectible spawn ettirir.
public class ChunkManager : MonoBehaviour
{
    [Header("Chunk Settings")]
    public int chunkCount = 5;
    public float chunkLength = 20f;

    [Header("References")]
    public Transform player;
    public ObjectPool chunkPool;

    [Header("Spawn Pools (Phase 4)")]
    public ObjectPool[] obstaclePools;
    public ObjectPool collectiblePool;
    public ObjectPool starPool;

    [Header("Spawn Chances")]
    [Range(0f, 1f)] public float obstacleChance = 0.3f;
    [Range(0f, 1f)] public float collectibleChance = 0.4f;
    [Range(0f, 1f)] public float starChance = 0.12f;

    [Header("Recycling")]
    public float recycleThreshold = 1f;

    // Aktif chunk'lar: Queue'nun başı = en arkadaki, sonu = en öndeki
    private readonly Queue<Transform> activeChunks = new Queue<Transform>();
    private float frontZ;

    private void Start()
    {
        if (chunkPool == null)
        {
            Debug.LogError("ChunkManager: Chunk Pool atanmamış! Inspector'dan PoolManager objesini sürükle.");
            enabled = false;
            return;
        }

        if (obstaclePools == null || obstaclePools.Length == 0)
        {
            Debug.LogWarning("ChunkManager: Obstacle Pools dizisi boş — hiç engel spawn edilmeyecek!");
        }

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

        // İlk chunk'a item koyma: kedi tam üstünde başlıyor
        for (int i = 0; i < chunkCount; i++)
        {
            SpawnChunkAt(transform.position.z + i * chunkLength, withItems: i > 0);
        }
    }

    private void Update()
    {
        if (activeChunks.Count == 0)
        {
            return;
        }

        Transform rearChunk = activeChunks.Peek();

        if (player.position.z - rearChunk.position.z > chunkLength * recycleThreshold)
        {
            RecycleRearChunk();
        }
    }

    private void RecycleRearChunk()
    {
        Transform rear = activeChunks.Dequeue();

        ChunkController rearController = rear.GetComponent<ChunkController>();
        if (rearController != null)
        {
            rearController.ClearSpawns();
        }

        chunkPool.Return(rear.gameObject);

        SpawnChunkAt(frontZ + chunkLength, withItems: true);
    }

    private void SpawnChunkAt(float z, bool withItems)
    {
        GameObject chunk = chunkPool.Get();
        chunk.transform.position = new Vector3(transform.position.x, transform.position.y, z);

        activeChunks.Enqueue(chunk.transform);
        frontZ = z;

        if (withItems)
        {
            ChunkController controller = chunk.GetComponent<ChunkController>();
            if (controller != null)
            {
                controller.SpawnItems(obstaclePools, collectiblePool, starPool, obstacleChance, collectibleChance, starChance);
            }
        }
    }
}
