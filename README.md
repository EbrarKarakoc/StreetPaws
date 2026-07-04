# Street Paws 

Street Paws is a 3D hypercasual "endless runner" prototype developed using Unity. The game features an easy-to-learn swerve mechanic where players control a low-poly street cat, dodging obstacles and collecting rewards to achieve the highest score.

##  Core Gameplay & Features
*   **Swerve Mechanics:** Simple one-finger control. Swipe left or right to move the cat across lanes.
*   **Continuous Action:** The character runs forward automatically on the Z-axis. No jumping or stopping required.
*   **Risk & Reward System:**
    *    **PawCoins:** Regular collectibles (+1 Point). *(Currently disabled in the scene while tagging issues are ironed out — logic exists in `PlayerMovement.cs` but no Coin objects are tagged yet.)*
    *    **Gold Stars:** High-value collectibles placed in risky spots (+5 Points).
    *    **Bombs:** Lethal obstacles. Hitting a bomb tries to trigger its `Animator` explosion (via an `Explode` trigger parameter) and results in an instant Game Over.

##  Tech Stack
*   **Game Engine:** Unity 2022.3.30f1 (URP)
*   **Programming Language:** C#

## ▶ Getting Started
1.  Install [Unity Hub](https://unity.com/download) and Unity **2022.3.30f1** (or a compatible 2022.3 LTS patch) with the URP template support.
2.  Clone this repository.
3.  Open the project folder from Unity Hub (`Add project from disk`).
4.  Open `Assets/Scenes/SampleScene.unity` and press Play.
5.  Controls: click-and-drag (mouse) or swipe (touch) left/right to swerve.

##  Future Roadmap
This prototype serves as the foundation for the game. Planned future updates include:
*   Transitioning from an infinite runner to a structured level-based progression system.
*   A Main Menu Hub where players can spend collected PawCoins.
*   Character upgrades (speed, magnet radius, temporary bomb invincibility).
*   Unlockable cat skins and fur patterns.
*   New environments and "Boss" encounters (e.g., dodging a grumpy low-poly dog).