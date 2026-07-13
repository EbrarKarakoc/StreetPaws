# 🐾 Street Paws

![Unity](https://img.shields.io/badge/Unity-6000.5.3f1-000000?logo=unity&logoColor=white)
![Pipeline](https://img.shields.io/badge/URP-Universal%20Render%20Pipeline-5c2d91)
![Platform](https://img.shields.io/badge/Platform-WebGL%20%7C%20Mobile-orange)
![Status](https://img.shields.io/badge/Status-Prototype-yellow)

**Street Paws** is a 3D hypercasual **endless runner** built with Unity. You control a low-poly street cat sprinting through an endless city street — swerve left and right with a single finger to dodge trash bins, trees and signs while collecting PawCoins for the highest score.

## 🎮 Core Gameplay

* **Swerve-only control** — one finger (or mouse drag) moves the cat smoothly across the road. No jumping, no stopping. By design.
* **Endless city** — the road never ends, built from recycled chunks with buildings flowing past on both sides.
* **Risk & reward** — grab PawCoins (+1) while avoiding lethal street obstacles.
* **Instant restart** — hit something, watch the animated Game Over card pop in, tap anywhere (or smash the pulsing **TEKRAR OYNA** button) and you're running again in under a second.

## 🏗️ Architecture Highlights

The whole game runs with **zero runtime `Instantiate`/`Destroy` calls** — everything is pooled, which keeps WebGL and mobile builds stutter-free.

| Script | Responsibility |
|---|---|
| `CatController` | Auto-forward run + drag-based swerve within road bounds |
| `ChunkManager` | Lays out road chunks, recycles the rear chunk to the front as the cat advances |
| `ChunkController` | Per-chunk spawn points; spawns/clears obstacles & collectibles on recycle |
| `ObjectPool` | Generic reusable pool (chunks, every obstacle type, collectibles) |
| `Obstacle` | Trigger → Game Over via the state manager |
| `Collectible` | Trigger → adds score, returns itself to its pool |
| `ScoreManager` | PawCoin counter + top-right HUD bar |
| `GameStateManager` | Run/GameOver state, delayed any-input restart (scene reload) |
| `GameOverScreen` | Code-driven UI animation: fade-in, card pop (ease-out-back), pulsing button |

**Fair difficulty guarantee:** the spawn system never fills every spawn point of a chunk with obstacles — there is always an escape corridor.

**Obstacle variety:** each obstacle type (trash bin, tree, sign...) has its own pool; the spawner picks a random type per spawn point.

## 🕹️ Controls

| Action | Desktop | Mobile / Touch |
|---|---|---|
| Swerve left / right | Hold left mouse button + drag | Drag finger across the screen |
| Restart after Game Over | Any key or click | Tap anywhere |

## ▶️ Getting Started

1. Install [Unity Hub](https://unity.com/download) and **Unity 6000.5.3f1** (Unity 6.5) with URP support.
2. Clone this repository and add the project folder in Unity Hub (`Add project from disk`).
3. Open `Assets/Scenes/SampleScene.unity` and press **Play**.

## 🗺️ Roadmap

- [x] Swerve movement system
- [x] Endless chunk-based road + object pooling
- [x] Obstacle & collectible spawn system (multi-type obstacles)
- [x] Animated Game Over card + instant restart
- [x] Score HUD (PawCoins)
- [ ] Gold Stars (+5) and Bombs as distinct pickups
- [ ] Best-score save + score on the Game Over card
- [ ] Chunk layout variants for more visual variety
- [ ] Moving obstacles (ping-pong bikes/scooters)
- [ ] 5 prototype levels + **Cat Evolution** reward
- [ ] Shop: Magnet & Shield power-ups
- [ ] WebGL build & publish (itch.io / GitHub Pages)

## 🧰 Tech & Assets

* **Engine:** Unity 6000.5.3f1 — Universal Render Pipeline (URP), C#
* **UI:** Unity UI + TextMesh Pro
* **Art packs:**
  * [SimplePoly City — Low Poly Assets](https://assetstore.unity.com) (streets, buildings, props)
  * BTM Items & Gems (coins, stars, bombs)
  * Ladymito Free Cat (cat model & animations)

---

*This is a living prototype — the design document (GDD) evolves alongside the code.* 🐈
