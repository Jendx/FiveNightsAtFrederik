using Godot;

namespace FiveNightsAtFrederik.CsScripts.Constants;

public static class NodeNames
{
    //Button
    public static readonly StringName UseAudio = new("UseAudio");

    //CameraSystem
    public static readonly StringName CameraSystemViewport = new("CameraSystemViewport");
    public static readonly StringName CameraView = new("CameraView");
    public static readonly StringName CameraSystemViewArea = new("CameraView/CameraSystemViewArea");

    // Player
    public static readonly StringName Camera = new("Camera");
    public static readonly StringName Camera_RayCast = new("RayCast");
    public static readonly StringName Camera_CarryableItemPositionMarker = new("CarryableItemPositionMarker");
    public static readonly StringName PlayerCollision = new("PlayerCollision");

    // Hud
    public static readonly StringName Crosshair = new("Crosshair");

    // CarryableItem
    public static readonly StringName PlayerInRoot = new("../player");

    // BaseEnemy
    public static readonly StringName NavigationAgent = new("NavigationAgent");
}
