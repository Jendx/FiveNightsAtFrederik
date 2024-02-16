using System.ComponentModel;

namespace FiveNightsAtFrederik.CsScripts.Enums;

public enum CollisionLayers
{
    PlayerCollideable = 1,
    RayCastCollideable = 2,
    CarryableItemCollideable = 3,
    EnemyRaycastCollideable = 4,

    /// <summary>
    /// Should be used only on minigame areas
    /// </summary>
    MinigameAreaCollideable = 5,
}
