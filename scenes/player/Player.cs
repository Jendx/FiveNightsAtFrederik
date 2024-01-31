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
    public bool IsHoldingWeapon { get; set; }
	public bool CanSprint { get; set; } = true;
	public PlayerStateSpeeds CurrentStateSpeed { get; set; }
    public PlayerAnimationStates CurrentAnimation { get; set; }

    public float CurrentStamina
    {
        get => currentStamina;
        set => currentStamina = Mathf.Clamp(value, 0, (float)SprintThresholds.Max);
    }

    public float MovementSpeed 
	{ 
		get
		{
            movementSpeed = Mathf.Lerp(movementSpeed, (float)CurrentStateSpeed, 0.1f);
			return movementSpeed;
        } 
	}

    public delegate void UsableObjectChangedEventHandler(HudCrosshairStates crosshairState);
	public event UsableObjectChangedEventHandler OnPlayerUpdateCrosshairTexture;

	[Signal]
	public delegate void OnRaycastColideEventHandler(Node colidedObject);

	private PlayerController PlayerController;
	private RayCast3D RayCast;
    private float movementSpeed = (float)PlayerStateSpeeds.Walk;
    private float currentStamina = (float)SprintThresholds.Max;
    private bool isInputDisabled;
    private PlayerAnimationStates currentAnimation;
    private bool isInteracting;

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
        
        PlayerController = new PlayerController(this);
        AnimationTree.AnimationFinished += (animationName) =>
        {
            if (animationName.ToString().Equals(PlayerAnimationStates.Press.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                isInteracting = false;
                GD.Print("IS NOT INTERACTING");
            }
        };
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

        // Do not move these lines. Animation must be handled at start
        CurrentStateSpeed = PlayerStateSpeeds.Walk;
        PlayerController.HandleHandAnimation();

        PlayerController.HandleCrouch(delta);
		PlayerController.HandleSprint();
		PlayerController.HandleMovement(delta);
		PlayerController.UpdateLookAtObject(RayCast);
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

        if (Input.IsActionJustPressed(ActionNames.Use) && !isInteracting)
        {
            GD.Print("Was used");
            isInteracting = PlayerController.TryUse();
        }

        if (Input.IsActionJustReleased(ActionNames.Use) && isInteracting)
        {
            GD.Print("STOP USING");
            PlayerController.StopUsing();
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
