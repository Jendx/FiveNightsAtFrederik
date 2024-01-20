using Godot;
using System.Linq;

namespace FiveNightsAtFrederik.CsScripts.Scenes.Level.Props.Camera_System;

/// <summary>
/// Handles control button's button pressed events
/// Needs Camera system with "View" in name.
/// </summary>
public partial class CameraSwitchButtons : Control
{
    private CameraSystem cameraSystem;

    public override void _Ready()
    {
        cameraSystem = GetTree().CurrentScene.GetChildren().FirstOrDefault(p => p is CameraSystem && p.Name.ToString().Contains("View")) as CameraSystem ??
            throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(cameraSystem)} with name containing View");
    }

    public void ChangeCamera(TextureButton pressedButton)
    {
        cameraSystem.SwitchToCamera(pressedButton.Name);
    }
}
