using System.Collections.Generic;
using UnityEngine;

// Genel amaçlı obje havuzu: oyun sırasında hiç Instantiate/Destroy yapmamak için
// objeleri önceden üretip kapalı bekletir; isteyene açıp verir (Get),
// işi biten objeyi kapatıp geri saklar (Return). Böylece GC (çöp toplayıcı)
// takılmaları önlenir — hypercasual/WebGL'de akıcılık için kritik.
// Şimdilik chunk'lar için; Faz 4'te engel ve collectible'lar için de AYNI sınıf kullanılacak.
public class ObjectPool : MonoBehaviour
{
    [Header("Havuz Ayarları")]
    // Havuzun üretip dağıtacağı prefab
    public GameObject prefab;

    // Başlangıçta kaç obje üretilsin (yetmezse havuz kendini otomatik büyütür)
    public int initialSize = 7;

    // Kapalı (inactive) bekleyen objeler
    private readonly Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        // Awake tüm Start'lardan önce çalışır → ChunkManager, Start'ında havuzu dolu bulur
        for (int i = 0; i < initialSize; i++)
        {
            pool.Enqueue(CreateNew());
        }
    }

    // Havuzdan bir obje verir ve aktif eder; havuz boşsa yenisini üretir (büyüme)
    public GameObject Get()
    {
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : CreateNew();
        obj.SetActive(true);
        return obj;
    }

    // Objeyi kapatır ve havuza geri koyar (Destroy YOK — obje tekrar kullanılacak)
    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    // Yeni obje üretir, kapatır ve bu havuzun child'ı yapar (Hierarchy düzenli kalsın)
    private GameObject CreateNew()
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        return obj;
    }
}
