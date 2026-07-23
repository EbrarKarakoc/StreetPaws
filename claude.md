# Project Overview: "StreetPaws" (Hypercasual 3D Endless Runner)

## Game Description
StreetPaws is a 3D hypercasual **endless runner** (Subway Surfers model). A street cat runs forward continuously — the run NEVER stops, there is NO jumping, NO ducking and **NO level system**. The only control is "Swerve": free X-axis sliding via mouse drag or one-finger swipe to dodge obstacles and collect items. The single goal is beating your own **distance record**. At every 1000 m milestone a short full-screen flash (~0.3 s) plays and the cat visually evolves **without interrupting the run**.

**Design source of truth:** GDD (Turkish, **v0.5**), kept on the maintainer's Google Drive (NOT in this repo). Workflow: design changes go into the GDD first, then get implemented. Keep the two in sync.

## Technical Details
- **Engine:** Unity 6000.5.3f1 (Unity 6.5, URP)
- **Target Platform:** WebGL (pitch demo) & mobile browsers (single-finger control)
- **Camera (FINAL decision):** Main Camera stays a child of the Player object (behind + slightly above). Validated: smooth, no motion sickness. Do NOT replace it with a follow script; the camera moving laterally with the swerve is intentional.

## Core Design Rules (from GDD v0.4 — decided, do not re-litigate)
1. **Endless run, no win state.** Difficulty comes from a distance-based speed ramp: forward speed starts at 8 units/s, +0.4 per 100 m, capped at 16 (initial values; tune via playtesting).
2. **Score and currency are fully separate.**
   - Score (**Paws**) = distance traveled (1 m = 1 Paws). Best distance (record) stored in PlayerPrefs.
   - Currency (**Pati Altını** / coins, +1 each) is spent only in the shop. Spending NEVER affects score or record ("records can't be bought").
3. **Star** = rare pickup: 2× Paws per meter for 8 s (new star refreshes the timer, no stacking). Gives NO money.
4. **Cat Evolution** = purely visual **material/color swap** on the same cat model at each milestone, triggered with the flash. Hitbox, speed and controls never change; no pause, no cutscene, no reward screen.
5. **Shop (post-prototype):** permanent level-based upgrades only — Magnet 5/7/10 s (150/400/1000 gold), Shield 1/2 hits (200/600 gold). No consumables, no in-run power-up pickups.

## Current Architecture (Assets/Scripts) & Scene
- `CatController` — ACTIVE player controller on `player`: distance-based speed ramp (baseSpeed 8 → maxSpeed 16, +0.4 per 100 m) + swerve (swerveSpeed 30, max 1.2/frame, X clamped to ±3). Exposes `DistanceTraveled`, read by ScoreManager as the Paws score.
- `ChunkManager` (on `ChunkManager`) + `ChunkController` (on chunk prefab) — endless road: 5 pooled chunks × 20 m, 6 spawn points each; per point 30% obstacle / 40% collectible / 30% empty (Inspector-tunable). Fair-difficulty guarantee in code: a chunk can never be all obstacles.
- `ObjectPool` — generic pool: chunks, obstacle variants (`ObstaclePool`, `TreePool`, `BenchPool`, + bomb via the obstacle array), `CollectiblePool` (PawCoin) and `StarPool` (rare star). No Instantiate/Destroy during a run — keep it that way.
- `Collectible` / `StarPickup` / `Obstacle` — trigger-based; identify the cat via the `CatController` component (NOT tags). On pickup a Collectible/Star only HIDES itself (`SetActive(false)`); the owning `ChunkController.ClearSpawns` is the SOLE pool-returner (single-ownership → prevents cross-chunk aliasing). Bomb reuses `Obstacle` (contact → game over).
- `ScoreManager` (on `GameFlow`) — economy split DONE: Paws score (distance × star multiplier), separate PawCoin counter, best record (PlayerPrefs). Drives the HUD banner; owns the star 2× multiplier (8 s) and triggers `ScreenFlash` + `MultiplierBadge`.
- `GameStateManager` (on `GameFlow`) + `GameOverScreen` (on `GameOverPanel`) — game over flow: 0.8 s input lock, animated card (shows final Paws + best + optional new-record badge), any input reloads the scene.
- `ScreenFlash` (full-screen flash — star pickup now, milestone flash later), `UIPulse` (heartbeat pulse on the 2× badge), `StarPickup` (star → activates the multiplier).
- **LEGACY REMOVED:** `PlayerMovement` and `GameFlowManager` deleted (old prototype controller/flow, fully superseded).

## Roadmap (mirror of GDD section 10 — implement ONE stage per user prompt, never ahead)
1. ✔ Swerve movement system (CatController)
2. ✔ Endless road: chunk system + object pooling
3. ✔ Obstacle/collectible spawn system + obstacle variety
4. ✔ Game Over card + score HUD + instant restart
5. ✔ Distance-based speed ramp (CatController)
6. ✔ Economy split: distance score (Paws) + coin counter + record save (PlayerPrefs) + legacy script cleanup
7. ✔ Star (timed 2× score multiplier) & bomb (+ star pickup screen flash, 2× HUD badge)
8. ☐ **NEXT:** Moving obstacles (X-axis ping-pong)
9. ☐ Milestone system: flash effect + material/color evolution (reuse ScreenFlash)
10. ☐ Game feel (particles, audio, camera shake — incl. star pickup "explosion" polish) + WebGL build & publish
11. ☐ Shop: magnet, shield (post-prototype)

## Coding Guidelines for AI Assistant
- Write clean, highly optimized C# suitable for hypercasual WebGL/mobile.
- Do NOT use `Update()` for physics calculations; use `FixedUpdate()` if Rigidbodies are involved, but prefer simple Transform movement for this game's feel (current scripts are transform-driven with kinematic-safe triggers).
- Object pooling is mandatory for anything spawned during a run.
- Always add brief comments explaining the logic — existing code comments are in **Turkish**; keep that style. **Exception — Inspector-facing text is English:** `[Header(...)]` labels (and similar Unity UI strings) are written in English to match the English Unity editor and keep the Inspector readable. In short: code comments in Turkish, Inspector labels in English.
- When providing code, specify exactly which GameObject the script should be attached to in the Unity Editor.
- Brainstorm/discuss first; only write code or create files after an explicit "do it" instruction from the user (respond in Turkish).
