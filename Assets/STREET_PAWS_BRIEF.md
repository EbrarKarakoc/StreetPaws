# STREET_PAWS_BRIEF.md — Endless Runner Sistemi

> **Bu dosya Claude Code için handoff brief'idir.**
> Sen (Claude Code) SADECE C# scriptlerini yazacaksın.
> Unity Editor içindeki manuel işleri (prefab oluşturma, obje sürükleme,
> Inspector'da referans bağlama, değer ayarlama) KULLANICI yapacak.
> Her faz sonunda "MANUEL ADIMLAR" bölümünü kullanıcıya net şekilde hatırlat.
> Fazları SIRAYLA yap. Kullanıcı "Faz 2'yi yap" demeden sonraki faza geçme.

---

## PROJE BAĞLAMI

- **Oyun:** Street Paws — 3D hypercasual endless runner (Unity, C#)
- **Karakter:** Bir kedi, otomatik ileri koşar (Temple Run tarzı)
- **Kontrol:** Serbest yatay hareket (şeritsiz swerve) — parmak/mouse sağa-sola
  kaydırınca kedi süzülerek yana gider, sınırlar içinde kalır
- **Mevcut durum:** Kedi ileri kayıyor, tek büyük düz plane var, şerit yok
- **Hedef:** Tek plane'i sonsuz akan chunk sistemine çevirmek + object pooling
  + engel/collectible spawn

### Kod standartları
- Türkçe yorum satırları kullan (kullanıcı Türkçe çalışıyor)
- Her public değişkene `[Header]` ve açıklayıcı yorum ekle
- Inspector'dan ayarlanabilir olması gereken değerleri `public` veya
  `[SerializeField] private` yap
- Kısa, okunabilir, tek sorumluluk prensibine uygun scriptler yaz
- Magic number kullanma, hepsini Inspector'a çıkar

---

## FAZ 1 — Serbest Yatay Hareket (Swerve)

**Amaç:** Kedi otomatik ileri giderken parmak kaydırmasıyla yana süzülsün,
sol/sağ sınırlarda dursun.

### Claude Code yapacak:
`CatController.cs` scriptini oluştur:
- İleri hareket: `transform.forward * forwardSpeed * Time.deltaTime`
- Yatay hareket: `Input.GetMouseButton(0)` basılıyken `Input.GetAxis("Mouse X")`
  ile deltaX al, `swerveSpeed` ile çarp, `maxSwerveAmount` ile clamp'le
- Final pozisyonda `x` değerini `leftBound` / `rightBound` arasında `Mathf.Clamp`
- Public alanlar: forwardSpeed, swerveSpeed, maxSwerveAmount, leftBound, rightBound
- Eğer kedide Rigidbody varsa yorum olarak "Is Kinematic açık olmalı" notu bırak

### MANUEL ADIMLAR (kullanıcı yapacak):
1. `CatController.cs` scriptini kedi objesine ekle
2. Varsa eski hareket scriptini kaldır (çakışmasın)
3. Inspector değerleri: forwardSpeed=8, swerveSpeed=30, maxSwerveAmount=1.2,
   leftBound=-3, rightBound=3
4. Play → mouse sol tuş basılı sürükle → kedi süzülmeli
5. Kedi yanlış yöne gidiyorsa: kediyi Y'de 180° döndür (mavi Z oku ileriye baksın)

**✅ Faz 1 bitince kullanıcı test etsin, onaylayınca Faz 2'ye geç.**

---

## FAZ 2 — Chunk (Zemin Parçası) Sistemi

**Amaç:** Tek büyük plane yerine, sabit uzunlukta yol parçaları (chunk) art arda
diziler. Kedi ilerledikçe arkada kalan chunk öne taşınır → sonsuz yol illüzyonu.

### Mimari
- Bir chunk = sabit uzunlukta (örn. 20 birim) bir yol parçası prefab'i
- Aynı anda ekranda birkaç chunk bulunur (örn. 5 tane), art arda dizili
- Kedi bir chunk boyu ilerlediğinde, en arkadaki chunk en öne taşınır (recycle)
- Böylece hiç Instantiate/Destroy yapılmaz, aynı chunk'lar döner durur

### Claude Code yapacak:

**1. `ChunkController.cs`** (her chunk prefab'ine takılacak)
- `chunkLength` (float) — bu chunk'ın Z uzunluğu
- (İleride spawn noktaları burada tutulacak, şimdilik boş bırak, yorum ekle)

**2. `ChunkManager.cs`** (sahnede tek bir yönetici obje)
- `chunkPrefab` referansı
- `chunkCount` — aynı anda kaç chunk olsun (örn. 5)
- `chunkLength` — her chunk'ın uzunluğu (örn. 20)
- `player` (Transform) — kediyi takip için referans
- Start'ta: chunkCount kadar chunk'ı art arda diz (Z ekseninde)
  ve bir `List<Transform>` veya `Queue` içinde tut
- Update'te: eğer kedi, en arkadaki chunk'ı yeterince geçtiyse
  (player.z - arkadakiChunk.z > chunkLength * eşik), o chunk'ı
  en öndeki chunk'ın önüne taşı (Z pozisyonunu güncelle)
- Bu fazda henüz object pool ayrı sınıf değil; recycle mantığını
  ChunkManager içinde basit tut. Faz 3'te ayrı pool'a taşıyacağız.

### MANUEL ADIMLAR (kullanıcı yapacak):
1. Mevcut büyük plane'i sil (veya deaktif et)
2. Bir chunk prefab'i oluştur:
   - Boş GameObject → içine bir Plane/zemin koy, 20 birim uzunlukta ölçekle
   - `ChunkController` scriptini ekle, chunkLength=20 gir
   - Prefabs klasörüne sürükleyip prefab yap, sahnedekini sil
3. Sahneye boş bir "ChunkManager" objesi oluştur, `ChunkManager` scriptini ekle
4. Inspector'da: chunkPrefab = oluşturduğun prefab, player = kedi,
   chunkCount=5, chunkLength=20 (prefab ile AYNI olmalı)
5. Play → sonsuz akan yol görmelisin. Kedi ilerledikçe boşluk oluşmamalı.

**⚠️ Önemli:** chunkLength iki yerde de (prefab + manager) aynı olmalı,
yoksa parçalar arasında boşluk/üst üste binme olur.

**✅ Faz 2 bitince test, onay, sonra Faz 3.**

---

## FAZ 3 — Object Pooling (Genel Havuz)

**Amaç:** Chunk recycle mantığını temiz bir "object pool" sınıfına çıkar. Bu
sınıf ileride engeller ve collectible'lar için de kullanılacak (tekrar kullanılabilir
altyapı). Amaç: oyun boyunca hiç Instantiate/Destroy yapmayıp GC (garbage collector)
takılmalarını önlemek → hypercasual'da akıcılık kritik.

### Claude Code yapacak:

**`ObjectPool.cs`** — generic, tekrar kullanılabilir havuz
- `Queue<GameObject>` ile havuz tut
- `prefab` ve `initialSize` alanları
- `Get()` — havuzdan bir obje ver (yoksa yenisini büyüt), aktif et
- `Return(GameObject)` — objeyi deaktif et, havuza geri koy
- Awake'te initialSize kadar obje üretip deaktif havuza doldur

**`ChunkManager.cs`'i güncelle:**
- Chunk'ları artık ObjectPool üzerinden Get/Return ile yönet
- Arkada kalan chunk → pool'a Return, öne yeni chunk → pool'dan Get + konumlandır

### MANUEL ADIMLAR (kullanıcı yapacak):
1. Sahneye bir "PoolManager" objesi (veya ChunkManager üstüne) ObjectPool ekle
2. Inspector'da prefab = chunk prefab, initialSize = chunkCount + 2 (örn. 7)
3. ChunkManager'ın yeni pool referansını bağla
4. Play → görsel olarak Faz 2 ile aynı görünmeli ama artık pool'dan besleniyor
   (Hierarchy'de sürekli yeni obje oluşup silinmediğini gözlemle)

**✅ Faz 3 bitince test, onay, sonra Faz 4.**

---

## FAZ 4 — Engel & Collectible Spawn

**Amaç:** Her chunk öne geldiğinde üzerine rastgele engeller (dodge edilecek)
ve toplanabilir objeler (coin/balık) yerleştir.

### Claude Code yapacak:

**1. `ChunkController.cs`'i genişlet:**
- Chunk prefab'i içine önceden yerleştirilmiş "spawn point" Transform'ları tut
  (`Transform[] spawnPoints`)
- `SpawnItems()` metodu: chunk her recycle olduğunda çağrılır, spawn point'lere
  rastgele engel/collectible yerleştirir (pool'dan çeker)
- Önceki spawn'ları temizle (pool'a Return) ki üst üste binmesin

**2. `Obstacle.cs` ve `Collectible.cs`:**
- `Obstacle`: kedi çarparsa OnTriggerEnter ile game over / can azalt
  (şimdilik sadece Debug.Log ile "Çarpıştı" yaz, game over sistemini
  ayrı fazda kuracağız)
- `Collectible`: kedi değince Debug.Log "Toplandı" + kendini pool'a Return

**3. Spawn mantığı ChunkManager veya ChunkController'da:**
- Rastgelelik: her spawn point için %ihtimalle engel mi collectible mi boş mu
- Engellerin ARASINDAN geçilebilir olmasına dikkat (hepsini aynı anda tüm
  yatay ekseni kapatma → oyun imkansız olmasın). Yorum olarak bu kuralı ekle.

### MANUEL ADIMLAR (kullanıcı yapacak):
1. Basit engel prefab'i yap (küp yeterli), collider'ı Is Trigger, `Obstacle` ekle
2. Basit collectible prefab'i yap (küre/coin), collider Is Trigger, `Collectible` ekle
3. Chunk prefab'i içine boş "SpawnPoint" objeleri koy (örn. sol/orta/sağ x
   pozisyonlarında birkaç tane), ChunkController'a dizi olarak bağla
4. Engel ve collectible için ObjectPool kur (Faz 3'teki ObjectPool'u tekrar kullan)
5. Kediye Rigidbody (Is Kinematic açık) + collider olduğundan emin ol
   (trigger algılansın diye en az birinde Rigidbody şart)
6. Play → engeller/coinler gelmeli, çarpınca/toplayınca Console'da log görmeli

**✅ Faz 4 bitince temel endless runner iskeleti tamamdır.**

---

## SONRAKİ FAZLAR (İleride — şimdilik yapma)
- Game over + restart sistemi
- Skor + mesafe sayacı
- Shop: Magnet ve Shield upgrade'leri
- Game feel polish: kamera shake, particle efektleri
- Zorluk artışı (hız/spawn yoğunluğu mesafeye göre artar)

---

## ÇALIŞMA KURALI (Claude Code için özet)
1. Sadece istenen fazı yap, sonrakine geçme.
2. Her fazda önce scriptleri yaz, sonra "MANUEL ADIMLAR"ı net madde madde hatırlat.
3. Değişkenleri Inspector'a çıkar, Türkçe yorum ekle.
4. Kullanıcı test edip onaylamadan ilerleme.
5. Bir hata olursa kullanıcının vereceği Console log'una göre düzelt.
