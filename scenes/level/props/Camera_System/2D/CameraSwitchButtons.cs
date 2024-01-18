using Godot;

namespace FiveNightsAtFrederik.CsScripts.Scenes.Level.Props.Camera_System;

public partial class CameraSwitchButtons : Control
{
    [Export]
    private CameraSystem cameraSystem;

    public void ChangeCamera(TextureButton pressedButton)
    {
        cameraSystem.SwitchToCamera(pressedButton.Name);
    }
}
