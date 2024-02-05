namespace FiveNightsAtFrederik.scenes.Enemy.MrDuck.BehaviourState.Enums;

/// <summary>
/// Holds also speeds because these enums would be same
/// </summary>
public enum MrDuckBehaviourStates
{
    Roam = 32,

    // Should be higher than player walk speed, but lower than sprint
    Chase = 250,
}
