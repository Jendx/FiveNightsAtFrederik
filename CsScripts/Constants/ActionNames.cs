using Godot;

namespace FiveNightsAtFrederik.CsScripts.Constants;

public static class ActionNames
{
    public static readonly StringName Use = new("Use");

    // Movement
    public static readonly StringName Move_Left = new("Move_Left");
    public static readonly StringName Move_Right = new("Move_Right");
    public static readonly StringName Move_Forward = new("Move_Forward");
    public static readonly StringName Move_Backwards = new("Move_Backwards");
}
