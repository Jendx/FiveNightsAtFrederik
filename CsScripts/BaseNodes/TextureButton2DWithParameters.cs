using FiveNightsAtFrederik.CsScripts.Models.Interfaces;
using Godot;

namespace FiveNightsAtFrederik.CsScripts.BaseNodes;

/// <summary>
/// Base class for Nodes that can be Indirectly (switch/button...) used by player
/// </summary>
[GlobalClass]
public partial class TextureButton2DWithParameters : TextureButton
{
    [Signal]
    public delegate void PressedEventHandler(TextureButton2DWithParameters pressedButton);

    protected TextureButton2DWithParameters() 
    { 
        base.Pressed += () => EmitSignal(nameof(Pressed), this);
    }
}
