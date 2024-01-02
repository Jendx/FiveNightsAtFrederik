using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;

namespace FiveNightsAtFrederik.Scenes.Player.PickableItems;

public partial class CarryableItem : RigidBody3D, IPlayerUsable
{
    [Export]
    private Player player;

    public void OnBeginUse()
    {
        Freeze = true;
        CanSleep = false;
        Reparent(player.Camera);
    }

    public void OnEndUse()
    {
        if (!Input.IsActionJustReleased(ActionNames.Use))
        {
            return;
        }

        Freeze = false;
        Sleeping = false;

        ApplyForceBasedOnPlayerMovementAndRotation();
        Reparent(player.GetParent());
    }

    public void ApplyForceBasedOnPlayerMovementAndRotation()
    {
        // Get the player's linear velocity
        Vector3 linearVelocity = player.Velocity;

        // Get the mouse's movement since the last frame
        Vector2 mouseDelta = Input.GetLastMouseVelocity().Normalized();

        // Convert the mouse's movement to a Vector3 and use it as the rotation vector
        // Invert Mouse.X velocity if looking bacwards (-90 <= Y <= 90  is looking in Front) to prevent throwing into opposit direction
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
