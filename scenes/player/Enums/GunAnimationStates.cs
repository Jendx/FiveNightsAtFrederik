using System.ComponentModel;

namespace FiveNightsAtFrederik.scenes.player.Enums;

public enum GunAnimationStates
{
    [Description("parameters/conditions/IsIdle")]
    Idle = 0,

    [Description("parameters/conditions/IsReloading")]
    Reload = 1,

    [Description("parameters/conditions/IsShooting")]
    Shoot = 2

}