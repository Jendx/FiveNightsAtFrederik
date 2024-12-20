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

	public Camera3D Camera { get; private set; }
	public CollisionShape3D CollisionMesh { get; private set; }
	public Marker3D CarryableItemPositionMarker { get; private set; }
	public Marker3D EquipableBasketPositionMarker { get; private set; }
	
	/// <summary>
	/// Marker which holds location for any holdable Item, taht will be reparented to player (Gun, basket...)
	/// </summary>
	public Marker3D EquipableItemPositionMarker { get; private set; }
	public AnimationTree AnimationTree { get; private set; }

	// Wait time must be equal to the animationTransitionTime (0.6 currently to transition from and back pressing)
	public Timer useDelayTimer { get; set; }
	public PlayerSpeeds CurrentStateSpeed { get; internal set; }
	public PlayerAnimationStates CurrentAnimation { get; internal set; }

	/// <summary>
	/// Determines if player has some object reparented to him self (Gun, basket...)
	/// </summary>
	public bool IsHoldingItem { get; set; }

	/// <summary>
	/// Determines if player is carrying something in front of him (Ingredient, Fuse...)
	/// </summary>
	public bool IsCarryingItem { get; set; }
	public bool IsUsing { get; set; }
	public bool IsPlayingMinigame { get; set; }
	public bool CanSprint { get; internal set; } = true;
	public float MovementSpeed { get; internal set; } = (float)PlayerSpeeds.Walk;

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
	private Hud hud;
	private PlayerAnimationStates currentAnimation;
	private float currentStamina = (float)SprintThresholds.Max;
	private bool isInputDisabled;

	public Player()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Ready()
	{
		Camera = this.TryGetNode<Camera3D>(NodeNames.Camera, nameof(Camera));
		CollisionMesh = this.TryGetNode<CollisionShape3D>(NodeNames.PlayerCollision, nameof(CollisionMesh));
		AnimationTree = this.TryGetNode<AnimationTree>(NodeNames.AnimationTree, nameof(AnimationTree));
		useDelayTimer = this.TryGetNode<Timer>(NodeNames.UseDelayTimer, nameof(useDelayTimer));
		RayCast = Camera.TryGetNode<RayCast3D>(NodeNames.RayCast, nameof(RayCast));
		CarryableItemPositionMarker = Camera.TryGetNode<Marker3D>(NodeNames.Camera_CarryableItemPositionMarker, nameof(CarryableItemPositionMarker));
		EquipableItemPositionMarker = Camera.TryGetNode<Marker3D>(NodeNames.Camera_GunPosition, nameof(EquipableItemPositionMarker));
		EquipableBasketPositionMarker = Camera.TryGetNode<Marker3D>(NodeNames.Camera_BasketPosition, nameof(EquipableBasketPositionMarker));
        hud = Camera.TryGetNode<Hud>(NodeNames.Camera_Hud, nameof(hud));
		PlayerController = new PlayerController(this);
	}

	public void UpdateCrosshairState(HudCrosshairStates newHudState) => hud?.UpdateCrosshairTexture(newHudState);

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed(ActionNames.DEBUG_TOGGLEMOUSE))
		{
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
		}

		if (isInputDisabled || IsPlayingMinigame)
		{
			return;
		}

		// Do not move these lines. The order must be maintained Because some actions have higher priority for animation
		CurrentStateSpeed = PlayerSpeeds.Walk;

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
		PlayerController.HandleHeldItemAnimations();
		PlayerController.UpdateHandAnimation();
		
		CalculateMovementSpeed();
	}

	public override void _Input(InputEvent @event)
	{
		if (isInputDisabled || IsPlayingMinigame)
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

	public void HideHud() => hud.Hide();

	public void ShowHud() => hud.Show();
}
