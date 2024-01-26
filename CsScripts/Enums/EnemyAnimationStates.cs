using System.ComponentModel;

namespace FiveNightsAtFrederik.CsScripts.Enums;

public enum EnemyAnimationStates
{
    [Description("parameters/conditions/isIdle")]
    Idle = 0,

    [Description("parameters/conditions/IsMoving")]
    WalkForward = 1,

    [Description("parameters/conditions/IsInJumpscareRange")]
    Jumpscare = 2
}
