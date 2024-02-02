using Godot;
using FiveNightsAtFrederik.CsScripts.Controllers;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Enums;
using System;
using FiveNightsAtFrederik.scenes.player.Enums;
using FiveNightsAtFrederik.CsScripts.Extensions;

namespace FiveNightsAtFrederik.Scenes.Player;

public partial class Player : CharacterBody3D, IMovableCharacter
{
	[Export]
	public float JumpVelocity { get; set; } = 5f;

	[Export]
	public float RotationSpeed { get; set; } = 0.01f;

	[Export]
	public float StandHeight { get; set; } = 1f;

	[Export]
	public float CrouchHeight { get; set; } = 0.2f;

	[Export]
	public float StaminaDrainRate { get; set; } = 0.4f;

	public Camera3D Camera { get; set; }
	public CollisionShape3D CollisionMesh { get; set; }
	public Marker3D CarryableItemPositionMarker { get; set; }
	public Marker3D EquipableItemPositionMarker { get; set; }
    public AnimationTree AnimationTree { get; set; }

    // Wait time must be equal to the animationTransitionTime (0.6 currently to transition from and back pressing)
    public Timer useDelayTimer { get; set; }
    public PlayerStateSpeeds CurrentStateSpeed { get; set; }
    public PlayerAnimationStates CurrentAnimation { get; internal set; }

    /// <summary>
    /// Determines if player has some object reparented to him self (Gun, basket...)
    /// </summary>
    public bool IsHoldingWeapon { get; set; }

    /// <summary>
    /// Determines if player is carrying something in front of him (Ingredient, Fuse...)
    /// </summary>
    public bool IsCarrying { get; set; }
	public bool CanSprint { get; set; } = true;
    public bool IsUsing { get; set; }
    public float MovementSpeed { get; internal set; } = (float)PlayerStateSpeeds.Walk;

    public float CurrentStamina
    {
        get => currentStamina;
        set => currentStamina = Mathf.Clamp(value, 0, (float)SprintThresholds.Max);
    }

    /// <summary>
    /// Calculates movement speed for next frame
    /// Handles interpolating between current MovementSpeed and CurrentStateSpeed
    /// </summary>
    /// <returns></returns>
    private float CalculateMovementSpeed()
    {
        MovementSpeed = Mathf.Lerp(MovementSpeed, (float)CurrentStateSpeed, 0.5f);
		return MovementSpeed;
    }

    public delegate void UsableObjectChangedEventHandler(HudCrosshairStates crosshairState);
	public event UsableObjectChangedEventHandler OnPlayerUpdateCrosshairTexture;

	[Signal]
	public delegate void OnRaycastCollideEventHandler(Node colidedObject);

	private PlayerController PlayerController;
	private RayCast3D RayCast;
    private PlayerAnimationStates currentAnimation;
    private float currentStamina = (float)SprintThresholds.Max;
    private bool isInputDisabled;

    public Player()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

    public void UpdateCrosshairState(HudCrosshairStates newHudState) => OnPlayerUpdateCrosshairTexture?.Invoke(newHudState);

    public override void _Ready()
	{
		Camera = GetNode<Camera3D>(NodeNames.Camera.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(Camera)} at {NodeNames.Camera}");
        CollisionMesh = GetNode<CollisionShape3D>(NodeNames.PlayerCollision.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(CollisionMesh)} at {NodeNames.PlayerCollision}");
        RayCast = Camera.GetNode<RayCast3D>(NodeNames.RayCast.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(RayCast)} at {NodeNames.RayCast}");
        CarryableItemPositionMarker = Camera.GetNode<Marker3D>(NodeNames.Camera_CarryableItemPositionMarker.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(CarryableItemPositionMarker)} at {NodeNames.Camera_CarryableItemPositionMarker}");
        EquipableItemPositionMarker = Camera.GetNode<Marker3D>(NodeNames.Camera_EquipableItemPosition.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(EquipableItemPositionMarker)} at {NodeNames.Camera_EquipableItemPosition}");
        AnimationTree = GetNode<AnimationTree>(NodeNames.AnimationTree.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(AnimationTree)} at {NodeNames.AnimationTree}");
        useDelayTimer = GetNode<Timer>(NodeNames.UseDelayTimer.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(useDelayTimer)} at {NodeNames.UseDelayTimer}");
         
        PlayerController = new PlayerController(this);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed(ActionNames.DEBUG_TOGGLEMOUSE))
		{
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
		}

        if (isInputDisabled)
        {
            return;
        }

        // Do not move these lines. The order must be maintained Because some actions have higher priority for animation
        CurrentStateSpeed = PlayerStateSpeeds.Walk;

        PlayerController.HandleCrouch(delta);
		PlayerController.HandleSprint();
		PlayerController.HandleMovement(delta);
		PlayerController.UpdateLookAtObject(RayCast);

        if (Input.IsActionJustPressed(ActionNames.Use) && !IsUsing && useDelayTimer.TimeLeft == 0)
        {
            PlayerController.TryUse();
        }

        if (Input.IsActionJustReleased(ActionNames.Use) && IsUsing)
        {
            PlayerController.StopUsing();
        }

        // Animation is handled at the end of the frame so animations with priority are used
        PlayerController.UpdateHandAnimation();
        CalculateMovementSpeed();
    }

	public override void _Input(InputEvent @event)
	{
        if (isInputDisabled)
        {
            return;
        }

		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			PlayerController.RotateByMouseDelta(eventMouseMotion.Relative, Camera);
		}
    }

    public void HandleJumpscare(Vector3 JumpscarePosition, Vector3 enemyPosition)
    {
        CurrentAnimation = PlayerAnimationStates.Jumpscare;

        Camera.GlobalPosition = JumpscarePosition;
        Camera.LookAt(enemyPosition);
        isInputDisabled = true;
    }
}
