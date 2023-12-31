using FiveNightsAtFrederik.CsScripts.Models.Interfaces;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveNightsAtFrederik.CsScripts.BaseNodes;

/// <summary>
/// Base class for Nodes that can be controlled by user's raycast (Basicaly Raycast replaces mouse)
/// </summary>
[GlobalClass]
public abstract partial class RaycastInteractable2DUiNode3D : Node3D
{
    protected RaycastInteractable2DUiNode3D() { }


}
