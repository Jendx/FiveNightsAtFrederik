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
    public static readonly StringName UseDelayTimer = new("UseDelayTimer");
    public static readonly StringName PlayerCollision = new("PlayerCollision");
    public static readonly StringName Camera = new("Camera");
    public static readonly StringName Camera_CarryableItemPositionMarker = new("CarryableItemPositionMarker");
    public static readonly StringName Camera_EquipableItemPosition = new("EquipableItemPosition");
    public static readonly StringName Hands = new("hands");
    public static readonly StringName Hands_Rig = new("Hands_rig");
    public static readonly StringName HandsSkeleton = new("Skeleton3D");
    public static readonly StringName Gun = new("GunBone");
    public static readonly StringName GunPosition = new("GunPosition");


    // Hud
    public static readonly StringName Crosshair = new("Crosshair");
    public static readonly StringName StaminaBar = new("StaminaBar");

    // Sight
    public static readonly StringName VisionArea = new("VisionArea");
    public static readonly StringName RecastTimer = new("RecastTimer");

    // BaseEnemy
    public static readonly StringName NavigationAgent = new("NavigationAgent");
    public static readonly StringName LookForwardPosition = new("LookForwardPosition");
    public static readonly StringName IdleTimer = new("IdleTimer");
    public static readonly StringName AnimationTree = new("AnimationTree");
    public static readonly StringName JumpscareCameraPosition = new("JumpscareCameraPosition");

    // Mr Duck
    public static readonly StringName ChaseCooldownTimer = new("ChaseCooldownTimer");

    // American Bug
    public static readonly StringName BugActivationTimer = new("BugActivationTimer");

    // Generic
    public static readonly StringName AudioPlayer = new("AudioPlayer");
    public static readonly StringName RayCast = new("RayCast");
    public static readonly StringName PlayerInRoot = new("player");
    public static readonly StringName Sight = new("Sight");

    // Gun
    public static readonly StringName DelayTimer = new("DelayTimer");
    public static readonly StringName AutomaticReloadTimer = new("AutomaticReloadTimer");
    public static readonly StringName GunBone = new("GunBone");

    // Basket
    public static readonly StringName BasketArea = new("BasketArea");
}
