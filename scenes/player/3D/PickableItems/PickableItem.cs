using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;
using System;

namespace FiveNightsAtFrederik.Scenes.Player.PickableItems;

public partial class PickableItem : RigidBody3D, IPlayerUsable
{
    private CharacterBody3D player;
    private Node originalParent;

    public override void _Ready()
    {
        originalParent = GetParent();
    }

    public void OnBeginUse()
    {
        Reparent(player);
    }

    public void OnEndUse()
    {
        Reparent(originalParent);
    }
}
