using Godot;
using System;

namespace FiveNightsAtFrederik.CsScripts.Scenes.Level.Props.Camera_System;

public partial class CameraSwitchButtons : Control
{

    public override void _Ready()
    {
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton)
        {
            GD.Print("Clicked");
        }
    }
}
