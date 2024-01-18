using System.ComponentModel;

namespace FiveNightsAtFrederik.scenes.Enemy.MrDuck;

internal enum MrDuckAnimations
{
    [Description("parameters/conditions/isIdle")]
    Idle = 0,

    [Description("parameters/conditions/IsMoving")]
    WalkForward = 1,

    [Description("parameters/conditions/IsInJumpscareRange")]
    Jumpscare = 2
}
