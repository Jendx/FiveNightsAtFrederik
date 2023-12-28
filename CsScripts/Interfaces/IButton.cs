using Godot;

namespace FiveNightsAtFrederik.CsScripts.Interfaces;

public interface IButton : IUsableNode
{
    public bool IsToggle { get; set; }

    public float DelayLength { get; set; }

    public Node3D UsableNode { get; set; }
}
