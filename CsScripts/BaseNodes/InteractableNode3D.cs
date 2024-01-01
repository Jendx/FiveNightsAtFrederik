using FiveNightsAtFrederik.CsScripts.Models.Interfaces;
using Godot;

namespace FiveNightsAtFrederik.CsScripts.BaseNodes;

/// <summary>
/// Base class for Nodes that can be Indirectly (switch/button...) used by player
/// </summary>
[GlobalClass]
public abstract partial class InteractableNode3D : Node3D
{
    protected InteractableNode3D() { }

    public abstract void OnBeginUse<TParameters>(TParameters parameters) where TParameters : BaseUsableParameters;

    public abstract void OnEndUse<TParameters>(TParameters parameters) where TParameters : BaseUsableParameters;
}
