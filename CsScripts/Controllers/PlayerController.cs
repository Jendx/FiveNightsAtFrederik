using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.Scenes.Player;
using Godot;

namespace FiveNightsAtFrederik.CsScripts.Controllers;

public class PlayerController
{
    private readonly Player player;
    private Vector3 velocity = new();

    private GodotObject colidingObject;
    private IPlayerUsable usableObject;
    private readonly float cameraOffset;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    private readonly float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public PlayerController(Player player)
    {
        this.player = player;
        cameraOffset = player.Camera.Position.Y - player.CollisionMesh.Position.Y;
    }

    /// <summary>
    /// Handles movement on XY Axis
    /// </summary>
    public void HandleMovement(double delta)
    {
        Vector2 inputDir = Input.GetVector(ActionNames.Move_Left, ActionNames.Move_Right, ActionNames.Move_Forward, ActionNames.Move_Backwards);
        Vector3 direction = (player.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * player.MovementSpeed;
            velocity.Z = direction.Z * player.MovementSpeed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(player.Velocity.X, 0, player.MovementSpeed);
            velocity.Z = Mathf.MoveToward(player.Velocity.Z, 0, player.MovementSpeed);
        }

        float fallingVelocity = 0f;
        if (!player.IsOnFloor())
        {
            fallingVelocity = velocity.Y - gravity * (float)delta;
        }

        velocity.Y = fallingVelocity;

        player.Velocity = velocity;
        player.MoveAndSlide();
    }

    /// <summary>
    /// Rotates Player model & camera
    /// </summary>
    /// <param name="mouseDelta">new rotation</param>
    /// <param name="camera">Player camera</param>
    public void RotateByMouseDelta(Vector2 mouseDelta, Camera3D camera)
    {
        // Rotate the player around the Y-axis (left and right)
        player.RotateY(-mouseDelta.X * player.RotationSpeed);

        // Rotate the camera around the X-axis (up and down)
        camera.RotateX(-mouseDelta.Y * player.RotationSpeed);

        // Clamp the camera's rotation so you can't look too far up or down
        float cameraRotation = Mathf.Clamp(camera.RotationDegrees.X, -70, 70);
        camera.RotationDegrees = new Vector3(cameraRotation, 0, 0);
    }

    public void UpdateLookAtObject(RayCast3D rayCast)
    {
        var newColidingObject = rayCast.GetCollider();
        if (colidingObject != newColidingObject && Input.IsActionPressed(ActionNames.Use))
        {
            StopUsing();
        }

        colidingObject = newColidingObject;

        var isValidObject = colidingObject is not null;
        if (isValidObject)
        {
            player.EmitSignal(nameof(player.OnRaycastColide), newColidingObject);
        }

        // Sometimes the colision can be on child of the node. 
        usableObject = colidingObject is IPlayerUsable playerUsable
            ? playerUsable
            : ((Node)colidingObject)?.Owner as IPlayerUsable;

        if (!(isValidObject && usableObject is not null))
        {
            usableObject = null;
            player.EmitSignal(nameof(player.UsableObjectChanged), false);
            return;
        }

        player.EmitSignal(nameof(player.UsableObjectChanged), usableObject.isInteractionUIDisplayed);
    }

    public void Use() => usableObject?.OnBeginUse();

    public void StopUsing() => usableObject?.OnEndUse();

    public void HandleCrouch(double delta)
    {
        float targetHeight;
        float currentHeight = default;

        if (Input.IsActionPressed(ActionNames.Crouch))
        {
            targetHeight = player.CrouchHeight;
            player.isCrouching = true;
        }
        else
        {
            targetHeight = player.StandHeight;
            player.isCrouching = false;
        }

        if (!Mathf.IsEqualApprox(currentHeight, targetHeight, 0.1f))
        {
            currentHeight = Mathf.Lerp(player.CollisionMesh.Scale.Y, targetHeight, player.StandSpeed * (float)delta);
            player.CollisionMesh.Scale = new Vector3(player.CollisionMesh.Scale.X, currentHeight, player.CollisionMesh.Scale.Z);
            player.Camera.Position = new Vector3(player.Camera.Position.X, (currentHeight + cameraOffset), player.Camera.Position.Z);
        }
    }
}
