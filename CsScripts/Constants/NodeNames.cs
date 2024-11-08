﻿using Godot;

namespace FiveNightsAtFrederik.CsScripts.Constants;

public static class NodeNames
{
    // Generic
    public static readonly StringName AudioPlayer = new("AudioPlayer");
    public static readonly StringName RayCast = new("RayCast");
    public static readonly StringName PlayerInRoot = new("player");
    public static readonly StringName Sight = new("Sight");

    // Button
    public static readonly StringName UseAudio = new("UseAudio");
    
    // CarriableItem
    public static readonly StringName CarriableItemMesh = new("CarriableItemMesh");

    // CameraSystem
    public static readonly StringName CameraSystemViewport = new("CameraSystemViewport");
    public static readonly StringName CameraView = new("CameraView");
    public static readonly StringName CameraSystemViewArea = new("CameraView/CameraSystemViewArea");

    // Player
    public static readonly StringName UseDelayTimer = new("UseDelayTimer");
    public static readonly StringName PlayerCollision = new("PlayerCollision");
    public static readonly StringName Camera = new("Camera");
    public static readonly StringName Camera_CarryableItemPositionMarker = new("CarryableItemPositionMarker");
    public static readonly StringName Camera_EquipableItemPosition = new("EquipableItemPosition");
    public static readonly StringName Camera_GunPosition = new("hands/Hands_rig/Skeleton3D/GunBone/EquipableItemPosition");
    public static readonly StringName Camera_BasketPosition = new("hands/Hands_rig/Skeleton3D/GunBone/EquipableBasketPosition");
    public static readonly StringName Camera_Hud = new("Hud");

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

    // Gun
    public static readonly StringName DelayTimer = new("DelayTimer");
    public static readonly StringName AutomaticReloadTimer = new("AutomaticReloadTimer");
    public static readonly StringName GunBone = new("GunBone");

    // Basket
    public static readonly StringName BasketArea = new("BasketArea");

    // Minigames
    public static readonly StringName MinigameCamera = new("MinigameCamera");
    public static readonly StringName MinigameInteractionCollision = new("MinigameInteractionBody/MinigameInteractionCollision");
    public static readonly StringName MinigameHud = new("MinigameCamera/MinigameHud");

    // Soda
    public static readonly StringName Mug = new("Mug");
    public static readonly StringName Drink = new("Mug/Drink");
    public static readonly StringName Foam = new("Mug/Drink/Foam");
    public static readonly StringName FoamRaiseTimer = new("FoamRaiseTimer");
    public static readonly StringName FoamShrinkingTimer = new("FoamShrinkingTimer");
    public static readonly StringName DrinkTopLimitArea = new("DrinkTopLimitArea");
    public static readonly StringName DrinkWinLimitArea = new("DrinkWinLimitArea");
    public static readonly StringName DrinkSpawner = new("DrinkSpawner");

    // Pizza
    public static readonly StringName Dough = new("Dough");
    public static readonly StringName TargetMesh = new("TargetMesh");
    public static readonly StringName TargetArea = new("TargetMesh/TargetArea");
    public static readonly StringName Ingredient = new("Ingredient");
    public static readonly StringName PizzaSpawner = new("PizzaSpawner");
    public static readonly StringName FailureArea = new("FailureArea");
    public static readonly StringName AddIngredientArea = new("AddIngredientArea");
    public static readonly StringName SelectedIngredientText = new("MinigameCamera/MinigameHud/SelectedIngredientText");
    public static readonly StringName NoIngredientsLabel = new("MinigameCamera/MinigameHud/NoIngredientsLabel");

    // Pizza expedition
    public static readonly StringName SubmitArea = new("SubmitArea");
    public static readonly StringName ReparentPositionMarker = new("ReparentPositionMarker");

    // Win Screen
    public static readonly StringName TotalValue = new("ColorRect/TotalValueDisplay");
    public static readonly StringName WinLabel = new("ColorRect/WinLabel");
}
