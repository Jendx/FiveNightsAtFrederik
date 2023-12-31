﻿using Godot;

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
    public static readonly StringName RayCast = new("RayCast");

    // Hud
    public static readonly StringName Crosshair = new("Crosshair");
}
