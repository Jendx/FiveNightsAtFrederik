using Godot;

namespace FiveNightsAtFrederik.CsScripts.Constants;

public static class ActionNames
{
    // Actions 
    public static readonly StringName Use = new("Use");
    public static readonly StringName Drop = new("Drop");
    public static readonly StringName Scroll_UP = new("Scroll_UP");
    public static readonly StringName Scroll_DOWN = new("Scroll_DOWN");
    public static readonly StringName Submit = new("Submit");

    // Movement
    public static readonly StringName Move_Left = new("Move_Left");
    public static readonly StringName Move_Right = new("Move_Right");
    public static readonly StringName Move_Forward = new("Move_Forward");
    public static readonly StringName Move_Backwards = new("Move_Backwards");
    public static readonly StringName Crouch = new("Crouch");
    public static readonly StringName Sprint = new("Sprint");
    
    public static readonly StringName DEBUG_TOGGLEMOUSE = new("Debug_ToggleMouse");
}
