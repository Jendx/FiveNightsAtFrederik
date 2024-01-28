using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.scenes.player;
using FiveNightsAtFrederik.Scenes.Player;
using Godot;

namespace FiveNightsAtFrederik.CsScripts.Controllers;

public class PlayerController
{
    private readonly Player player;
    private Vector3 velocity = new();

    private GodotObject colidingObject;
    private IPlayerUsable usableObject;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    private readonly float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public PlayerController(Player player)
    {
        this.player = player;
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
            velocity.X = direction.X * player.MovementSpeed * (float)delta;
            velocity.Z = direction.Z * player.MovementSpeed * (float)delta;
        }
        else
        {
            velocity.X = Mathf.MoveToward(player.Velocity.X, 0, player.MovementSpeed * (float)delta);
            velocity.Z = Mathf.MoveToward(player.Velocity.Z, 0, player.MovementSpeed * (float)delta);
        }

        float fallingVelocity = 0f;
        if (!player.IsOnFloor())
        {
            fallingVelocity = velocity.Y - gravity;
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

        var hudCrosshairState = GetHudCrosshairTexture();

        if (!(isValidObject && usableObject is not null))
        {
            usableObject = null;
            player.UpdateCrosshairState(hudCrosshairState);
            return;
        }

        player.UpdateCrosshairState(hudCrosshairState);
    }

    private HudCrosshairStates GetHudCrosshairTexture()
    {
        var hudCrosshairState = HudCrosshairStates.Point;
        if (usableObject is not null && usableObject.IsInteractionUIDisplayed)
        {
            hudCrosshairState = HudCrosshairStates.Use;
        }

        if (player.IsHoldingWeapon)
        {
            hudCrosshairState = HudCrosshairStates.Aim;
        }

        return hudCrosshairState;
    }

    public void HandleCrouch(double delta)
    {
        float targetHeight = player.StandHeight;
        float currentHeight = default;

        if (Input.IsActionPressed(ActionNames.Crouch))
        {
            targetHeight = player.CrouchHeight;
            player.CurrentStateSpeed = PlayerStateSpeeds.Crouch;
        }

        if (!Mathf.IsEqualApprox(currentHeight, targetHeight, 0.1f))
        {
            currentHeight = Mathf.Lerp(player.CollisionMesh.Scale.Y, targetHeight, (float)PlayerStateSpeeds.CrouchTransition * (float)delta);
            player.CollisionMesh.Scale = new Vector3(player.CollisionMesh.Scale.X, currentHeight, player.CollisionMesh.Scale.Z);
        }
    }

    /// <summary>
    /// Player can sprint if current stamina is > 0
    /// If player depleats stamina, he won't be able to sprint until the stamina is bigger than (float)SprintTresholds.Middle
    /// </summary>
    public void HandleSprint()
    {
        if (player.CurrentStamina <= (float)SprintTresholds.Min)
        {
            player.CanSprint = false;
        }

        if (!player.CanSprint && player.CurrentStamina >= (float)SprintTresholds.Middle)
        {
            player.CanSprint = true;
        }

        if (
            Input.IsActionPressed(ActionNames.Sprint)
            && !player.Velocity.IsZeroApprox()
            && player.CanSprint)
        {
            player.CurrentStateSpeed = PlayerStateSpeeds.Sprint;
            player.CurrentStamina -= player.StaminaDrainRate;

            return;
        }

        const float rechargeRate = 0.2f;
        player.CurrentStamina += player.CurrentStamina < (float)SprintTresholds.Low ? rechargeRate : rechargeRate + 0.1f;
    }

    public void Use() => usableObject?.OnBeginUse();

    public void StopUsing() => usableObject?.OnEndUse();
}
