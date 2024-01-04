using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;
using System;

namespace FiveNightsAtFrederik.Scenes.Player.PickableItems;

public partial class CarryableItem : RigidBody3D, IPlayerUsable
{
    private const float Speed = 0.001f;
    private const float MaxDistance = 50;
    
    [Export]
    private Player player;

    private bool isHeld;

    public override void _Ready()
    {
        player = GetNode<Player>(NodeNames.PlayerInRoot.ToString());

        if (player is null)
        {
            GD.PrintErr($"{Name} did not find player node");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if(!isHeld)
        {
            return;
        }

        var direction = (player.CarryableItemPositionMarker.GlobalPosition - GlobalPosition).Normalized();
        var distance = GlobalPosition.DistanceTo(player.CarryableItemPositionMarker.GlobalPosition);

        if (distance < 0.01f )
        {
            return;
        }

        if (distance > MaxDistance ) 
        {
            GlobalPosition = player.CarryableItemPositionMarker.GlobalPosition;
        }
        
        var motion = direction * (Speed + distance);
        MoveAndCollide(motion, maxCollisions: 1);
    }

    public void OnBeginUse()
    {
        isHeld = true;
        Freeze = false;
    }

    public void OnEndUse()
    {
        if (!Input.IsActionJustReleased(ActionNames.Use))
        {
            return;
        }

        isHeld = false;

        ApplyForceBasedOnPlayerMovementAndRotation();
    }

    public void ApplyForceBasedOnPlayerMovementAndRotation()
    {
        // Get the player's linear velocity
        Vector3 linearVelocity = player.Velocity;

        // Get the mouse's movement since the last frame
        Vector2 mouseDelta = Input.GetLastMouseVelocity().Normalized();

        // Convert the mouse's movement to a Vector3 and use it as the rotation vector
        // Invert Mouse.X velocity if looking backwards (-90 <= Y <= 90  is looking in Front) to prevent throwing into opposite direction
        // Invert Mouse.Y to fix throw direction (Defaultly inverted)
        // Z is how far it will go In front of the player
        Vector3 rotationVector = new Vector3(
            player.GlobalRotationDegrees.Y <= 90 && player.GlobalRotationDegrees.Y >= -90 ? mouseDelta.X : -mouseDelta.X, 
            -mouseDelta.Y,
            0);

        // Apply the force to the PickableItem
        this.ApplyCentralImpulse(rotationVector * 3f + linearVelocity );
    }
}
