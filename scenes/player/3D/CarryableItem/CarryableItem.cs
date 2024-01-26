using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;
using System;

namespace FiveNightsAtFrederik.Scenes.Player;

public partial class CarryableItem : RigidBody3D, IPlayerUsable
{
    private const float Speed = 0.001f;
    private const float MaxDistance = 1.5f;
    private Player player;

    [Export]
    public bool IsInteractionUIDisplayed { get; set; } = true;

    private Vector3 direction;
    private bool isHeld;

    public override void _Ready()
    {
        player = GetNode<Player>(NodeNames.PlayerInRoot.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(player)} at {NodeNames.PlayerInRoot}"); ;
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
