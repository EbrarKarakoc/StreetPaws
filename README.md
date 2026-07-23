# Street Paws

![Unity](https://img.shields.io/badge/Unity-6000.5.3f1-000000?logo=unity&logoColor=white)
![Pipeline](https://img.shields.io/badge/URP-Universal%20Render%20Pipeline-5c2d91)
![Platform](https://img.shields.io/badge/Platform-WebGL%20%7C%20Mobile-orange)
![Status](https://img.shields.io/badge/Status-Prototype-yellow)

**Street Paws** is a 3D hypercasual **endless runner** built with Unity. You control a low-poly street cat sprinting through an endless city street — swerve left and right with a single finger to dodge trash bins, trees and benches, collect PawCoins, and chase your own **distance record**.

## Core Gameplay (current build)

* **Swerve-only control** — one finger (or mouse drag) moves the cat smoothly across the road. No jumping, no stopping. By design.
* **Endless city** — the road never ends, built from recycled chunks with buildings flowing past on both sides.
* **Speed ramp** — the run keeps accelerating with distance (8 → 16 units/s); difficulty is speed, not level design.
* **Score, coins & record** — distance = **Paws** score; **PawCoins** are a separate wallet (+1 each); your best distance is saved (PlayerPrefs) and shown on the Game Over card.
* **Stars & bombs** — grab a rare **Star** for a timed **2× Paws** multiplier (screen flash + pulsing 2× badge); a **Bomb** ends the run on contact.
* **Instant restart** — hit something, watch the animated Game Over card pop in, tap anywhere (or smash the pulsing **TEKRAR OYNA** button) and you're running again in under a second.

## Design Pillars (GDD v0.5)

The design targets below are locked in and being implemented stage by stage (see Roadmap):

* **One uninterrupted run, no levels.** The only goal is beating your personal distance record.
* **Score ≠ money.** Score (**Paws**) comes from distance alone (1 m = 1 Paws). PawCoins are a separate shop currency — spending them never touches your score or record.
* **Star pickup** = a timed **2× score multiplier** (8 s, refreshes on pickup). Earned by skill mid-run; gives no money.
* **Difficulty = speed.** Forward speed ramps from 8 up to a cap of 16 units/s as distance grows (+0.4 per 100 m).
* **Cat Evolution.** Every 1000 m: a short full-screen flash and the cat instantly changes material/color — the run never pauses.

## Architecture Highlights

The whole game runs with **zero runtime `Instantiate`/`Destroy` calls** — everything is pooled, which keeps WebGL and mobile builds stutter-free.

| Script | Responsibility |
|---|---|
| `CatController` | Auto-forward run + distance-based speed ramp (8→16) + drag-based swerve; exposes `DistanceTraveled` (the Paws score source) |
| `ChunkManager` | Lays out road chunks, recycles the rear chunk to the front; feeds obstacle/collectible/star pools & spawn chances |
| `ChunkController` | Per-chunk spawn points; spawns obstacles / coins / rare stars, and is the SOLE pool-returner on recycle |
| `ObjectPool` | Generic reusable pool (chunks, every obstacle type, collectibles, stars) |
| `Obstacle` | Trigger → Game Over via the state manager (also used by the bomb) |
| `Collectible` / `StarPickup` | Trigger → collect; only hide themselves (`SetActive`), the chunk returns them to the pool (single-ownership) |
| `ScoreManager` | Paws (distance × star multiplier) score + PawCoin counter + best record (PlayerPrefs); drives the HUD banner & the star 2× state |
| `ScreenFlash` / `UIPulse` | Full-screen star-pickup flash / heartbeat pulse on the 2× badge |
| `GameStateManager` | Run/GameOver state, delayed any-input restart (scene reload) |
| `GameOverScreen` | Code-driven UI animation: fade-in, card pop (ease-out-back), pulsing button; shows final Paws + best |

**Fair difficulty guarantee:** the spawn system never fills every spawn point of a chunk with obstacles — there is always an escape corridor.

**Obstacle variety:** each obstacle type (trash bin, tree, bench) has its own pool; the spawner picks a random type per spawn point.

## Controls

| Action | Desktop | Mobile / Touch |
|---|---|---|
| Swerve left / right | Hold left mouse button + drag | Drag finger across the screen |
| Restart after Game Over | Any key or click | Tap anywhere |

## Getting Started

1. Install [Unity Hub](https://unity.com/download) and **Unity 6000.5.3f1** (Unity 6.5) with URP support.
2. Clone this repository and add the project folder in Unity Hub (`Add project from disk`).
3. Open `Assets/Scenes/SampleScene.unity` and press **Play**.

## Roadmap

- [x] Swerve movement system
- [x] Endless chunk-based road + object pooling
- [x] Obstacle & collectible spawn system (multi-type obstacles)
- [x] Animated Game Over card + instant restart + score HUD
- [x] Distance-based speed ramp (8 → 16 units/s)
- [x] Economy split: distance score (Paws) + PawCoin wallet + best-distance save
- [x] Star (timed 2× score multiplier) & bombs as distinct pickups (+ pickup screen flash, 2× HUD badge)
- [ ] Moving obstacles (ping-pong bikes/scooters)
- [ ] Milestone system: screen flash + material/color **Cat Evolution** (every 1000 m)
- [ ] Game feel pass (particles, audio, camera shake, chunk layout variants) + WebGL build & publish (itch.io / GitHub Pages)
- [ ] Shop: permanent **Magnet** & **Shield** upgrades (post-prototype)

## Tech & Assets

* **Engine:** Unity 6000.5.3f1 — Universal Render Pipeline (URP), C#
* **UI:** Unity UI + TextMesh Pro
* **Art packs:**
  * [SimplePoly City — Low Poly Assets](https://assetstore.unity.com) (streets, buildings, props)
  * BTM Items & Gems (coins, stars, bombs)
  * Ladymito Free Cat (cat model & animations)

---

*This is a living prototype — the design document (GDD v0.5) evolves alongside the code.* 🐈
