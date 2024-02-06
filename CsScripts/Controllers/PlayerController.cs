using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.scenes.player.Enums;
using FiveNightsAtFrederik.Scenes.Player;
using Godot;
using System.Linq;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.BaseNodes;

namespace FiveNightsAtFrederik.CsScripts.Controllers;

#nullable enable

public class PlayerController
{
    private readonly Player player;
    private Vector3 velocity = new();
    private IPlayerUsable? usableObject;
    private PlayerAnimationStates nextAnimation;
    private readonly string[] forbiddenSprintPressedActions = new[] { ActionNames.Move_Left.ToString(), ActionNames.Move_Backwards.ToString(), ActionNames.Move_Right.ToString() };
    

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

        // if player is not moving set next animation to Idle
        if (inputDir == Vector2.Zero)
        {
            nextAnimation = PlayerAnimationStates.Idle;
        }

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
        var collidingObject = rayCast.GetCollider();

        var isValidObject = collidingObject is not null;
        if (isValidObject)
        {
            player.EmitSignal(nameof(player.OnRaycastCollide), collidingObject);
        }

        // Sometimes the collision can be on child of the node.
        var newUsableObject = collidingObject.TryConvertTo<IPlayerUsable>();


        // If player looks away from the usableObject and is still interacting with the usableObject stop using it
        if (newUsableObject is null
            && usableObject is not null
            && player.IsUsing)
        {
            StopUsing();
        }

        var hudCrosshairState = GetHudCrosshairTexture(newUsableObject);
        if (!(isValidObject && newUsableObject is not null))
        {
            usableObject = null;
            player.UpdateCrosshairState(hudCrosshairState);
            return;
        }

        usableObject = newUsableObject;
        player.UpdateCrosshairState(hudCrosshairState);
    }

    private HudCrosshairStates GetHudCrosshairTexture(IPlayerUsable playerUsable)
    {
        var hudCrosshairState = HudCrosshairStates.Point;
        if (playerUsable is not null && playerUsable.IsInteractionUIDisplayed)
        {
            hudCrosshairState = HudCrosshairStates.Use;
        }

        if (player.IsHoldingItem)
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
            player.CurrentStateSpeed = PlayerSpeeds.Crouch;
            nextAnimation = PlayerAnimationStates.Idle;
        }

        if (!Mathf.IsEqualApprox(currentHeight, targetHeight, 0.1f))
        {
            currentHeight = Mathf.Lerp(player.CollisionMesh.Scale.Y, targetHeight, (float)PlayerSpeeds.CrouchTransition * (float)delta);
            player.CollisionMesh.Scale = new Vector3(player.CollisionMesh.Scale.X, currentHeight, player.CollisionMesh.Scale.Z);
        }
    }

    /// <summary>
    /// Player can sprint if current stamina is > 0
    /// If player depletes stamina, he won't be able to sprint until the stamina is bigger than (float)SprintThresholds.Middle
    /// </summary>
    public void HandleSprint()
    {
        if (player.CurrentStamina <= (float)SprintThresholds.Min)
        {
            player.CanSprint = false;
        }

        // If player had depleted stamina & it finally recharged above Middle threshold Make him available to sprint again
        if (!player.CanSprint && player.CurrentStamina >= (float)SprintThresholds.Middle)
        {
            player.CanSprint = true;
        }

        // If player can't sprint. He must have depleted stamina => Slower movement speed punishment
        if (!player.CanSprint)
        {
            player.CurrentStateSpeed = PlayerSpeeds.ExhaustedWalk;
        }

        // Player can sprint if he is not carrying anything, is moving forward, is not moving backwards and is holding sprint button
        if (
            Input.IsActionPressed(ActionNames.Sprint)
            && Input.IsActionPressed(ActionNames.Move_Forward)
            && !Input.IsActionPressed(ActionNames.Move_Backwards)
            && !player.IsCarryingItem
            && player.CanSprint)
        {
            player.CurrentStateSpeed = PlayerSpeeds.Sprint;
            nextAnimation = PlayerAnimationStates.Running;
            player.CurrentStamina -= player.StaminaDrainRate;

            return;
        }

        const float rechargeRate = 0.2f;
        player.CurrentStamina += player.CurrentStamina < (float)SprintThresholds.Low ? rechargeRate : rechargeRate + 0.1f;
        nextAnimation = PlayerAnimationStates.Idle;
    }

    /// <summary>
    /// Set's state of PlayerHandAnimation
    /// </summary>
    public void UpdateHandAnimation()
    {
        if (player.CurrentAnimation == nextAnimation)
        {
            return;
        }

        player.AnimationTree.Set(player.CurrentAnimation.GetDescription(), false);

        player.CurrentAnimation = nextAnimation;

        player.AnimationTree.Set(player.CurrentAnimation.GetDescription(), true);
    }

    /// <summary>
    /// Tries to use object.
    /// </summary>
    /// <returns>If the object is not usable returns false</returns>
    public void TryUse()
    {
        player.IsUsing = usableObject is not null;
        if (!player.IsUsing)
        {
            return;
        }

        usableObject!.OnBeginUse();
        
        var isCarriableObject = usableObject is BaseCarriableItem;
        var isHoldableObject = usableObject is BaseHoldableItem;
        if (!isHoldableObject && !isCarriableObject)
        {
            nextAnimation = PlayerAnimationStates.Press;
            player.useDelayTimer.Start();

            return;
        }

        if (isCarriableObject)
        {
            GD.Print("Carrying");
            nextAnimation = PlayerAnimationStates.Grab;
            player.useDelayTimer.Start();
        }
    }

    public void StopUsing() {
        usableObject?.OnEndUse();
        player.IsUsing = false;
    }
}
