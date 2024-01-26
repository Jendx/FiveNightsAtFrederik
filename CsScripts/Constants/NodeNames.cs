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
    public static readonly StringName Camera_CarryableItemPositionMarker = new("CarryableItemPositionMarker");
    public static readonly StringName PlayerCollision = new("PlayerCollision");
    public static readonly StringName Camera_EquipableItemPosition = new("EquipableItemPosition");

    // Hud
    public static readonly StringName Crosshair = new("Crosshair");

    // CarryableItem
    public static readonly StringName PlayerInRoot = new("../player");

    // BaseEnemy
    public static readonly StringName NavigationAgent = new("NavigationAgent");
    public static readonly StringName LookForwardPosition = new("LookForwardPosition");
    public static readonly StringName IdleTimer = new("IdleTimer");
    public static readonly StringName AnimationTree = new("AnimationTree");

    // American Bug
    public static readonly StringName BugActivationTimer = new("BugActivationTimer");

    // Generic
    public static readonly StringName AudioPlayer = new("AudioPlayer");
    public static readonly StringName RayCast = new("RayCast");

    // Gun
    public static readonly StringName DelayTimer = new("DelayTimer");
    public static readonly StringName AutomaticReloadTimer = new("AutomaticReloadTimer");
}
