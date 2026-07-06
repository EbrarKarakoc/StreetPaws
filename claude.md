# Project Overview: "Cat Swerve" (Hypercasual 3D Runner)

## Game Description
This is a 3D hypercasual continuous runner game. The player controls a street cat moving continuously forward on a road. There is NO jumping and NO stopping. The only control mechanic is "Swerve" (moving left and right using mouse drag or touch swipe) to dodge obstacles and collect items. 

## Technical Details
- **Engine:** Unity (Universal Render Pipeline - URP)
- **Target Platform:** WebGL (for pitch demo) & Mobile (Single finger control)
- **Camera:** The Main Camera is currently a child of the Player object, following it automatically from behind and slightly above.

## Current Scene Hierarchy & Transform Data
1. **Player (Capsule + 3D Cat Model Child)**
   - Position: (X: 0, Y: 1, Z: 0)
   - Note: The player must stay strictly on the road. X-axis movement should be clamped (e.g., between -4 and 4) so the cat doesn't fall off the edges.
2. **Road (Cube)**
   - Position: (X: 0, Y: 0, Z: 0)
   - Scale: (X: 10, Y: 1, Z: 200) 
3. **Obstacles (Empty GameObject)**
   - Position: (X: 0, Y: 0, Z: 0)
   - Contains static obstacles (trash cans, trees) placed along the Z-axis at various lanes (X: -3, 0, 3).

## Roadmap & Upcoming Features (DO NOT implement all at once, wait for user prompt)
- **Phase 1 (Current):** Smooth Swerve Movement System (Forward constant speed + X-axis swipe control).
- **Phase 2:** Obstacle Collision & Game Over state.
- **Phase 3:** Collectibles (Paw-shaped coins) and a Top UI Bar to track score.
- **Phase 4:** Moving Obstacles (Ping-pong movement on X-axis like bikes/scooters).
- **Phase 5:** Level Management (5 prototype levels with increasing difficulty and a "Cat Evolution" reward at level 5).

## Coding Guidelines for AI Assistant
- Write clean, highly optimized C# scripts suitable for hypercasual mobile/WebGL games.
- Do NOT use `Update()` for physics calculations; use `FixedUpdate()` if Rigidbodies are involved, but prefer simple `Transform.Translate` for this specific hypercasual feel unless physics collisions dictate otherwise.
- Always add brief comments explaining the logic.
- When providing code, specify exactly which GameObject the script should be attached to in the Unity Editor.