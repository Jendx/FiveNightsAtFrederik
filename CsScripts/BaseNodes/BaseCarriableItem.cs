﻿using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.Scenes.Player;
using Godot;
using System.Linq;

namespace FiveNightsAtFrederik.CsScripts.BaseNodes;

public partial class BaseCarriableItem : RigidBody3D, IPlayerUsable
{
    public bool IsHeld { get; set; }

    [Export]
    public bool IsInteractionUIDisplayed { get; set; } = true;

    [Export]
    public CollisionShape3D? CollisionShape { get; set; }

    /// <summary>
    /// Sets mesh that should be used. To update call UpdateCurrentDisplayedMesh
    /// </summary>
    [Export]
    public Mesh? Mesh { get; set; }

    public delegate void PickedUpEventHandler(BaseCarriableItem item);
    public event PickedUpEventHandler OnItemPickedUp;

    private Player? player;
    protected Node? originalParent;
    protected MeshInstance3D? carriableMeshInstance;

    private Vector3 direction;
    private const float Speed = 0.001f;
    private const float MaxDistance = 1.5f;

    public override void _Ready()
    {
        player = GetTree().GetNodesInGroup(GroupNames.PlayerGroup.ToString()).FirstOrDefault() as Player ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(player)} at {NodeNames.PlayerInRoot}");

        if (GetChildren().Count == 0)
        {
            carriableMeshInstance = new() { Name = NodeNames.CarriableItemMesh.ToString() };
            AddChild(carriableMeshInstance);
        }

        carriableMeshInstance ??= this.TryGetNode<MeshInstance3D>(NodeNames.CarriableItemMesh, nameof(carriableMeshInstance));
        carriableMeshInstance.Mesh = Mesh;

        originalParent = GetParent();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsHeld)
        {
            return;
        }

        direction = (player.CarryableItemPositionMarker.GlobalPosition - GlobalPosition).Normalized();
        var distance = GlobalPosition.DistanceTo(player.CarryableItemPositionMarker.GlobalPosition);

        // To prevent "Lagging" from trying to move closer than it is possible (big Speed)
        if (distance < 0.01f)
        {
            return;
        }

        // If player is too far away from the object, stop holding object
        if (distance > MaxDistance)
        {
            Drop();
        }

        var motion = direction * (Speed + distance);
        MoveAndCollide(motion);
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustReleased(ActionNames.Use) && IsHeld)
        {
            Drop();
        }
    }

    /// <summary>
    /// Changes displayed mesh of meshInstance
    /// </summary>
    /// <param name="mesh">If null will use Mesh property</param>
    public void UpdateCurrentDisplayedMesh(Mesh? mesh = null)
    {
        carriableMeshInstance.Mesh = mesh ?? Mesh;
    }
    
    protected virtual void Drop()
    {
        Freeze = false;
        IsHeld = false;
        player.IsCarryingItem = false;

        ApplyCentralImpulse(direction);
    }

    public void OnBeginUse()
    {
        IsHeld = true;
        Freeze = true;
        player.IsCarryingItem = true;
  
        OnItemPickedUp?.Invoke(this);
    }

    public void OnEndUse()
    {
        if (!Input.IsActionJustReleased(ActionNames.Use))
        {
            return;
        }

        Drop();
    }
}