using Godot;
using System;

namespace FiveNightsAtFrederik.CsScripts.Scenes.Level.Props.Camera_System;

public partial class CameraSwitchButtons : Control
{
    [Export]
    private CameraSystem cameraSystem;

    public override void _Ready() {}

    public void ChangeCamera(TextureButton pressedButton)
    {
        cameraSystem.SwitchToCamera(pressedButton.Name);
    }
}