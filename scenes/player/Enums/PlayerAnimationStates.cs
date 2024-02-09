using System.ComponentModel;

namespace FiveNightsAtFrederik.scenes.player.Enums;

public enum PlayerAnimationStates
{
    [Description("parameters/conditions/IsIdle")]
    Idle = 0,

    [Description("parameters/conditions/IsRunning")]
    Running = 1,

    [Description("parameters/conditions/IsDying")]
    Jumpscare = 2,

    [Description("parameters/conditions/IsPressing")]
    Press = 3,

    [Description("parameters/conditions/IsGrabbing")]
    Grab = 4,

    [Description("parameters/conditions/IsIdleArmed")]
    IdleArmed = 5,

    [Description("parameters/conditions/IsReloading")]
    Reload = 6

}