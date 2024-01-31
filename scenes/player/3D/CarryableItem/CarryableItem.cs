using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;
using System;
using System.Linq;

namespace FiveNightsAtFrederik.Scenes.Player;

[GlobalClass]
public partial class CarryableItem : RigidBody3D, IPlayerUsable
{
    public bool isHeld { get; set; }

    [Export]
    public bool IsInteractionUIDisplayed { get; set; } = true;

    [Export]
    public CollisionShape3D CollisionShape { get; set; }

    [Export]
    public MeshInstance3D MeshInstance { get; set; }

    private Vector3 direction;
    private Player player;
    private const float Speed = 0.001f;
    private const float MaxDistance = 1.5f;

    public override void _Ready()
    {
        player = GetTree().GetNodesInGroup(GroupNames.playerGroup.ToString()).FirstOrDefault() as Player ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(player)} at {NodeNames.PlayerInRoot}"); ;
    }

    public override void _PhysicsProcess(double delta)
    {
        if(!isHeld)
        {
            return;
        }

        direction = (player.CarryableItemPositionMarker.GlobalPosition - GlobalPosition).Normalized();
        var distance = GlobalPosition.DistanceTo(player.CarryableItemPositionMarker.GlobalPosition);

        // To prevent "Lagging" from trying to move closer than it is possible (big Speed)
        if (distance < 0.01f )
        {
            return;
        }

        // If player is too far away from the object, stop holding object
        if (distance > MaxDistance ) 
        {
            Drop();
        }

        var motion = direction * (Speed + distance);
        MoveAndCollide(motion);
    }

    private void Drop()
    {
        Freeze = false;
        isHeld = false;

        ApplyCentralImpulse(direction);
    }

    public void OnBeginUse()
    {
        isHeld = true;
        Freeze = true;
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
